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
using System.Windows.Shapes;
using System.Linq;
using System;

namespace Karttailu2.Graphics.Wpf2
{
    public partial class MapVisualiser
    {


        /// <summary>
        /// Lasketaan recordin zIndeksi.
        /// </summary>
        /// <param name="record"></param>

        private void CalculatePolylineZ(Data.Generic.PolylineRecord record)
        {
            var settings = GetSettings(record.Attribute.Luokka, record.Attribute.Ryhma);
            // ei näytetä, jos maan sisässä. tiet poikkeus

            if (record.Attribute.Versuh == -1 && SaakoPiirtaaPinnanAlle(record, settings) == false)
            {
                if (settings.VerSuh != Graphics.VerSuh.EiMitaan)
                {
                    WriteLog("Line Versuh -1 detected. Not shown: " + record.Attribute.Versuh + " record: " + record.Id + "  type:" + record.Attribute.Luokka);
                }
                return;
            }
            
            if (record.Attribute.Versuh != 0 && settings.VerSuh == 0 && record.Attribute.Ryhma != 25 && record.Attribute.Versuh > -2)
            {
                WriteLog("Line Versuh detected: " + record.Attribute.Versuh + " record: " + record.Id + "  type:" + record.Attribute.Luokka);
            }

            // Piirretäänkö luokka?
            if (settings.StrokeThickness == -1)
            {
                return;
            }

            // Käydään viivan objektit läpi

            for (int i = 0; i < record.Objects.Length; i++)
            {
                // Tunneli
                if (record.Attribute.Versuh == -11)
                {
                    AddZIndex(record, settings.ZIndex);
                    continue;
                }
                else if (settings.Shape.Type == 5)
                {
                    // Talvitie katkoviiva
                    // Versuh voi olla myös pinnan alla/päällä (tien ali, joen yli)
                    if (record.Attribute.Versuh != 0)
                    {
                        AddZIndex(record, GetVerSuhZIndex(record.Attribute.Versuh, settings.VerSuh, settings.ZIndex));
                    }
                    else
                    {
                        AddZIndex(record, settings.ZIndex);
                    }
                }
                else if (settings.Shape.Type == 19)
                {
                    // Luonnonsuojelualueen raja
                    AddZIndex(record, settings.ZIndex); 
                }
                else if (settings.Shape.Type == 21)
                {
                    // Jyrkänne
                    AddZIndex(record, settings.ZIndex);
                }
                else if (settings.Shape.Type == 22)
                {
                    // Luiska. Vastaa vähän jyrkännettä.
                    AddZIndex(record, settings.ZIndex);
                }
                else
                {
                    // Tämä suoritetaan vain luokille 30211 ja 30212
                    if (CheckReunaviiva(record) == false)
                    {
                        continue;
                    }

                    // kakkosviivat + pääviivan tyylit

                    if (settings.Shape.Type == 8)
                    {
                        // Tie, kaksivärinen
                        if (record.Attribute.Versuh == 0)
                        {

                            AddZIndex(record, settings.ZIndex + 1);
                        }
                        else if (record.Attribute.Versuh >= 1)
                        {
                            // Silta, tulee tummemmalla
                            AddZIndex(record, settings.ZIndex+3);  
                        }
                        else if (record.Attribute.Versuh == -1)
                        {
                            // Pinnan alla/tunneli
                            AddZIndex(record, GetVerSuhZIndex(record.Attribute.Versuh, settings.VerSuh, settings.ZIndex) + 1);
                        }
                        else if (record.Attribute.Versuh == -11)
                        {
                            // tunneli. Ei pitäisi koskaan tulla tänne
                            WriteLog("Versuh tunneli shape 8, luokka:" + record.Attribute.Luokka + " versuh: " + record.Attribute.Versuh);
                        }
                        else
                        {
                            // Tuntematon versuh
                            WriteLog("TuntematonVersuh shape 8, luokka:" + record.Attribute.Luokka + " versuh: " + record.Attribute.Versuh);
                            throw new Exception("Shape 8, versuh:" + record.Attribute.Versuh);
                        }
                    }
                    // rautatie, kaksivärinen
                    else if (settings.Shape.Type == 36)
                    {
                        if (record.Attribute.Versuh == 0)
                        {
                            AddZIndex(record, settings.ZIndex );
                            AddZIndex(record, settings.ZIndex + 1);
                        }
                        else if (record.Attribute.Versuh == 1)
                        {
                            AddZIndex(record, settings.ZIndex + 3);
                            return;
                            // Rautatiesilta. tämä on yksivärinen
                        }
                        else if (record.Attribute.Versuh == -1)
                        {
                            // Sillan alusta. Piirretään, ettei tule hassuja pätkiä
                            // Eli piirretään normaali kaksivärinen katkoviiva

                            AddZIndex(record, GetVerSuhZIndex(record.Attribute.Versuh, settings.VerSuh, settings.ZIndex) + 1);
                        }
                        else if (record.Attribute.Versuh == -11)
                        {
                            // Tunneli pitäisi tulla muuta kautta, ei tulla koskaan tänne
                            var str = "Versuh tunneli shape 36 rautatie, luokka:" + record.Attribute.Luokka + " versuh: " + record.Attribute.Versuh;
                            WriteLog(str);
                        }
                        else
                        {
                            var str = "Versuh muu kuin 0,1,-1: shape 36 rautatie, luokka:" + record.Attribute.Luokka + " versuh: " + record.Attribute.Versuh;
                            WriteLog(str);
                            throw new Exception(str);
                        }
                    }
                    else if (settings.Shape.Type == 9)
                    {
                        // Aita. Viiva, jossa pisteitä mukana. Piirretään pisteet tässä
                        AddZIndex(record, settings.ZIndex + 1);

                    }

                    var zIndex = settings.ZIndex;

                    // Reunaviiva. Saadaan väri erillisestä tietorakenteesta
                    if (record.Attribute.Luokka == 30211)
                    {
                        zIndex = drawClasSettings[record.Attribute.KartoGlk].ZIndex + 2;
                    }

                    // Epämääräinen reunaviiva. Pistemäinen, saadaan väri erillisestä tietorakenteesta
                    if (record.Attribute.Luokka == 30212)
                    {
                        zIndex = drawClasSettings[record.Attribute.KartoGlk].ZIndex + 2;
                    }


                    // Pinnan alla kulkevat putket: osa on katkoviivalla, osa ei. Katkoviivallisile ei saa muuttaa zIndeksiä
                    if ((record.Attribute.Luokka == 22311 || record.Attribute.Luokka == 22312 || record.Attribute.Luokka == 26111) && record.Attribute.Versuh == -1)
                    {

                    }
                    else
                    {
                        if (record.Attribute.Versuh < 0)
                        {
                            zIndex = GetVerSuhZIndex(record.Attribute.Versuh, settings.VerSuh, settings.ZIndex);
                        }
                    }
                    AddZIndex(record, zIndex);
                }
            }
        }


