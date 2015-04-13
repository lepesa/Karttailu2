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

namespace Karttailu2.Graphics.GenericWpf
{
    public partial class MapVisualiser
    {
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
        /// <param name="canv">Canvas</param>
        /// <param name="record">Luokan recordi</param>
        /// <param name="index">Indexksi</param>
        /// <param name="settings">Luokan asetustiedot</param>
        /// <param name="guidStart">guid alku</param>
        void DrawTunneli(Canvas canv, Data.Generic.PolylineRecord record, int index, Graphics.ClassSettings settings, string guidStart)
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

            // Asetaan viivoille tunnisteet
            tunneliViiva1.Uid = guidStart + "-1";
            graphicsElements[tunneliViiva1.Uid] = record;

            tunneliViiva2.Uid = guidStart + "-2";
            graphicsElements[tunneliViiva2.Uid] = record;

            tunneliViiva1.StrokeThickness = GetStrokeThicknessRelative(settings.Height);
            tunneliViiva2.StrokeThickness = GetStrokeThicknessRelative(settings.Height);

            tunneliViiva1.StrokeDashArray = Settings.dcKatkoviivat["Tunneli"];
            tunneliViiva2.StrokeDashArray = Settings.dcKatkoviivat["Tunneli"];

            // Lisätään canvakselle ja asetetaan zIndex.
            canv.Children.Add(tunneliViiva1);
            canv.Children.Add(tunneliViiva2);

            Canvas.SetZIndex(tunneliViiva1, settings.ZIndex);
            Canvas.SetZIndex(tunneliViiva2, settings.ZIndex);
        }

        /// <summary>
        /// Piirtää talvitien (kaksiviivaa vierekkäin, katkoviivalla).
        /// </summary>
        /// <param name="canv">Canvas</param>
        /// <param name="record">Luokan recordi</param>
        /// <param name="index">Indexksi</param>
        /// <param name="settings">Luokan asetustiedot</param>
        /// <param name="guidStart">guid alku</param>

        void DrawTalvitie(Canvas canv, Data.Generic.PolylineRecord record, int index, Graphics.ClassSettings settings, string guidStart)
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

            // Asetaan viivoille tunnisteet
            talviTie1.Uid = guidStart + "-1";
            graphicsElements[talviTie1.Uid] = record;

            talviTie2.Uid = guidStart + "-2";
            graphicsElements[talviTie2.Uid] = record;

            // Lisätään ne canvakselle ja asetetaan zindex
            canv.Children.Add(talviTie1);
            canv.Children.Add(talviTie2);

