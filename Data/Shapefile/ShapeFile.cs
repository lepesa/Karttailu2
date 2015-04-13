/*
   Copyright 2015 Esa Leppänen

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;


namespace Karttailu2.Data.Shapefile
{
    /// <summary>
    /// MapFilen toteuttamat rutiinit. Lataa Shapefilen ja palauttaa geneerisen tietorakenteen.
    /// </summary>
    class ShapeFile : Karttailu2.Data.IMapfile
    {
        // Juokseva id

        private static int genericRecordId = 1;

        /// <summary>
        /// Haetaan kartta käyttäen oletus filemaskia.
        /// </summary>
        /// <param name="filePath">Hakemisto, jossa tiedostot ovet</param>
        /// <returns>Kartan datarakenne</returns>
        public Data.Generic.Layer LoadMap(string filePath)
        {
            return LoadMap(filePath, "*.shp");
        }

        /// <summary>
        /// Lataa ja konversoi kaikki tiedosto
        /// </summary>
        /// <param name="filePath">Hakemisto, jossa tiedostot ovat</param>
        /// <returns>Kartan datarakenne</returns>
        public Data.Generic.Layer LoadMap(string filePath, string fileMask)
        {
            Data.Generic.Layer genericLayer = new Data.Generic.Layer();
            
            //Shapefile.Layer layer;
            var sf = new Karttailu2.Data.Shapefile.ShapeFile();
            
            string[] fileNames = System.IO.Directory.GetFiles(filePath, fileMask);

            foreach (var fullName in fileNames)
            {
                //WriteLog("Loading: " + fileName);
                var layer = sf.LoadLayer(System.IO.Path.GetFileNameWithoutExtension(fullName), System.IO.Path.GetDirectoryName(fullName));

                #region ConvertLayer
                // Boksin maksimidimensio talteen
                genericLayer.Mbr.CheckMaxMBR(layer.Header.Mbr.MbrMinX, layer.Header.Mbr.MbrMinY, layer.Header.Mbr.MbrMaxX, layer.Header.Mbr.MbrMaxY);

                // Headerin tiedot talteen
                genericLayer.FileName = layer.FileName;

                // Recordit talteen

                foreach(var record in layer.Records)
                {
                    // Piste
                    if( layer.Header.ShapeType == 1 )
                    {
                        genericLayer.PointRecords.Add(ConvertPointRecord(record as PointRecord));   
                    }
                    // Polyline
                    else if(layer.Header.ShapeType == 3)
                    {
                        genericLayer.PolylineRecords.Add(ConvertPolylineRecord(genericLayer, record as PolylineRecord));   
                    }
                    // Polygon
                    else if(layer.Header.ShapeType == 5)
                    {
                        genericLayer.PolygonRecords.Add(ConvertPolygonRecord(genericLayer, record as PolygonRecord));   
                    } else
                    {
                        // Tuntematon muoto
                        throw new Exception("Conversion: Unknown Shapefile record type.");
                    }
                }
                
                #endregion 
            }

            return genericLayer;
        }

        /// <summary>
        /// Konvertoi polylinen geneeriseen muotoon.
        /// </summary>
        /// <param name="record">Polyline olio shapefile muodossa</param>
        /// <returns>Polyline olio geneerisessa muodossa.</returns>
        private Data.Generic.PolylineRecord ConvertPolylineRecord(Data.Generic.Layer genericLayer, PolylineRecord record)
        {
            var genericRecord = new Data.Generic.PolylineRecord();

            // Id ja tyyppi
            genericRecord.Id = genericRecordId++;
            genericRecord.Type = Data.Generic.ShapeType.Polyline;

            // Mbr
            genericLayer.Mbr.CheckMaxMBR(record.Box.MbrMinX, record.Box.MbrMinY, record.Box.MbrMaxX, record.Box.MbrMaxY);
                       
            // Objectien ja niiden pisteiden tiedot. 
            
            // Varataan tila objekteille
            genericRecord.Objects = new Data.Generic.Object[record.NumParts];

            for (int i = 0; i < record.NumParts; i++ )
            {
                // Varataan tila objektin pisteille
                genericRecord.Objects[i] = new Data.Generic.Object();
                genericRecord.Objects[i].Points = new Data.Generic.Point[record.Parts[i].Points.Length];

                // Siirretään pisteet objektiin
                for(int j=0; j<record.Parts[i].Points.Length; j++)
                {
                    genericRecord.Objects[i].Points[j] = new Data.Generic.Point(record.Parts[i].Points[j].X, record.Parts[i].Points[j].Y);
                }
            }

            //Attribuutit

            genericRecord.Attribute.Teksti = record.Attribute.Teksti;
            genericRecord.Attribute.Ryhma = record.Attribute.Ryhma;
            genericRecord.Attribute.Luokka = record.Attribute.Luokka;
            genericRecord.Attribute.Tastar = record.Attribute.Tastar;
            genericRecord.Attribute.Kortar = record.Attribute.Kortar;
            genericRecord.Attribute.KorArv = record.Attribute.KorArv;
            genericRecord.Attribute.Kulkutapa = record.Attribute.Kulkutapa;
            genericRecord.Attribute.Versuh = (int)record.Attribute.Versuh;
            genericRecord.Attribute.Suunta = record.Attribute.Suunta;
            genericRecord.Attribute.Siirt_Dx = record.Attribute.Siirt_Dx;
            genericRecord.Attribute.Siirt_Dy = record.Attribute.Siirt_Dy;
            genericRecord.Attribute.Korkeus = record.Attribute.Korkeus;
            genericRecord.Attribute.KartoGlk = (int)record.Attribute.Kartoglk;
            return genericRecord;
        }

        /// <summary>
        /// Konvertoi polygonin geneeriseen muotoon.
        /// </summary>
        /// <param name="record">Polygon olio shapefilen muodossa</param>
        /// <returns>Polygon olio geneerisessä muodossa</returns>
        private Data.Generic.PolygonRecord ConvertPolygonRecord(Data.Generic.Layer genericLayer, PolygonRecord record)
        {
            var genericRecord = new Data.Generic.PolygonRecord();

            // Id ja tyyppi
            genericRecord.Id = genericRecordId++;
            genericRecord.Type = Data.Generic.ShapeType.Polygon;

            // Mbr
            genericLayer.Mbr.CheckMaxMBR(record.Box.MbrMinX, record.Box.MbrMinY, record.Box.MbrMaxX, record.Box.MbrMaxY);

            // Objectien ja niiden pisteiden tiedot. 

            // Varataan tila objekteille
            genericRecord.Objects = new Data.Generic.Object[record.NumParts];

            for (int i = 0; i < record.NumParts; i++)
            {
                // Varataan tila objektin pisteille
                genericRecord.Objects[i] = new Data.Generic.Object();
                genericRecord.Objects[i].Points = new Data.Generic.Point[record.Parts[i].Points.Length];

                // Siirretään pisteet objektiin
                for (int j = 0; j < record.Parts[i].Points.Length; j++)
                {
                    genericRecord.Objects[i].Points[j] = new Data.Generic.Point(record.Parts[i].Points[j].X, record.Parts[i].Points[j].Y);
                }
            }

            //Attribuutit

            genericRecord.Attribute.Teksti = record.Attribute.Teksti;
            genericRecord.Attribute.Ryhma = record.Attribute.Ryhma;
            genericRecord.Attribute.Luokka = record.Attribute.Luokka;
            genericRecord.Attribute.Tastar = record.Attribute.Tastar;
            genericRecord.Attribute.Kortar = record.Attribute.Kortar;
            genericRecord.Attribute.KorArv = record.Attribute.KorArv;
            genericRecord.Attribute.Kulkutapa = record.Attribute.Kulkutapa;
            genericRecord.Attribute.Versuh = (int)record.Attribute.Versuh;
            genericRecord.Attribute.Suunta = record.Attribute.Suunta;
            genericRecord.Attribute.Siirt_Dx = record.Attribute.Siirt_Dx;
            genericRecord.Attribute.Siirt_Dy = record.Attribute.Siirt_Dy;
            genericRecord.Attribute.Korkeus = record.Attribute.Korkeus;
            return genericRecord;
        }

        /// <summary>
        /// Konveroit Shapefilen recordin geneeriseksi tietotyypiksi
        /// </summary>
        /// <param name="record">Shapefilen PointRecord</param>
        /// <returns>Geneerinen PointRecord</returns>
        private Data.Generic.PointRecord ConvertPointRecord(PointRecord record)
        {
          

            var genericRecord = new Data.Generic.PointRecord();

            // Id, tyyppi ja pisteen datat
            genericRecord.Id = genericRecordId++;
            genericRecord.Type = Data.Generic.ShapeType.Point;
            genericRecord.Point.X = record.Point.X;
            genericRecord.Point.Y = record.Point.Y;
            
            //Attribuutit

            genericRecord.Attribute.Teksti = record.Attribute.Teksti;
            genericRecord.Attribute.Ryhma = record.Attribute.Ryhma;
            genericRecord.Attribute.Luokka = record.Attribute.Luokka;
            genericRecord.Attribute.Tastar = record.Attribute.Tastar;
            genericRecord.Attribute.Kortar = record.Attribute.Kortar;
            genericRecord.Attribute.KorArv = record.Attribute.KorArv;
            genericRecord.Attribute.Kulkutapa = record.Attribute.Kulkutapa;
            genericRecord.Attribute.Versuh = (int)record.Attribute.Versuh;
            genericRecord.Attribute.Suunta = record.Attribute.Suunta;
            genericRecord.Attribute.Siirt_Dx = record.Attribute.Siirt_Dx;
            genericRecord.Attribute.Siirt_Dy = record.Attribute.Siirt_Dy;
            genericRecord.Attribute.Korkeus = record.Attribute.Korkeus;
            genericRecord.Attribute.KartoGlk = (int)record.Attribute.Kartoglk;
            return genericRecord;
        }

        /// <summary>
        /// Lataa layerin tiedot sisään. Huom! ignoorattu SQL Injektio varoitus, koska taulun nimeä ei saa parametrisoitua.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public Layer LoadLayer(string fileName, string filePath)
        {
            // Tätä ei olla vielä pohdittu mitä tehdä. Shapefilen luvussa tarvitsee otttaa huomioon, että saadaan luvut luettua oikein.
            if (!BitConverter.IsLittleEndian)
            {
                throw new NotImplementedException("No Big Endian CPU support.");
            }
            
            Layer layer = new Layer();
            layer.FileName = fileName;

            // Luetaan shapefile kokonaan muistiin
            byte[] buffer = null;
            try
            {
                buffer = File.ReadAllBytes(Path.Combine(filePath,fileName + ".shp"));
            }
            catch(Exception e)
            {
                throw new Exception("Filename error." + e.Message);
            }

            #region MainHeader

            MainHeader mh = new MainHeader();
            
            // Tiedoston tunniste
            mh.FileCode = ReadBigEndianInt32(buffer,0);

            // Jos väärä tunniste, niin ei ole shapefile.
            if( mh.FileCode != 9994)
            {
                throw new Exception("Not a shapefile! ID: " + mh.FileCode);
            }

            // Täytetään mainheaderin tiedot
            mh.FileLengthWords = ReadBigEndianInt32(buffer, 24);
            mh.FileLength = mh.FileLengthWords * 2;
            mh.Version = ReadLittleEndianInt32(buffer, 28);
            mh.ShapeType= ReadLittleEndianInt32(buffer, 32);
            mh.Mbr = ReadMbr(buffer, 36);
            mh.MinM = ReadLittleEndianDouble(buffer, 68);
            mh.MaxM = ReadLittleEndianDouble(buffer, 76);
            mh.MinZ = ReadLittleEndianDouble(buffer, 84);
            mh.MaxZ = ReadLittleEndianDouble(buffer, 92);

            layer.Header = mh;
            
            #endregion
            #region ReadAttributes

            int fileIndex = 100;
            
            // Lue attribuutit DBASE IV tiedostosta

            DataSet ds;
            try
            {
                string dbfFileName = fileName + ".dbf";

                // Taulun nimi on 8.3 muodossa
                dbfFileName = FileNameHelper.GetShortPathName(filePath, dbfFileName);

                string constr = DBaseIVHelper.GetConnectionString(filePath);

                // Haetaan shapefilen kaikki metatiedot
                using (OleDbConnection con = new OleDbConnection(constr))
                {
                    // Tästä tulee SQL Injektio varoitus. 
                    var sql = "select * from [" + dbfFileName + "]";
                    OleDbCommand cmd = new OleDbCommand(sql, con);
                    con.Open();
                    ds = new DataSet();
                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                    da.Fill(ds);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }

            #endregion
            #region ReadShapes

            // Lue tiedoston sisältämät kuviot. Kts. speksit 

            // Jatketaan, kunnes ollaan tiedoston lopussa
            while (fileIndex < mh.FileLength)
            {
                // Ensimmäiseksi saadaan recordin numero ja pituus.

                int recordNbr = ReadBigEndianInt32(buffer, fileIndex);
                int recordLength = ReadBigEndianInt32(buffer, fileIndex + 4);
                fileIndex += 8;

                // Sitten recordin tyyppi (piste, polyline, polygon)

                int shapeType = ReadLittleEndianInt32(buffer, fileIndex);
                int subIndex = fileIndex;

                // Fileindeksi siirretään recordin loppuun. SubIndeksi sisältää aliohjelman tarvitsemat tiedot
                fileIndex += (recordLength*2);
                
                // Piste
                if (shapeType == 1)
                {
                    PointRecord plr = ReadPointRecord(buffer, subIndex, ds.Tables[0].Rows[recordNbr - 1], recordNbr);
                    layer.Records.Add(plr);
                }

                // Polyline: MBR, Number of parts, Number of points, Parts, Points
                else if( shapeType == 3 )
                {
                    PolylineRecord plr = ReadPolyLineRecord(buffer, subIndex, ds.Tables[0].Rows[recordNbr - 1], recordNbr);
                    layer.Records.Add(plr);
                } 
                // Polygon
                else if(shapeType == 5)
                {
                    PolygonRecord plr = ReadPolygonRecord(buffer, subIndex, ds.Tables[0].Rows[recordNbr - 1], recordNbr);
                    layer.Records.Add(plr);
                }
                else
                {
                    throw new Exception(String.Format("Unknown shapetype: {0}", shapeType));
                }
            }
            ds.Dispose();

            #endregion

            return layer;


        }

        /// <summary>
        /// Lukee pisteen sisään tiedostosta.
        /// </summary>
        /// <param name="buffer">Shapefile data</param>
        /// <param name="index">Indeksi dataan</param>
        /// <param name="dr">Attribuutin rivi</param>
        /// <param name="number">Recordin numero</param>
        /// <returns>Pisteen tiedot</returns>
        private PointRecord ReadPointRecord(byte[] buffer, int index, DataRow dr, int number)
        {

            PointRecord plr = new PointRecord();
            plr.Number = number;

            // Luetaan ja asetetaan attribuutit
            plr.ReadAttributes(dr);

            // Luetaan pisteen tiedot 
            plr.Point = new Point();
            plr.Point.X = ReadLittleEndianDouble(buffer, index + 4);
            plr.Point.Y = ReadLittleEndianDouble(buffer, index + 12);
            
            return plr;
        }

        /// <summary>
        /// Polylinen tiedot rakenteeseen. Polyline voi koostua useammasta osasta.
        /// </summary>
        /// <param name="buffer">Shapefile data</param>
        /// <param name="index">Indeksi dataan</param>
        /// <param name="dr">Attribuutin rivi</param>
        /// <param name="number">Recordin numero</param>
        /// <returns>Polylinen tiedot</returns>
        private PolylineRecord ReadPolyLineRecord(byte[] buffer, int index, DataRow dr, int number)
        {
         
            PolylineRecord plr = new PolylineRecord();
            plr.Number = number;

            // Luetaan attribuutit ja MBR
            plr.ReadAttributes(dr);
            
            plr.Box = ReadMbr(buffer, index+4);

            // Osat ja pisteiden lukumäärä muistiin
            int numParts = ReadLittleEndianInt32(buffer, index+36);
            int numPoints = ReadLittleEndianInt32(buffer, index+40);

            plr.NumParts = numParts;
            plr.NumPoints = numPoints;
            
            plr.Parts = new PointPart[numParts];

            int pointIndex = index + 44 + 4 * numParts;

            int fromIndex;
            int toIndex;

            // Luetaan osien indeksit muistiin            
            for (int i = 0; i < numParts; i++)
            {
                plr.Parts[i] = new PointPart();
                plr.Parts[i].Index = ReadLittleEndianInt32(buffer, index + 44 + i * 4);
            }

            // Luetaan osien pisteet muistiin
            for(int i=0; i<numParts; i++)
            {
            
                // Mistä kohdasta osa alkaa
                fromIndex = plr.Parts[i].Index;

                // Jos on viimeinen osa, päättyy pistejoukon loppuun
                if (numParts == i + 1)
                {
                    toIndex = numPoints;
                }
                else
                {
                    // Muussa tapauksessa päättyy seuraavan osan alkuun
                    toIndex = plr.Parts[i + 1].Index;
                }

                // Varataan tilaa pisteille
                plr.Parts[i].Points = new Point[toIndex - fromIndex];


                // Luetaan osan kaikki pisteet tietorakenteeseen. 
                // Yksi piste on 16 tavua pitkä (kaksi doublea), alkaa fromIndexista ja loppuu toIndexiin shapefilessä.
                for (int k = 0; k < toIndex - fromIndex; k++)
                {
                    plr.Parts[i].Points[k] = ReadPoint(buffer, pointIndex + (fromIndex +k) * 16);
                }

            }

            return plr;
        }

        /// <summary>
        /// Lukee polygonin tietorakenteeseen. Polygoni voi koostua useammasta osasta
        /// </summary>
        /// <param name="buffer">Shapefile data</param>
        /// <param name="index">Indeksi dataan</param>
        /// <param name="dr">Attribuutin rivi</param>
        /// <param name="number">Recordin numero</param>
        /// <returns>Polygonin tiedot</returns>
        private PolygonRecord ReadPolygonRecord(byte[] buffer, int index, DataRow dr, int number)
        {

            var plr = new PolygonRecord();
            plr.Number = number;
            
            // Luetaan attribuutit ja Mbr:N tiedot
            plr.ReadAttributes(dr);

            plr.Box = ReadMbr(buffer, index + 4);

            // osien ja pisteiden määrä 
            int numParts = ReadLittleEndianInt32(buffer, index + 36);
            int numPoints = ReadLittleEndianInt32(buffer, index + 40);

            plr.NumParts = numParts;
            plr.NumPoints = numPoints;

            plr.Parts = new PointPart[numParts];

            // Osien indeksit alkaa headerin jälkeen
            int pointIndex = index + 44 + 4 * numParts;

            int fromIndex;
            int toIndex;
            
            // Luetaan indeksit muistiin
            for (int i = 0; i < numParts; i++)
            {
                plr.Parts[i] = new PointPart();
                plr.Parts[i].Index = ReadLittleEndianInt32(buffer, index + 44 + i * 4);
            }

            // Luetaan osien pisteet muistiin
            for (int i = 0; i < numParts; i++)
            {
                // Mistä kohdasta osa alkaa
                fromIndex = plr.Parts[i].Index;

                // Jos on viimeinen osa, päättyy pistejoukon loppuun
                if (numParts == i + 1)
                {
                    toIndex = numPoints;
                }
                else
                {
                    // Muussa tapauksessa päättyy seuraavan osan alkuun
                    toIndex = plr.Parts[i + 1].Index;
                }

                // Varataan tilaa pisteille
                plr.Parts[i].Points = new Point[toIndex - fromIndex];


                // Luetaan osan kaikki pisteet tietorakenteeseen. 
                // Yksi piste on 16 tavua pitkä (kaksi doublea), alkaa fromIndexista ja loppuu toIndexiin shapefilessä.
                for (int k = 0; k < toIndex - fromIndex; k++)
                {
                    plr.Parts[i].Points[k] = ReadPoint(buffer, pointIndex + (fromIndex + k) * 16);
                }
            }
            return plr;
        }

        /// <summary>
        /// Lukee pisteen tiedot annetusta puskurista.
        /// </summary>
        /// <param name="buffer">Tiedoston muistialue</param>
        /// <param name="pointIndex">Indeksi pisterakenteen alkuun</param>
        /// <returns>Pisteen tiedot objektina</returns>
        private Point ReadPoint(byte[] buffer, int pointIndex)
        {
            Point pt = new Point();
            pt.X = ReadLittleEndianDouble(buffer, pointIndex);
            pt.Y = ReadLittleEndianDouble(buffer, pointIndex + 8);
            return pt;
        }

        /// <summary>
        /// Lukee Mbr -tietorakenteen ja palauttaa sen.
        /// </summary>
        /// <param name="buffer">Tiedoston muistialue</param>
        /// <param name="pointIndex">Indeksi pisterakenteen alkuun</param>
        /// <returns>Mbr:n tiedot objektina</returns>
        private MBR ReadMbr(byte[] buffer, int index)
        {
            MBR mbr = new MBR();
            mbr.MbrMinX = ReadLittleEndianDouble(buffer, index);
            mbr.MbrMinY = ReadLittleEndianDouble(buffer, index+8);
            mbr.MbrMaxX = ReadLittleEndianDouble(buffer, index+16);
            mbr.MbrMaxY = ReadLittleEndianDouble(buffer, index+24);
            return mbr;
        }

        /// <summary>
        /// Lukee Int32 tiedostosta, rakenteen BigEndian
        /// </summary>
        /// <param name="bff">Tiedoston muistialue</param>
        /// <param name="index">Indeksi luettavan tiedon alkuun</param>
        /// <returns>Int32 luvun</returns>
        private Int32 ReadBigEndianInt32(byte[] bff, int index)
        {
            Int32 ret = 0;
            ret = (ret << 8) + (Int32)bff[index];
            ret = (ret << 8) + (Int32)bff[index + 1];
            ret = (ret << 8) + (Int32)bff[index + 2];
            ret = (ret << 8) + (Int32)bff[index + 3];

            return ret;
        }

        /// <summary>
        /// Lukee Int32 tiedostosta, rakenteena LittleEndian
        /// </summary>
        /// <param name="bff">Tiedoston muistialue</param>
        /// <param name="index">Indeksi luettavan tiedon alkuun</param>
        /// <returns>Int32 luvun</returns>
        private Int32 ReadLittleEndianInt32(byte[] bff, int index)
        {
            Int32 ret = 0;
            ret = ((Int32)bff[index]);
            ret += ((Int32)bff[index + 1]) * 256;
            ret += ((Int32)bff[index + 2]) * 256 * 256;
            ret += ((Int32)bff[index + 3]) * 256 * 256 * 256;

            return ret;
        }

        /// <summary>
        /// Lukee doublen tiedostosta, rakenteena LittleEndian
        /// </summary>
        /// <param name="bff"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private double ReadLittleEndianDouble(byte[] bff, int index)
        {
            double ret = BitConverter.ToDouble(bff, index);
            return ret;
        }
    }
}