        /// <summary>
        /// Luo talvitieviivan annetuista pisteistä.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        private Path CreateParallerPath(List<Point> points, Graphics.ClassSettings settings)
        {

            var g1 = new StreamGeometry();
            using (StreamGeometryContext ctx1 = g1.Open())
            {
                ctx1.BeginFigure(GetRelativePoint(points[0]), false, false);
                for (int k = 1; k < points.Count; k++)
                {
                    ctx1.LineTo(GetRelativePoint(points[k]), true, true);
                }
            }

            var pa1 = new Path();

            pa1.Stroke = settings.Stroke;
            pa1.StrokeThickness = GetStrokeThicknessRelative(settings.StrokeThickness);
            pa1.StrokeLineJoin = PenLineJoin.Round;
            pa1.StrokeDashArray = Settings.dcKatkoviivat["Talvitie"];

            g1.Freeze();
            pa1.Data = g1;

            return pa1;

        }


        /// <summary>
        /// Palauttaa saako piirtää VerSuh:n mukaan pinnan alle katkoviiva vai jätetäänkö näyttämättä
        /// </summary>
        /// <param name="record">Objektin record -rivi</param>
        /// <param name="settings">Objektin asetukset</param>
        /// <returns></returns>
        private bool SaakoPiirtaaPinnanAlle(Data.Generic.PolylineRecord record, Karttailu2.Graphics.ClassSettings settings)
        {
            if ((settings.VerSuh == Graphics.VerSuh.Erikois || settings.VerSuh == Graphics.VerSuh.MolemmatOk || record.Attribute.Ryhma == 25 || record.Attribute.Luokka == 22311 || record.Attribute.Luokka == 22312 || record.Attribute.Luokka == 26111))
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Piirtää tunnelin
        /// </summary>
        /// <param name="record">Luokan recordi</param>
        /// <param name="index">Indexksi</param>
        /// <param name="settings">Luokan asetustiedot</param>
        void DrawTunneli(Data.Generic.PolylineRecord record, int index, Graphics.ClassSettings settings)
        {
            var lista = new List<Point>();

            var pointArr = record.Objects[index].Points;

            // Hae kaikki pisteet listaan
            for (int j = 0; j < pointArr.Length; j++)
            {
                lista.Add(new Point(pointArr[j].X, pointArr[j].Y));
            }

            // Viiva on keskellä, tehdään sen molemmille puolille pisteet

            var line1 = ParallerLine.CreateParallerPoints(lista, (settings.Width + settings.Height + 2) / 2);
            var line2 = ParallerLine.CreateParallerPoints(lista, -1 * ((settings.Width + settings.Height + 2) / 2));

            // Luodaan tästä graafinen viiva
            Path tunneliViiva1 = CreateParallerPath(line1, settings);
            Path tunneliViiva2 = CreateParallerPath(line2, settings);

            tunneliViiva1.StrokeThickness = GetStrokeThicknessRelative(settings.Height);
            tunneliViiva2.StrokeThickness = GetStrokeThicknessRelative(settings.Height);

            tunneliViiva1.StrokeDashArray = Settings.dcKatkoviivat["Tunneli"];
            tunneliViiva2.StrokeDashArray = Settings.dcKatkoviivat["Tunneli"];

            // Lisätään canvakselle
          
            AddItem(tunneliViiva1);
            AddItem(tunneliViiva2);          
        }

        /// <summary>
        /// Piirtää talvitien (kaksiviivaa vierekkäin, katkoviivalla).
        /// </summary>
        /// <param name="record">Luokan recordi</param>
        /// <param name="index">Indexksi</param>
        /// <param name="settings">Luokan asetustiedot</param>

        void DrawTalvitie(Data.Generic.PolylineRecord record, int index, Graphics.ClassSettings settings)
        {
            var lista = new List<Point>();
            var pointArr = record.Objects[index].Points;

            // Lisätään pisteet listaan
            for (int j = 0; j < pointArr.Length; j++)
            {
                lista.Add(new Point(pointArr[j].X, pointArr[j].Y));
            }

            // Tehdään molemmin puolin viivaa talvitien kaksoisviivapisteet
            var line1 = ParallerLine.CreateParallerPoints(lista, settings.Width / 2);     // 2
            var line2 = ParallerLine.CreateParallerPoints(lista, -1 * settings.Width / 2);        // -2

            // Luodaan pisteiden perusteella objektit
            Path talviTie1 = CreateParallerPath(line1, settings);
            Path talviTie2 = CreateParallerPath(line2, settings);
        
            // Lisätään ne canvakselle ja asetetaan zindex
            
            AddItem(talviTie1);
            AddItem(talviTie2);
        }


        /// <summary>
        /// Piirtää luonnonsuojelualueen rajan - viiva ja siitä sen vieressä katkoviivoitus alueen sisäpuolella.
        /// </summary>
        /// <param name="record">Luokan recordi</param>
        /// <param name="index">Indexksi</param>
        /// <param name="settings">Luokan asetustiedot</param>
 
        void DrawLuonnonsuojeluviiva(Data.Generic.PolylineRecord record, int index, Graphics.ClassSettings settings)
        {
            var lista = new List<Point>();
            var pointArr = record.Objects[index].Points;
            // Luonnonsuojelualueen raja. Katkoviiva ja toinen viiva vieressä täytettynä tekstuurilla. 

            for (int j = 0; j < pointArr.Length; j++)
            {
                lista.Add(new Point(pointArr[j].X, pointArr[j].Y));
            }
            
            double relWidth = GetStrokeThicknessRelative(settings.StrokeThickness * 2 + 4);

            // Tehdään rinnakkainen viiva äsken määritellylle puolelle.
            var parLine = ParallerLine.CreateParallerPoints(lista, relWidth);

            // Itse viiva
            Path myPath = new Path();
            myPath.Stroke = settings.Stroke;
            myPath.StrokeThickness = GetStrokeThicknessRelative(settings.StrokeThickness);

            StreamGeometry geometry = new StreamGeometry();
            geometry.FillRule = FillRule.EvenOdd;
            
            // Viedään pisteet streamgeometrycontextiin.
            // Ensäimmäinen piste pitää kertoa eri metodilla kuin seuraavat.
            using (StreamGeometryContext ctx = geometry.Open())
            {
                ctx.BeginFigure(GetRelativePoint(lista.First()), false, false);

                foreach (var element in lista.Skip(1))
                {
                    ctx.LineTo(GetRelativePoint(element), true, false);
                }
            }

            // Määritellään viivan tyypit ja katkoviivoitus

            myPath.StrokeLineJoin = PenLineJoin.Round;
            myPath.StrokeEndLineCap = PenLineCap.Square;
            myPath.StrokeDashArray = Settings.dcKatkoviivat["Luonnonsuojelu19"];
            geometry.Freeze();
            myPath.Data = geometry;
            
            // Tehdään raidoitus viivan viereen. Tämä tulee tekstuurina, jos se on olemassa.
            Path myPath2 = new Path();
            myPath2.Stroke = Brushes.Red;
            myPath2.StrokeThickness = GetStrokeThicknessRelative(settings.StrokeThickness * 2 + 3);

            // Tehdään uusi geometria
            StreamGeometry geometry2 = new StreamGeometry();
            geometry2.FillRule = FillRule.EvenOdd;

            // Tehdään vierekkäisen viivan pisteiden perusteella taas geometria
            using (StreamGeometryContext ctx = geometry2.Open())
            {
                ctx.BeginFigure(GetRelativePoint(parLine.First()), false, false);

                foreach (var element in parLine.Skip(1))
                {
                    ctx.LineTo(GetRelativePoint(element), true, false);
                }
            }

            myPath2.StrokeLineJoin = PenLineJoin.Round;
            geometry2.Freeze();
            myPath2.Data = geometry2;
            // Tekstuuri
            myPath2.Stroke = settings.Fill;
                      
            AddItem(myPath);
            AddItem(myPath2);
           
        }

        /// <summary>
        /// Piirtää jyrkänteen: kiinteä viiva, josta lähtee "piikkejä". Tämä tehdään piirtämällä normaali viiva ja sen viereen
        /// katkoviiva piikeiksi.
        /// </summary>
        /// <param name="record">Luokan recordi</param>
        /// <param name="index">Indexksi</param>
        /// <param name="settings">Luokan asetustiedot</param>
      
        /// 
        void DrawJyrkanneviiva(Data.Generic.PolylineRecord record, int index, Graphics.ClassSettings settings)
        {

            var lista = new List<Point>();
            var pointArr = record.Objects[index].Points;

            for (int j = 0; j < pointArr.Length; j++)
            {
                lista.Add(new Point(pointArr[j].X, pointArr[j].Y));
            }

            double relWidth = GetStrokeThicknessRelative(settings.StrokeThickness * 2);

            // Viereen katkoviiva
            var parLine = ParallerLine.CreateParallerPoints(lista, relWidth);

            // Piirretään ensimmäisenä normaaliviiva
            Path myPath = new Path();
            myPath.Stroke = settings.Stroke;
            myPath.StrokeThickness = GetStrokeThicknessRelative(settings.StrokeThickness);

            StreamGeometry geometry = new StreamGeometry();

            using (StreamGeometryContext ctx = geometry.Open())
            {
                ctx.BeginFigure(GetRelativePoint(lista.First()), false, false);

                foreach (var element in lista.Skip(1))
                {
                    ctx.LineTo(GetRelativePoint(element), true, false);
                }
            }

            myPath.StrokeLineJoin = PenLineJoin.Round;
            myPath.StrokeEndLineCap = PenLineCap.Square;
            geometry.Freeze();
            myPath.Data = geometry;

            // Tämän jälkeen jyrkänneviiva

            Path myPath2 = new Path();

            StreamGeometry geometry2 = new StreamGeometry();

            using (StreamGeometryContext ctx = geometry2.Open())
            {
                ctx.BeginFigure(GetRelativePoint(parLine.First()), false, false);

                foreach (var element in parLine.Skip(1))
                {
                    ctx.LineTo(GetRelativePoint(element), true, false);
                }
            }

            myPath2.StrokeLineJoin = PenLineJoin.Round;
            geometry2.Freeze();
            myPath2.Data = geometry2;

            myPath2.Stroke = settings.Stroke;
            myPath2.StrokeThickness = GetStrokeThicknessRelative(settings.StrokeThickness * 3.6);

            myPath2.StrokeLineJoin = PenLineJoin.Round;
            myPath2.StrokeStartLineCap = PenLineCap.Flat;
            myPath2.StrokeEndLineCap = PenLineCap.Flat;
            myPath2.StrokeDashCap = PenLineCap.Flat;

            // Jyrkänteen "piikit"
            myPath2.StrokeDashArray = Settings.dcKatkoviivat["Jyrkanne21"];
            
            // Lisätään viivat canvakselle
            AddItem(myPath);
            AddItem(myPath2);
        }

        /// <summary>
        /// Piirtää luiskan. Viiva, josta lähtee normaalin suuntaan viivoja. Piirretään samanlailla kuin jyrkännekkin: normaali viiva, 
        /// katkoviiva normaalin suuntaisiksi viivoiksi
        /// </summary>
        /// <param name="record">Luokan recordi</param>
        /// <param name="index">Indexksi</param>
        /// <param name="settings">Luokan asetustiedot</param>
        /// 
        void DrawLuiskaviiva(Data.Generic.PolylineRecord record, int index, Graphics.ClassSettings settings)
        {
            // Otetaan pisteet talteen
            var lista = new List<Point>();
            var pointArr = record.Objects[index].Points;

            for (int j = 0; j < pointArr.Length; j++)
            {
                lista.Add(new Point(pointArr[j].X, pointArr[j].Y));
            }

            double relWidth = GetStrokeThicknessRelative(settings.StrokeThickness * 5);

            // Tehdään rinnakkainen viiva
            var parLine = ParallerLine.CreateParallerPoints(lista, relWidth);
            
            // Piirretään normaaliviiva
            Path myPath = new Path();
            myPath.Stroke = settings.Stroke;
            myPath.StrokeThickness = GetStrokeThicknessRelative(settings.StrokeThickness);

            StreamGeometry geometry = new StreamGeometry();

            using (StreamGeometryContext ctx = geometry.Open())
            {
                ctx.BeginFigure(GetRelativePoint(lista.First()), false, false);

                foreach (var element in lista.Skip(1))
                {
                    ctx.LineTo(GetRelativePoint(element), true, false);
                }
            }

            myPath.StrokeLineJoin = PenLineJoin.Round;
            myPath.StrokeEndLineCap = PenLineCap.Square;
            geometry.Freeze();
            myPath.Data = geometry;

            // Piirretään katkoviiva

            Path myPath2 = new Path();

            StreamGeometry geometry2 = new StreamGeometry();

            using (StreamGeometryContext ctx = geometry2.Open())
            {
                ctx.BeginFigure(GetRelativePoint(parLine.First()), false, false);

                foreach (var element in parLine.Skip(1))
                {
                    ctx.LineTo(GetRelativePoint(element), true, false);
                }
            }

            myPath2.StrokeLineJoin = PenLineJoin.Round;
            geometry2.Freeze();
            myPath2.Data = geometry2;

            //myPath2.Stroke = settings.Stroke;
            myPath2.Stroke = settings.Stroke;
            myPath2.StrokeThickness = GetStrokeThicknessRelative(settings.StrokeThickness * 5);

            myPath2.StrokeLineJoin = PenLineJoin.Round;
            myPath2.StrokeStartLineCap = PenLineCap.Flat;
            myPath2.StrokeEndLineCap = PenLineCap.Flat;
            myPath2.StrokeDashCap = PenLineCap.Flat;

            // Luiskan normaaliviivat
            myPath2.StrokeDashArray = Settings.dcKatkoviivat["Luiska22"];

            // Lisätään canvakselle
            AddItem(myPath);
            AddItem(myPath2);
        }

        /// <summary>
        /// Palauttaa onko reunaviiva ok. Jos ei ole reunaviiva, niin ei tehdä mitään.
        /// </summary>
        /// <param name="record">Recordi, josta tarkastetaan onko se reunaviiva.</param>
        /// <returns></returns>
        private bool CheckReunaviiva(Data.Generic.PolylineRecord record)
        {
            // Normaali reunaviiva
            if (record.Attribute.Luokka == 30211)
            {
                if (!drawReunaviivaSettings.ContainsKey(record.Attribute.KartoGlk))
                {
                    WriteLog("Reunaviiva uupuu: " + record.Attribute.KartoGlk);
                    return false;
                }
                // ei reunaviivaa?
                if (drawReunaviivaSettings[record.Attribute.KartoGlk] == null)
                {
                    return false;
                }
            }

            // Epämääräinen reunaviiva
            if (record.Attribute.Luokka == 30212)
            {
                if (!drawEpamaarainenReunaviivaSettings.ContainsKey(record.Attribute.KartoGlk))
                {
                    WriteLog("Epämäärinen reunaviiva uupuu: " + record.Attribute.KartoGlk);
                    return false;
                }
                // ei reunaviivaa?
                if (drawEpamaarainenReunaviivaSettings[record.Attribute.KartoGlk] == null)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Palauttaa oikean katkoviivoituksen recordille tai nullin.
        /// </summary>
        /// <param name="settings">Yleiset asetukset</param>
        /// <param name="record">Piirrettävä recordi</param>
        /// <returns>Katkoviivoitus</returns>
        private DoubleCollection GetStrokeDashArray(Graphics.ClassSettings settings, Data.Generic.PolylineRecord record)
        {

            if (settings.Shape.Type == 6 && record.Attribute.Versuh == 0)  
            {
                // Polku
                return Settings.dcKatkoviivat["Polku6"];
            } else if (settings.Shape.Type == 7 && record.Attribute.Versuh == 0) 
            {
                // Polku
                return Settings.dcKatkoviivat["Polku7"];
            } else if (settings.Shape.Type == 31 && settings.VerSuh == Graphics.VerSuh.Erikois) 
            {
                // Purojuonne. Eli katkoviiva
                if (record.Attribute.Versuh == -1)
                {
                    return Settings.dcKatkoviivat["Puro31"];
                }
            } else if (settings.Shape.Type == 20)  // kuntaraja
            {
                return Settings.dcKatkoviivat["Kuntaraja20"];
            } else if (record.Attribute.Luokka == 54100)
            {
                // Syvyyskäyrät menevät näin:
                // alle 3m pisteviiva
                // alle 6m viiva viiva
                // alle 10m 2*6 viivan pituus
                // yli 10m suora viiva
                if (record.Attribute.KorArv < 3)
                {
                    return Settings.dcKatkoviivat["Syvyyskayra1"];
                }
                else if (record.Attribute.KorArv < 6)
                {
                    return Settings.dcKatkoviivat["Syvyyskayra3"];
                }
                else if (record.Attribute.KorArv < 10)
                {
                    return Settings.dcKatkoviivat["Syvyyskayra6"];
                }
            }
            //Ei katkoviivaa, pelkkä suora viiva
            return null;
        }

        /// <summary>
        /// Piirtää polyline/viivakokoelman
        /// </summary>
        /// <param name="record">Piirrettävä polylinen recordi</param>

        void DrawPolyline(Data.Generic.PolylineRecord record, int zIndex)
        {
        
            var settings = GetSettings(record.Attribute.Luokka, record.Attribute.Ryhma);

            // ei näytetä, jos maan sisässä. tiet poikkeus
            if (record.Attribute.Versuh == -1 && SaakoPiirtaaPinnanAlle(record, settings) == false)
            {
                if (settings.VerSuh != Graphics.VerSuh.EiMitaan)
                {
                    WriteLog("Line Versuh -1 detected. Not shown: " + record.Attribute.Versuh + " record: " + record.Id + "  type:" + record.Attribute.Luokka);
                }
                return;
            }

            // Tarkastetaan onko jokin outo VerSuh
            if (record.Attribute.Versuh != 0 && settings.VerSuh == 0 && record.Attribute.Ryhma != 25 && record.Attribute.Versuh > -2)
            {
                WriteLog("Line Versuh detected: " + record.Attribute.Versuh + " record: " + record.Id + "  type:" + record.Attribute.Luokka);
            }

            // Piirretäänkö luokka?
            if (settings.StrokeThickness == -1)
            {
                return;
            }

            // Käydään viivan objektit läpi
            
            for (int i = 0; i < record.Objects.Length; i++)
            {

                int pointsNum = record.Objects[i].Points.Length;
 
                // Tunneli
                if (record.Attribute.Versuh == -11)
                {
                    DrawTunneli(record, i, settings);
                }
                else if (settings.Shape.Type == 5)
                {
                    // Talvitie katkoviiva
                    DrawTalvitie(record, i, settings);
                }
                else if (settings.Shape.Type == 19)
                {
                    // Luonnonsuojelualueen raja
                    DrawLuonnonsuojeluviiva(record, i, settings);
                }
                else if (settings.Shape.Type == 21)
                {
                    // Jyrkänne
                    DrawJyrkanneviiva(record, i, settings);
                }
                else if (settings.Shape.Type == 22)
                {
                    // Luiska. Vastaa vähän jyrkännettä.
                    DrawLuiskaviiva(record, i, settings);
                }
                else
                {
                    // Tämä suoritetaan vain luokille 30211 ja 30212
                    if (CheckReunaviiva(record) == false)
                    {
                        continue;
                    }

                    // Tee pääviiva
                    Path myPath = new Path();

                    myPath.Stroke = settings.Stroke;
                    myPath.StrokeThickness = GetStrokeThicknessRelative(settings.StrokeThickness);

                    // Käytetään StreamGeometryä, koska nopeampi ja ei tarvita segmenttejä.

                    StreamGeometry geometry = new StreamGeometry();
                    geometry.FillRule = FillRule.EvenOdd;

                    var points = record.Objects[i].Points;

                    using (StreamGeometryContext ctx = geometry.Open())
                    {
                        ctx.BeginFigure(GetRelativePoint(record.Objects[i].Points[0].X, record.Objects[i].Points[0].Y), false, false);
                        for (int j = 1; j < points.Length; j++)
                        {
                            ctx.LineTo(GetRelativePoint(record.Objects[i].Points[j].X, record.Objects[i].Points[j].Y), true, false);
                        }
                    }

                    myPath.StrokeLineJoin = PenLineJoin.Round;
                    myPath.StrokeStartLineCap = PenLineCap.Round;

                    geometry.Freeze();
                    myPath.Data = geometry;

                    // Pääshapet
                    // Tie
                    if (settings.Shape.Type == 8)
                    {
                        myPath.StrokeThickness = GetStrokeThicknessRelative(settings.StrokeThickness + 2);

                         // kakkosviiva
                        if( zIndex>settings.ZIndex)
                        {
                            if (record.Attribute.Versuh == 0)
                            {
                                // Normaali tie
                                var path2 = new Path();
                                path2.Stroke = settings.Fill;
                                path2.StrokeThickness = GetStrokeThicknessRelative(settings.StrokeThickness);
                                path2.StrokeLineJoin = PenLineJoin.Round;
                                path2.StrokeStartLineCap = PenLineCap.Round;
                                path2.Data = geometry;

                                AddItem(path2);

                            }
                            else if (record.Attribute.Versuh == -1)
                            {
                                // Sillan alunen, piirretään viiva silti, koska muuten tulee hassuja aukkoja.
                                var path2 = new Path();
                                path2.Stroke = settings.Fill;
                                path2.StrokeThickness = GetStrokeThicknessRelative(settings.StrokeThickness);
                                path2.StrokeLineJoin = PenLineJoin.Round;
                                path2.StrokeStartLineCap = PenLineCap.Round;
                                path2.Data = geometry;


                                AddItem(path2);

                            }
                            if (record.Attribute.Versuh >= 1)
                            {
                                // silta, tummemmalla
                                var path2 = new Path();
                                path2.Stroke = Settings.scbTiesilta;

                                path2.StrokeThickness = GetStrokeThicknessRelative(settings.StrokeThickness - 2);
                                path2.StrokeLineJoin = PenLineJoin.Round;
                                path2.StrokeStartLineCap = PenLineCap.Round;
                                path2.Data = geometry;

                                path2.StrokeLineJoin = PenLineJoin.Miter;
                                path2.StrokeStartLineCap = PenLineCap.Square;
                                path2.StrokeEndLineCap = PenLineCap.Square;

                                // sillan reunat

                                myPath.StrokeLineJoin = PenLineJoin.Miter;
                                myPath.StrokeStartLineCap = PenLineCap.Square;
                                myPath.StrokeEndLineCap = PenLineCap.Square;

                                AddItem(myPath);
                                AddItem(path2);

                            }
                            return;
                        } 
                        
                        if (record.Attribute.Versuh >= 1)
                        {
                            return;
                        }  else if (record.Attribute.Versuh == -11)
                        {
                            // tunneli. Ei pitäisi koskaan tulla tänne
                            WriteLog("Versuh tunneli shape 8, luokka:" + record.Attribute.Luokka + " versuh: " + record.Attribute.Versuh);
                        }
                        else if (record.Attribute.Versuh != 0 && record.Attribute.Versuh != -1)
                        {
                            // Tuntematon versuh
                            WriteLog("TuntematonVersuh shape 8, luokka:" + record.Attribute.Luokka + " versuh: " + record.Attribute.Versuh);
                            throw new Exception("Shape 8, versuh:" + record.Attribute.Versuh);
                        }  
                    }
                    // rautatie, kaksivärinen
                    else if (settings.Shape.Type == 36)
                    {
                        if (record.Attribute.Versuh == 1)
                        {
                            // Rautatiesilta. tämä on yksivärinen

                            myPath.StrokeLineJoin = PenLineJoin.Miter;
                            myPath.StrokeStartLineCap = PenLineCap.Flat;
                            myPath.StrokeEndLineCap = PenLineCap.Flat;

                            myPath.Stroke = Settings.scbRautatieSilta;
                            myPath.Fill = Settings.scbRautatieSilta;
                        }
                        else if (record.Attribute.Versuh == -11)
                        {
                            // Tunneli pitäisi tulla muuta kautta, ei tulla koskaan tänne
                            var str = "Versuh tunneli shape 36 rautatie, luokka:" + record.Attribute.Luokka + " versuh: " + record.Attribute.Versuh;
                            WriteLog(str);
                        }
                        else if (record.Attribute.Versuh != 0 && record.Attribute.Versuh != -1)
                        {
                            var str = "Versuh muu kuin 0,1,-1: shape 36 rautatie, luokka:" + record.Attribute.Luokka + " versuh: " + record.Attribute.Versuh;
                            WriteLog(str);
                            throw new Exception(str);
                        }

                        if (zIndex > settings.ZIndex && record.Attribute.Versuh<1)
                        {
                            if (record.Attribute.Versuh == 0)
                            {
                                // Normaali rautatie. Luodaan tässä katkoviiva, että saadaan kaksivärinen katkoviiva

                                var path2 = new Path();
                                path2.Stroke = settings.Fill;
                                path2.StrokeThickness = GetStrokeThicknessRelative(settings.StrokeThickness - 2);
                                path2.StrokeLineJoin = PenLineJoin.Miter;
                                path2.StrokeStartLineCap = PenLineCap.Flat;
                                path2.Data = geometry;

                                path2.StrokeDashArray = Settings.dcKatkoviivat["Rautatie36"];
                                AddItem(path2);

                            }
                            else if (record.Attribute.Versuh == -1)
                            {
                                // Sillan alusta. Piirretään, ettei tule hassuja pätkiä
                                // Eli piirretään normaali kaksivärinen katkoviiva
                                var path2 = new Path();
                                path2.Stroke = settings.Fill;
                                path2.StrokeThickness = GetStrokeThicknessRelative(settings.StrokeThickness - 2);
                                path2.StrokeLineJoin = PenLineJoin.Miter;
                                path2.StrokeStartLineCap = PenLineCap.Flat;
                                path2.Data = geometry;

                                path2.StrokeDashArray = Settings.dcKatkoviivat["Rautatie36"];
                                AddItem(path2);
                            }
                            return;
                        }
                       
                    }
                    else if (settings.Shape.Type == 9)
                    {
                        // Aita. Viiva, jossa pisteitä mukana. Piirretään pisteet tässä

                        var path2 = new Path();
                        path2.Stroke = settings.Fill;
                        path2.StrokeThickness = GetStrokeThicknessRelative(settings.StrokeThickness + 3);
                        path2.StrokeLineJoin = PenLineJoin.Round;
                        path2.StrokeStartLineCap = PenLineCap.Round;
                        path2.StrokeDashCap = PenLineCap.Round;
                        path2.StrokeEndLineCap = PenLineCap.Round;
                        path2.Data = geometry;

                        // Pisteet tulevat katkoviivalla
                        path2.StrokeDashArray = Settings.dcKatkoviivat["Aita9"];
                        AddItem(path2);
                    }
                    else if (settings.Shape.Type == 32)
                    {
                        // Suojänne.  "aita ilman viivaa". Pisteitä tiheästi
                        myPath.StrokeDashCap = PenLineCap.Round;
                        myPath.StrokeDashArray = Settings.dcKatkoviivat["Suojanne32"];
                    }
                    else if (settings.Shape.Type == 33)  
                    {
                        // koski. valkoisia palloja viivana
                        myPath.StrokeDashCap = PenLineCap.Round;
                        myPath.StrokeDashArray = Settings.dcKatkoviivat["Koski33"];
                    }
                    else if (settings.Shape.Type == 34)
                    {
                        // Puurivi, vihreitä palloja harvempana viivana
                        myPath.StrokeDashCap = PenLineCap.Round;
                        myPath.StrokeDashArray = Settings.dcKatkoviivat["Puurivi34"];
                    }
                    else if (settings.Shape.Type == 38)
                    {
                        // Pensasaita, tiheämmin vihreitä palloja
                        myPath.StrokeDashCap = PenLineCap.Round;
                        myPath.StrokeDashArray = Settings.dcKatkoviivat["Pensasaita38"];
                    }
                    else if (settings.Shape.Type == 35)
                    {
                        // Venereitti, katkoviiva vedessä
                        myPath.StrokeStartLineCap = PenLineCap.Flat;
                        myPath.StrokeDashCap = PenLineCap.Flat;
                        myPath.StrokeDashArray = Settings.dcKatkoviivat["Venereitti35"];
                    }
                    else if (settings.Shape.Type == 39)
                    {
                        // Suoja-alue: katkoviiva
                        myPath.StrokeDashArray = Settings.dcKatkoviivat["SuojaAlue"];
                    }


                    // Korkeuskäyrä
                    if (record.Attribute.Luokka == 52100)
                    {
                        if (record.Attribute.Luokka == 52100 && ((record.Attribute.KorArv % 20) == 0))
                        {
                            // korkeuskäyrä 20m välein on paksumpi
                            myPath.StrokeThickness += GetStrokeThicknessRelative(settings.StrokeThickness);
                        }
                        if (record.Attribute.Luokka == 52100 && ((record.Attribute.KorArv * 2 % 5) == 0) && (record.Attribute.KorArv % 5 != 0))
                        {
                            // korkeuskäyrä 2,5m välein, tämä on katkoviivalla
                            myPath.StrokeDashArray = Settings.dcKatkoviivat["Korkeuskayra25"];
                        }

                    }

                    // Jos ei ole katkoviivoitusta, käydään katsoa että tarvitseeko olla
                    if (myPath.StrokeDashArray.Count == 0)
                    {
                        myPath.StrokeDashArray = GetStrokeDashArray(settings, record);
                    }

                    // Reunaviiva. Saadaan väri erillisestä tietorakenteesta
                    if (record.Attribute.Luokka == 30211)
                    {
                        myPath.Stroke = drawReunaviivaSettings[record.Attribute.KartoGlk];
                        myPath.StrokeThickness = GetStrokeThicknessRelative(settings.StrokeThickness);
                    }

                    // Epämääräinen reunaviiva. Pistemäinen, saadaan väri erillisestä tietorakenteesta
                    if (record.Attribute.Luokka == 30212)
                    {
                        myPath.Stroke = drawEpamaarainenReunaviivaSettings[record.Attribute.KartoGlk];
                        myPath.StrokeThickness = GetStrokeThicknessRelative(settings.StrokeThickness);
                        myPath.StrokeDashArray = Settings.dcKatkoviivat["EpamaarainenReunaviiva"];
                    }

                    // Pinnan alla kulkevat putket: osa on katkoviivalla, osa ei. Katkoviivallisile ei saa muuttaa zIndeksiä
                    if ((record.Attribute.Luokka == 22311 || record.Attribute.Luokka == 22312 || record.Attribute.Luokka == 26111) && record.Attribute.Versuh == -1)
                    {
                        myPath.StrokeDashArray = Settings.dcKatkoviivat["PinnanAllaJohto"];
                    }
                    else
                    {
                     
                    }
                    AddItem(myPath);
                }
            }
        }
    }
}