            // Versuh voi olla myös pinnan alla/päällä (tien ali, joen yli)
            if (record.Attribute.Versuh != 0)
            {
                Canvas.SetZIndex(talviTie1, GetVerSuhZIndex(record.Attribute.Versuh, settings.VerSuh, settings.ZIndex));
                Canvas.SetZIndex(talviTie2, GetVerSuhZIndex(record.Attribute.Versuh, settings.VerSuh, settings.ZIndex));
            }
            else
            {
                Canvas.SetZIndex(talviTie1, settings.ZIndex);
                Canvas.SetZIndex(talviTie2, settings.ZIndex);
            }
        }


        /// <summary>
        /// Piirtää luonnonsuojelualueen rajan - viiva ja siitä sen vieressä katkoviivoitus alueen sisäpuolella.
        /// </summary>
        /// <param name="canv">Canvas</param>
        /// <param name="record">Luokan recordi</param>
        /// <param name="index">Indexksi</param>
        /// <param name="settings">Luokan asetustiedot</param>
        /// <param name="guidStart">guid alku</param>

        void DrawLuonnonsuojeluviiva(Canvas canv, Data.Generic.PolylineRecord record, int index, Graphics.ClassSettings settings, string guidStart)
        {
            var lista = new List<Point>();
            var pointArr = record.Objects[index].Points;
            // Luonnonsuojelualueen raja. Katkoviiva ja toinen viiva vieressä täytettynä tekstuurilla. Halutaan että on inakin kaksi pistettä.

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

            // Asetetaan guid ja lisätään katkoviiva canvasiin
            myPath.Uid = guidStart + "-1";
            graphicsElements[myPath.Uid] = record;

            myPath2.Uid = guidStart + "-2";
            graphicsElements[myPath2.Uid] = record;


            canv.Children.Add(myPath);
            canv.Children.Add(myPath2);
            Canvas.SetZIndex(myPath2, settings.ZIndex - 1);
            Canvas.SetZIndex(myPath, settings.ZIndex);
        }

        /// <summary>
        /// Piirtää jyrkänteen: kiinteä viiva, josta lähtee "piikkejä". Tämä tehdään piirtämällä normaali viiva ja sen viereen
        /// katkoviiva piikeiksi.
        /// </summary>
        /// <param name="canv">Canvas</param>
        /// <param name="record">Luokan recordi</param>
        /// <param name="index">Indexksi</param>
        /// <param name="settings">Luokan asetustiedot</param>
        /// <param name="guidStart">guid alku</param>
        /// 
        void DrawJyrkanneviiva(Canvas canv, Data.Generic.PolylineRecord record, int index, Graphics.ClassSettings settings, string guidStart)
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

            // Piirretään ekanan normaaliviiva
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

            //myPath2.Stroke = settings.Stroke;
            myPath2.Stroke = settings.Stroke;
            myPath2.StrokeThickness = GetStrokeThicknessRelative(settings.StrokeThickness * 3.6);

            myPath2.StrokeLineJoin = PenLineJoin.Round;
            myPath2.StrokeStartLineCap = PenLineCap.Flat;
            myPath2.StrokeEndLineCap = PenLineCap.Flat;
            myPath2.StrokeDashCap = PenLineCap.Flat;

            // Jyrkänteen "piikit"
            myPath2.StrokeDashArray = Settings.dcKatkoviivat["Jyrkanne21"];

            // Lisätään uid:t ja viivat canvakselle.

            myPath.Uid = guidStart + "-1";
            graphicsElements[myPath.Uid] = record;


            myPath2.Uid = guidStart + "-2";
            graphicsElements[myPath2.Uid] = record;

            canv.Children.Add(myPath);
            canv.Children.Add(myPath2);
            Canvas.SetZIndex(myPath2, settings.ZIndex);
            Canvas.SetZIndex(myPath, settings.ZIndex);
        }

        /// <summary>
        /// Piirtää luiskan. Viiva, josta lähtee normaalin suuntaan viivoja. Piirretään samanlailla kuin jyrkännekkin: normaali viiva ka 
        /// katkoviiva normaalin suuntaisiksi viivoiksi
        /// </summary>
        /// <param name="canv">Canvas</param>
        /// <param name="record">Luokan recordi</param>
        /// <param name="index">Indexksi</param>
        /// <param name="settings">Luokan asetustiedot</param>
        /// <param name="guidStart">guid alku</param>
        /// 
        void DrawLuiskaviiva(Canvas canv, Data.Generic.PolylineRecord record, int index, Graphics.ClassSettings settings, string guidStart)
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

            // Tehdään guid ja lisätään viivat canvakselle

            myPath.Uid = guidStart + "-1";
            graphicsElements[myPath.Uid] = record;

            myPath2.Uid = guidStart + "-2";
            graphicsElements[myPath2.Uid] = record;

            canv.Children.Add(myPath);
            canv.Children.Add(myPath2);
            Canvas.SetZIndex(myPath2, settings.ZIndex);
            Canvas.SetZIndex(myPath, settings.ZIndex);
        }

        /// <summary>
        /// Palauttaa onko reunaviiva ok. Jos ei ole reunaviiva, niin ei tehdä mitään.
        /// </summary>
        /// <param name="record">REcordi, josta tarkastetaan onko se reunaviiva.</param>
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

            if (settings.Shape.Type == 6 && record.Attribute.Versuh == 0)  // polku
            {
                return Settings.dcKatkoviivat["Polku6"];
            }
            if (settings.Shape.Type == 7 && record.Attribute.Versuh == 0)  // polku
            {
                return Settings.dcKatkoviivat["Polku7"];
            }

            // Purojuonne. Eli katkoviiva
            if (settings.Shape.Type == 31 && settings.VerSuh == Graphics.VerSuh.Erikois)
            {
                if (record.Attribute.Versuh == -1)
                {
                    return Settings.dcKatkoviivat["Puro31"];
                }
            }
            if (settings.Shape.Type == 20)  // kuntaraja
            {
                return Settings.dcKatkoviivat["Kuntaraja20"];
            }

            if (record.Attribute.Luokka == 54100)
            {
                // alle 3 pisteviiva
                // alle 6 viiva viiva
                // alle 10 2*6 viivan pituus
                // yli 10 suora viiva
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
                // muuten suora viiva
            }
            return null;
        }

        /// <summary>
        /// Piirtää polyline/viivakokoelman
        /// </summary>
        /// <param name="cMap">Piirtoalueen / canvaksen tieto</param>
        /// <param name="record">Piirrettävä polylinen recordi</param>

        void DrawPolyline(Canvas canv, Data.Generic.PolylineRecord record)
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

                int pointsNum = record.Objects[i].Points.Length;
                string guid = "poly-" + record.Id + "-" + i; 
                    //GetPolylineGuid(record.Id, i);


                // Tunneli
                if (record.Attribute.Versuh == -11)
                {
                    DrawTunneli(canv, record, i, settings, guid);
                }
                else if (settings.Shape.Type == 5)
                {
                    // Talvitie katkoviiva
                    DrawTalvitie(canv, record, i, settings, guid);
                }
                else if (settings.Shape.Type == 19)
                {
                    // Luonnonsuojelualueen raja
                    DrawLuonnonsuojeluviiva(canv, record, i, settings, guid);
                }
                else if (settings.Shape.Type == 21)
                {
                    // Jyrkänne
                    DrawJyrkanneviiva(canv, record, i, settings, guid);
                }
                else if (settings.Shape.Type == 22)
                {
                    // Luiska. Vastaa vähän jyrkännettä.
                    DrawLuiskaviiva(canv, record, i, settings, guid);

                }
                else
                {
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

                    if (settings.Shape.Type == 8)
                    {
                        myPath.StrokeThickness = GetStrokeThicknessRelative(settings.StrokeThickness + 2);

                        // Tie, kaksivärinen
                        if (record.Attribute.Versuh == 0)
                        {
                            // Normaali tie
                            var path2 = new Path();
                            path2.Stroke = settings.Fill;
                            path2.StrokeThickness = GetStrokeThicknessRelative(settings.StrokeThickness);
                            path2.StrokeLineJoin = PenLineJoin.Round;
                            path2.StrokeStartLineCap = PenLineCap.Round;
                            path2.Data = geometry;

                            path2.Uid = guid + "-2";
                            graphicsElements[path2.Uid] = record;

                            canv.Children.Add(path2);
                            Canvas.SetZIndex(path2, settings.ZIndex + 1);
                        }
                        else if (record.Attribute.Versuh >= 1)
                        {
                            // Pelkkä silta, on yksivärinen
                            myPath.StrokeLineJoin = PenLineJoin.Miter;
                            myPath.StrokeStartLineCap = PenLineCap.Square;
                            myPath.StrokeEndLineCap = PenLineCap.Square;

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

                            path2.Uid = guid + "-2";
                            graphicsElements[path2.Uid] = record;

                            canv.Children.Add(path2);
                            Canvas.SetZIndex(path2, GetVerSuhZIndex(record.Attribute.Versuh, settings.VerSuh, settings.ZIndex) + 1);
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
                            // Normaali rautatie. Luodaan tässä katkoviiva, että saadaan kaksivärinen katkoviiva

                            var path2 = new Path();
                            path2.Stroke = settings.Fill;
                            path2.StrokeThickness = GetStrokeThicknessRelative(settings.StrokeThickness - 2);
                            path2.StrokeLineJoin = PenLineJoin.Miter;
                            path2.StrokeStartLineCap = PenLineCap.Flat;
                            path2.Data = geometry;

                            path2.Uid = guid + "-2";
                            graphicsElements[path2.Uid] = record;

                            path2.StrokeDashArray = Settings.dcKatkoviivat["Rautatie36"];

                            canv.Children.Add(path2);
                            Canvas.SetZIndex(path2, settings.ZIndex + 1);
                        }
                        else if (record.Attribute.Versuh == 1)
                        {
                            // Rautatiesilta. tämä on yksivärinen

                            myPath.StrokeLineJoin = PenLineJoin.Miter;
                            myPath.StrokeStartLineCap = PenLineCap.Square;
                            myPath.StrokeEndLineCap = PenLineCap.Square;

                            myPath.Stroke = Settings.scbRautatieSilta;
                            myPath.Fill = Settings.scbRautatieSilta;
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

                            path2.Uid = guid + "-2";
                            graphicsElements[path2.Uid] = record;

                            path2.StrokeDashArray = Settings.dcKatkoviivat["Rautatie36"];
                            canv.Children.Add(path2);

                            Canvas.SetZIndex(path2, GetVerSuhZIndex(record.Attribute.Versuh, settings.VerSuh, settings.ZIndex) + 1);
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

                        path2.Uid = guid + "-2";
                        graphicsElements[path2.Uid] = record;

                        canv.Children.Add(path2);
                        Canvas.SetZIndex(path2, settings.ZIndex + 1);
                    }
                    else if (settings.Shape.Type == 32)
                    {
                        // Suojänne.  "aita ilman viivaa". Pisteitä tiheästi

                        myPath.StrokeDashCap = PenLineCap.Round;
                        myPath.StrokeDashArray = Settings.dcKatkoviivat["Suojanne32"];
                    }
                    else if (settings.Shape.Type == 33)  // koski. valkoisia palloja viivana
                    {
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

                    var zIndex = settings.ZIndex;

                    // Reunaviiva. Saadaan väri erillisestä tietorakenteesta
                    if (record.Attribute.Luokka == 30211)
                    {
                        myPath.Stroke = drawReunaviivaSettings[record.Attribute.KartoGlk];
                        myPath.StrokeThickness = GetStrokeThicknessRelative(settings.StrokeThickness);
                        zIndex = drawClasSettings[record.Attribute.KartoGlk].ZIndex + 2;
                    }

                    // Epämääräinen reunaviiva. Pistemäinen, saadaan väri erillisestä tietorakenteesta
                    if (record.Attribute.Luokka == 30212)
                    {
                        myPath.Stroke = drawEpamaarainenReunaviivaSettings[record.Attribute.KartoGlk];
                        myPath.StrokeThickness = GetStrokeThicknessRelative(settings.StrokeThickness);
                        zIndex = drawClasSettings[record.Attribute.KartoGlk].ZIndex + 2;
                        myPath.StrokeDashArray = Settings.dcKatkoviivat["EpamaarainenReunaviiva"];
                    }


                    // Pinnan alla kulkevat putket: osa on katkoviivalla, osa ei. Katkoviivallisile ei saa muuttaa zIndeksiä
                    if ((record.Attribute.Luokka == 22311 || record.Attribute.Luokka == 22312 || record.Attribute.Luokka == 26111) && record.Attribute.Versuh == -1)
                    {
                        myPath.StrokeDashArray = Settings.dcKatkoviivat["PinnanAllaJohto"];
                    }
                    else
                    {
                        // Asetetaan ZIndex
                        if (record.Attribute.Versuh != 0)
                        {
                            Canvas.SetZIndex(myPath, GetVerSuhZIndex(record.Attribute.Versuh, settings.VerSuh, zIndex));
                        }
                    }

                    // Lisätään pääviiva
                    myPath.Uid = guid;
                    graphicsElements[myPath.Uid] = record;
                    canv.Children.Add(myPath);
                    Canvas.SetZIndex(myPath, zIndex);
                }
            }
        }

    }
}
