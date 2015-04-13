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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
namespace Karttailu2.Graphics.GenericWpf
{
    public partial class MapVisualiser
    {
        /// <summary>
        /// Piirtää Piste -objektin ja lisää sen Canvakselle.
        /// </summary>
        /// <param name="cmap">Canvas</param>
        /// <param name="record">Recordi, joka sisältää pisteen</param>
        private void DrawPoint(Canvas canv, Data.Generic.PointRecord record)
        {
            // ei näytetä, jos maan sisässä
            if (record.Attribute.Versuh == -1)
            {
                return;
            }

            var settings = GetSettings(record.Attribute.Luokka, record.Attribute.Ryhma);

            // Ei näytetä, jos viivan paksuus on -1
            if (settings.StrokeThickness == -1)
            {
                return;
            }

            string guidStart = "Point-";

            // Tulostetaan viesti, jos VerSuh on ei-tuettu.
            if ((record.Attribute.Versuh < 0 && settings.VerSuh == 0) || (record.Attribute.Versuh < 0 || record.Attribute.Versuh > 1))
            {
                WriteLog("Point Versuh detected: " + record.Attribute.Versuh + " record: " + record.Id + "  type:" + record.Attribute.Luokka);
            }

            // Tekstuuri/objekti, joka piirretään 1:1 neliönä

            if (settings.Shape.Type == 1)
            {
                var rect = new Rectangle();

                rect.StrokeThickness = GetStrokeThicknessRelative(settings.StrokeThickness);
                rect.Width = settings.Width;
                rect.Height = settings.Height;

                // Jos on annettu tekstuuri, käyteään sitä
                if (settings.Fill != null)
                {
                    rect.Fill = settings.Fill;
                    rect.Fill.Freeze();
                }

                // Käännetään kuvaa
                // Suunta on 1/10 000 radiaani
                if (record.Attribute.Suunta != 0)
                {
                    double angle = -1 * (Convert.ToDouble(record.Attribute.Suunta) * 180) / Math.PI / 10000;

                    var rotateTransform1 = new RotateTransform(angle);

                    // Käännetään keskipisteen mukaan
                    rotateTransform1.CenterX = rect.Width / 2;
                    rotateTransform1.CenterY = rect.Height / 2;

                    rect.LayoutTransform = rotateTransform1;
                    rect.LayoutTransform.Freeze();
                }

                // Otetaan uusi koko selville
                rect.Measure(new Size(double.MaxValue, double.MaxValue));
                var rectSize = rect.DesiredSize;
                
                rect.Uid = String.Format("{0}-{1}-0", guidStart, record.Id);
                graphicsElements[rect.Uid] = record;
                
                // Siirretään objekti oikeen kohtaan
                var p = GetRelativePoint(record.Point.X, record.Point.Y);

                Canvas.SetLeft(rect, p.X - rectSize.Width / 2);
                Canvas.SetTop(rect, p.Y - rectSize.Height / 2);
                Canvas.SetZIndex(rect, settings.ZIndex);
                canv.Children.Add(rect);
            }
            // 30 on viettoviiva. Tässä rotaation alkupiste ei ole keskipiste vaan kulmassa. Muuten sama kuin objekti
            else if (settings.Shape.Type == 30)
            {
                // Piirretään neliö
                var rect = new Rectangle();

                //rect.Stroke = settings.Stroke;
                rect.StrokeThickness = GetStrokeThicknessRelative(settings.StrokeThickness);
                rect.Width = settings.Width;
                rect.Height = settings.Height;

                // Käytetään tekstuuria
                if (settings.Fill != null)
                {
                    rect.Fill = settings.Fill;
                    rect.Fill.Freeze();
                }
                
                rect.Measure(new Size(double.MaxValue, double.MaxValue));
                var rectSize = rect.DesiredSize;

                // Suunta on 1/10 000 radiaani
                //double angle = -1 * (Convert.ToDouble(record.Attribute.Suunta  ) * 180) / Math.PI / 10000;
                if (record.Attribute.Suunta != 0)
                {
                    double angle = 360 - (Convert.ToDouble(record.Attribute.Suunta) * 180 / Math.PI) / 10000;
                    RotateTransform rotateTransform1 = new RotateTransform(angle);

                    // Asetetaan keskipiste muualle
                    rect.RenderTransformOrigin = new Point(0, 1);

                    // Siirretään rotatoitua objektia
                    var trans = new TranslateTransform(0, -rect.Height);
                    TransformGroup myTransformGroup = new TransformGroup();

                    myTransformGroup.Children.Add(rotateTransform1);
                    myTransformGroup.Children.Add(trans);

                    // Tehdään transformaatiot
                    rect.RenderTransform = myTransformGroup;
                    rect.RenderTransform.Freeze();
                }
                rect.Uid = String.Format("{0}-{1}-0", guidStart, record.Id);

                graphicsElements[rect.Uid] = record;

                // Lisätään objekti ruudulle ja asetetaan paikka
                canv.Children.Add(rect);

                var p = GetRelativePoint(record.Point.X, record.Point.Y);
                Canvas.SetLeft(rect, p.X);
                Canvas.SetTop(rect, p.Y);
                Canvas.SetZIndex(rect, settings.ZIndex);
            }
            else
            {
                // Ei ollut shape, katsotaan onko teksti vai ellipsi.
                if (record.Attribute.Teksti != null && record.Attribute.Teksti.Length > 0)
                {
                    string fonttiOhje = record.Attribute.KartoGlk.ToString();

                    string kirjaisinLeikkaustunnus = "";            // Kirjaisinleikkaustunnus. 1- 7
                    int kirjaisinKoko = 0;                          // 1/72 tuumaa. 1-99
                    string harvennus = "";                          // Harvennus. Ei vielä käytössä

                    // Leikkaustunnus & koko
                    kirjaisinLeikkaustunnus = fonttiOhje.Substring(0, 1);
                    kirjaisinKoko = Int32.Parse(fonttiOhje.Substring(1, 2));

                    if (fonttiOhje.Length == 4)
                    {
                        // Harvennus
                        harvennus = fonttiOhje.Substring(3, 1);
                    }
                    
                    // leikkaus:
                    // 1 = käytetään boldaus, vaikka ??
                    // 2 = boldattu? kylän nimi 48120
                    // 3 = normaali
                    // 4 = boldattu
                    // 6 = italic
                    // 7 = backslanted
                    // 

                    var label = new Label();

                    label.FontFamily = Settings.ffLabelFont;
                    label.FontWeight = FontWeights.Regular;

                    // boldattu teksti

                    // Päätellään tekstin tyyppi
                    switch (kirjaisinLeikkaustunnus)
                    {
                        case "1":
                            label.FontWeight = FontWeights.Bold;
                            label.FontStyle = FontStyles.Normal;
                            break;
                        case "2":
                        case "4":
                            label.FontWeight = FontWeights.Bold;
                            break;
                        case "6":
                            label.FontStyle = FontStyles.Italic;
                            break;
                        default:
                            break;
                    }
                    // Kirjoitettava teksti
                    label.Content = record.Attribute.Teksti;

                    // Fontn koko. Saadaan 1/72 tuumina
                    if (kirjaisinKoko > 0)
                    {
                        double kirj = kirjaisinKoko;
                        label.FontSize = kirj * Settings.GlobalDPI / 72;
                    }
                    else
                    {
                        label.FontSize = settings.Height;
                        WriteLog("Tuntematon kirjaisinkoko '" + fonttiOhje + "' - '" + kirjaisinKoko + "'" + " luokassa " + record.Attribute.Luokka);
                    }

                    // Asetaan väri. Bordereita ei ole

                    label.Foreground = settings.Stroke;
                    label.BorderBrush = Brushes.Red;
                    label.BorderThickness = new Thickness(0);
                    label.Padding = new Thickness(0);

                    // Haetaan alkuperäinen koko
                    label.Measure(new Size(double.MaxValue, double.MaxValue));
                    var labelSize = label.DesiredSize;

                    // Suunta on 1/10 000 radiaani
                    // kulma on asteita. Väännetään tekstiä oikeaan suuntaan
                    double angle = 360 - (Convert.ToDouble(record.Attribute.Suunta) * 180 / Math.PI) / 10000;
                    var rotateTransform = new RotateTransform(angle);


                    // Keskitetty teksti pyöräytetään keskeltä. Muuten pyöräytetään vasemmasta alakulmasta
                    // Huomaa että käytössä on transformgroup, koska voi tulla ensiksi pyöräytys ja sitten kallistus vasemmalle.
                    if (settings.Shape.TextCentered)
                    {
                        var myTransformGroup = new TransformGroup();

                        if (kirjaisinLeikkaustunnus == "7") //settings.Shape.FontBackSlanted)
                        {
                            var skew = new SkewTransform(Settings.backSlantedAngle, 0);
                            myTransformGroup.Children.Add(skew);
                        }

                        label.RenderTransformOrigin = new Point(0.5, 0.5);
                        myTransformGroup.Children.Add(rotateTransform);

                        label.RenderTransform = myTransformGroup;

                    }
                    else
                    {
                        label.RenderTransformOrigin = new Point(0, 1);

                        var trans = new TranslateTransform(0, -labelSize.Height);

                        var myTransformGroup = new TransformGroup();

                        if (kirjaisinLeikkaustunnus == "7")
                        {
                            var skew = new SkewTransform(Settings.backSlantedAngle, 0);
                            myTransformGroup.Children.Add(skew);

                        }
                        myTransformGroup.Children.Add(rotateTransform);
                        myTransformGroup.Children.Add(trans);

                        label.RenderTransform = myTransformGroup;
                    }

                    label.RenderTransform.Freeze();

                    // Varjostus valkoisella
                    label.Effect = Settings.effectLabelShadow;

                    // Otetaan tekstin uusi koko tietoon, koska se on käännetty. Käytetään sitä sitten mahdollisessa siirrossa
                    // Keskelle/sivuun
                    label.Measure(new Size(double.MaxValue, double.MaxValue));
                    labelSize = label.DesiredSize;

                    label.Uid = String.Format("{0}-{1}-0", guidStart, record.Id);

                    graphicsElements[label.Uid] = record;
                    canv.Children.Add(label);
                    Canvas.SetZIndex(label, settings.ZIndex);

                    // Sitten lasketaan paljonko tekstiä pitää siirtää. Tämä riippuu onko piste keskellä vai vasemmassa alakulmassa
                    if (settings.Shape.TextCentered)
                    {
                        // Lasketaan ensiksi määritelty siirto
                        var pd1 = GetDxDyPoint((decimal)record.Attribute.Siirt_Dx, (decimal)record.Attribute.Siirt_Dy);
                        var pd2 = GetRelativePoint(record.Point.X - pd1.X, record.Point.Y - pd1.Y);

                        Canvas.SetLeft(label, pd2.X - labelSize.Width / 2);
                        Canvas.SetTop(label, pd2.Y - labelSize.Height / 2);
                    }
                    else
                    {
                        // vasen alakulma, käytetään suoraan koordinaatteja.

                        var pe4 = GetRelativePoint(record.Point.X, record.Point.Y);

                        Canvas.SetLeft(label, pe4.X);
                        Canvas.SetTop(label, pe4.Y);
                    }
                }
                else
                {
                    // Perus, eli ellipsi ruudulle:
                    var elli = new Ellipse();

                    elli.Stroke = settings.Stroke;
                    elli.StrokeThickness = GetStrokeThicknessRelative(settings.StrokeThickness);
                    elli.Width = settings.Width;
                    elli.Height = settings.Height;

                    // Jos on annettu taustaväri, käytetään sitä
                    if (settings.Fill != null)
                    {
                        elli.Fill = settings.Fill;
                        elli.Fill.Freeze();
                    }

                    // Käännetään ellipsiä. Tämä voi tulla siinä, että on tekstuuri, 
                    // muutenhan ympyrän kääntäminen on aika turha.

                    // Suunta on 1/10 000 radiaani
                    if (record.Attribute.Suunta != 0)
                    {
                        double angle = (Convert.ToDouble(record.Attribute.Suunta) * 2 * Math.PI) / 10000;
                        RotateTransform rotateTransform1 = new RotateTransform(angle);
                        rotateTransform1.CenterX = elli.Width / 2;
                        rotateTransform1.CenterY = elli.Height / 2;
                        elli.LayoutTransform = rotateTransform1;
                        elli.LayoutTransform.Freeze();
                    }

                    elli.Uid = String.Format("{0}-{1}-0", guidStart, record.Id);

                    graphicsElements[elli.Uid] = record;

                    // Lisätään se ruudulle ja asetetaan koordinaatit.

                    var p = GetRelativePoint(record.Point.X, record.Point.Y);

                    canv.Children.Add(elli);
                    Canvas.SetZIndex(elli, settings.ZIndex);

                    Canvas.SetLeft(elli, p.X - elli.Width / 2);
                    Canvas.SetTop(elli, p.Y - elli.Height / 2);
                }
            }
        }
    }
}
