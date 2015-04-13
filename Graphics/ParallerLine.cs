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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Karttailu2.Graphics
{
    public class ParallerLine
    {

        /// <summary>
        /// Toteuttaa annetuista pisteistä uuden vierekkäisen pistejonon annetun leveyden päähän.
        /// Viiva lasketaan ottamalla olemasta olevasta viivasta normaali ja sen perusteella lasketaan se
        /// n-pisteen päähän. Tämän jälkeen voidaan laskea uudelle viivalle alkupiste edellisen viivan ja uuden viivan
        /// yhtälöjen perusteella. Tämä piste tulee myös edellisen viivan loppupisteeksi. Syy, miksi näin tehdään on se, että
        /// viiva ei ole kulmissa tms. aina n-pisteen päästä toisesta viivasta:(X-X vanha viiva, Y-Y-Y uusi viiva)
        /// 
        ///   YYYYYYYYYYYYYYYY
        ///   Y              Y
        ///   Y XXXXXXXXXXXX Y
        ///   Y X          X Y
        ///   Y X          X  YYYYYYYYY
        ///   Y X           X  
        ///   Y X            XXXXXXXXX
        /// </summary>
        /// <param name="points">Lista pisteistä</param>
        /// <param name="width">Kuinka kauaksi pisteistä tehdään pistejono</param>
        /// <returns>Listan pisteistä, jotka ovat width etäisyydellä</returns>
        public static List<Point> CreateParallerPoints(List<Point> points, double width)
        {
            // Palautettava lista pisteistä
            var retList = new List<Point>();
            
            // viivan alku- ja loppupisteet edelliselle ja nykyiselle viivalle
            var line1Start = new Point();
            var line1End = new Point();
            var line2Start = new Point();
            var line2End = new Point();

            int count = points.Count;

            for (int i = 0; i < count; i++)
            {
                // Alkupiste
                if (i == 0)
                {
                    // Laske alkupiste ja seuraava piste
                    CalculateLinePoint(ref line1Start, ref line1End, points[i], points[i + 1], width);
                    retList.Add(new Point(line1Start.X, line1Start.Y));

                    // yksi viiva vain, eli kaksi pistettä
                    if (count == 2)
                    {
                        line2End = line1End;
                    }
                }
                else
                    // Loppupiste
                    if (i == count - 1)
                    {
                        // Viiva loppuu tähän.
                        retList.Add(new Point(line2End.X, line2End.Y));
                    }
                    // välipisteet -> laske viivojen leikkaus
                    else
                    {
                        // Laske seuraava viiva
                        CalculateLinePoint(ref line2Start, ref line2End, points[i], points[i + 1], width);

                        // Laske nykyisen viivan ja seuraavan viivan leikkaus
                        Point interP = CalculateIntersection(line1Start, line1End, line2Start, line2End);

                        // Leikkauspiste listaan
                        retList.Add(interP);

                        // Nykyinen viiva tulee vanhaksi viivaksi seuraavassa loopissa
                        line1Start = line2Start;
                        line1End = line2End;
                    }
            }
            return retList;
        }

        /// <summary>
        /// Lasketaan normaalin avulla samansuuntaine vektori. Pistetään alku- ja loppupiste
        /// olemasa olevan viivan alku- ja loppupisteestä laskettuna normaalin avulla. Tämä 
        /// alku- ja loppupiste muuttuu myöhemmin, jos ei ole ensimmäinen/viimeinen piste.
        /// </summary>
        /// <param name="lineStart">Laskettavan viivan alkupiste</param>
        /// <param name="lineEnd">Laskettavan viivan loppupiste</param>
        /// <param name="startPoint">Alkupiste lähteestä</param>
        /// <param name="endPoint">Loppupiste lähteestä</param>
        /// <param name="width">Kuinka kauksi viiva tehdään lähteestä</param>
        private static void CalculateLinePoint(ref Point lineStart, ref Point lineEnd, Point startPoint, Point endPoint, double width = 2.5)
        {
            // Lasketaan viivan normaali
            var v = endPoint - startPoint;
            var n = new Vector(v.Y, -v.X);
            // Normalisoidaan se
            n.Normalize();

            //Lasketaan uusi alku- ja loppupiste lähdepisteiden, normaalin ja etäisyyden avulla.
            lineStart = startPoint + width * n;
            lineEnd = endPoint + width * n;
        }

        /// <summary>
        /// Lasketaan kahden annetun viivan leikkauspiste. Näin saadaan siististi kaikkiin kulmiin jatkuva viiva,
        /// joka ei huoju sinne tänne, riippuen kulman koveruudesta/kuperuudesta.
        /// </summary>
        /// <param name="p1">Viivan 1 alkupiste</param>
        /// <param name="p2">Viivan 1 loppupiste</param>
        /// <param name="r1">Viivan 2 alkupiste</param>
        /// <param name="r2">Viivan 2 loppupiste</param>
        /// <returns>Viivojen leikkauspiste</returns>
        private static Point CalculateIntersection(Point p1, Point p2, Point r1, Point r2)
        {
            // Viivojen leikkauspisteen matemaattinen yhtälö suoraan alla:

            decimal A1 = (decimal)p2.Y - (decimal)p1.Y;
            decimal B1 = (decimal)p1.X - (decimal)p2.X;
            decimal C1 = A1 * (decimal)p1.X + B1 * (decimal)p1.Y;

            decimal A2 = (decimal)r2.Y - (decimal)r1.Y;
            decimal B2 = (decimal)r1.X - (decimal)r2.X;
            decimal C2 = A2 * (decimal)r1.X + B2 * (decimal)r1.Y;

            // determinantti
            decimal det = A1 * B2 - A2 * B1;

            if (det == 0)
            {
                // Viivat samansuuntaisia, palautetaan jälkimmäisen viivan alkupiste.
                return new Point(r1.X, r1.Y);
            }
            else
            {
                // Lasketaan leikkauspiste ja palautetaan se
                double x = (double)((B2 * C1 - B1 * C2) / det);
                double y = (double)((A1 * C2 - A2 * C1) / det);
                return new Point(x, y);
            }
        }
    }
}
