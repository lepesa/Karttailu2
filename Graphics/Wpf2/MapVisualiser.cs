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

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace Karttailu2.Graphics.Wpf2
{
    public partial class MapVisualiser : IMapVisualiser
    {
        DrawingSettings dwSettings = null;
        Karttailu2.Data.Generic.MBR mapMBR = null;

        Dictionary<int, Karttailu2.Graphics.ClassSettings> drawClasSettings = null;

        Dictionary<int, SolidColorBrush> drawReunaviivaSettings = null;
        Dictionary<int, SolidColorBrush> drawEpamaarainenReunaviivaSettings = null;

        Dictionary<int, List<Data.Generic.Record>> zIndexes = new Dictionary<int, List<Data.Generic.Record>>();

        ILog logger = null;

        double scaleX, scaleY;
        double topX, topY;

        /// <summary>
        /// Lisää recordin tiettyyn zIndeksiryhmään.
        /// </summary>
        /// <param name="record">Recordi</param>
        /// <param name="zIndex">Z-indeksi</param>
        private void AddZIndex(Data.Generic.Record record, int zIndex)
        {
            if (!zIndexes.ContainsKey(zIndex))
            {
                zIndexes[zIndex] = new List<Data.Generic.Record>();
            }
            zIndexes[zIndex].Add(record);
        }

        /// <summary>
        /// Palauttaa recordin id:n perusteella. Ei käytössä
        /// </summary>
        /// <param name="id">Recordin id</param>
        /// <returns>Recordi</returns>
        public Karttailu2.Data.Generic.Record GetGraphicsElement(string id)
        {
                return null;
        }

        /// <summary>
        /// Kirjoittaa lokiin
        /// </summary>
        /// <param name="text">Kirjoitettava teksti</param>
        private void WriteLog(string text)
        {
            logger.WriteLog(text);
        }

        /// <summary>
        /// Lisää elementin canvakselle.
        /// </summary>
        /// <param name="element"></param>
        private void AddItem(UIElement element)
        {
            mainCanvas.Children.Add(element);  
        }

        /// <summary>
        /// Lisää elementin canvakselle ja asettaa sen koordinaatit
        /// </summary>
        /// <param name="element"></param>
        private void AddItem(UIElement element, double x, double y)
        {
            mainCanvas.Children.Add(element);
            Canvas.SetLeft(element, x);
            Canvas.SetTop(element, y);
        }


        Canvas mainCanvas;

        /// <summary>
        /// Piirtää kartan annetusta datasta
        /// </summary>
        /// <param name="canvas">Piirtopinta</param>
        /// <param name="layers">Ladattu kartta</param>
        /// <param name="drawSettings">Asetukset</param>
        /// <param name="mbr">Kartan koko</param>
        /// <param name="_logger">Lokiluokka</param>
        /// <param name="cWidth">Kartan leveys pikseleinä</param>
        /// <param name="cHeight">Kartan korkeus pikseleinä</param>

        public void DrawMap(Canvas canvas, List<Data.Generic.Layer> layers, DrawingSettings drawSettings, Karttailu2.Data.Generic.MBR mbr, ILog _logger, double cWidth, double cHeight)
        {
            logger = _logger;
            dwSettings = drawSettings;
            mapMBR = mbr;

            mainCanvas = canvas;

            drawClasSettings = dwSettings.GetSettings();
            drawReunaviivaSettings = dwSettings.GetReunaviivaSettings();
            drawEpamaarainenReunaviivaSettings = dwSettings.GetEpamaarainenReunaviivaSettings();

            // Lasketaan rajat/skaalaus

            double sizeX = (mbr.MbrMaxX - mbr.MbrMinX);
            double sizeY = (mbr.MbrMaxY - mbr.MbrMinY);

            double borderSize = 64;

            scaleX = ((double)cWidth - borderSize) / (sizeX);
            scaleY = ((double)cHeight - borderSize) / (sizeY);

            topX = mbr.MbrMinX - (borderSize / 2);
            topY = mbr.MbrMaxY + (borderSize / 2);


            WriteLog("Mapvisualizer: Wpf2");

            // Lasketaan zIndeksit etukäteen.

            // Polygon
            foreach (var layer in layers)
            {
                foreach (var record in layer.PolygonRecords)
                {
                    CalculatePolygonZ(record);
                }
                WriteLog("Map has " + layer.PolygonRecords.Count + " polygons.");
            }

            // Polyline, ei täytettyä aluetta

            foreach (var layer in layers)
            {
                foreach (var record in layer.PolylineRecords)
                {
                    CalculatePolylineZ(record);
                }
                WriteLog("Map has " + layer.PolylineRecords.Count + " polylines.");
            }

            // Piste
            foreach (var layer in layers)
            {
                foreach (var record in layer.PointRecords)
                {
                    CalculatePointZ(record);
                }
                WriteLog("Map has " + layer.PointRecords.Count + " points.");
            }

            // Nyt kun zIndeksit on kerätty, piirretään ne pienimmästä alkaen

            // Lajitellaan ensiksi ne järjestykseen
            List<int> keyList = new List<int>(zIndexes.Keys);
            keyList.Sort();

            // Käydään kaikki läpi
            foreach(int i in keyList)
            {
                // Piirretään kaikki sen zIndeksin elementit
                foreach (var rec in zIndexes[i])
                {
                  
                    if (rec is Data.Generic.PolylineRecord)
                    {
                        DrawPolyline( (Data.Generic.PolylineRecord)rec, i);
                    }
                    else if (rec is Data.Generic.PolygonRecord)
                    {
                        DrawPolygon((Data.Generic.PolygonRecord)rec, i);

                    } else if( rec is Data.Generic.PointRecord)
                    {
                        DrawPoint((Data.Generic.PointRecord)rec, i);
                    }
                }
                // valmis.   
            }
        }
    }
}
