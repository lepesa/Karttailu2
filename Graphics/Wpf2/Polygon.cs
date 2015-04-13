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
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
namespace Karttailu2.Graphics.Wpf2
{
    public partial class MapVisualiser
    {
        /// <summary>
        /// Lasketaan annetun polygonin zIndex ja pistetään se tietorakenteeseen. 
        /// </summary>
        /// <param name="record">Recordi</param>
        private void CalculatePolygonZ(Data.Generic.PolygonRecord record)
        {
            // Haetaan piirtoasetukset tälle luokalle ja ryhmälle
            var classSetting = GetSettings(record.Attribute.Luokka, record.Attribute.Ryhma);

            // Varoitetaan, jos versuh on tukematon.
            if (record.Attribute.Versuh != 0 && classSetting.VerSuh == 0)
            {
                WriteLog("Polygon Versuh detected: " + record.Attribute.Versuh + " record: " + record.Id + "  type:" + record.Attribute.Luokka);
            }

            // Jos viivanlevys on -1, aluetta ei piirretä
            if (classSetting.StrokeThickness == -1)
            {
                return;
            }

            AddZIndex(record, classSetting.ZIndex);
        }

        /// <summary>
        /// Piirtää polygon -objektin ja lisää sen Canvakselle.
        /// </summary>
        /// <param name="record">Recordi, joka sisältää polygonin</param>

        private void DrawPolygon(Data.Generic.PolygonRecord record, int zIndex)
        {
            // Haetaan piirtoasetukset tälle luokalle ja ryhmälle
            var classSetting = GetSettings(record.Attribute.Luokka, record.Attribute.Ryhma);

            // Varoitetaan, jos versuh on tukematon.
            if (record.Attribute.Versuh != 0 && classSetting.VerSuh == 0)
            {
                WriteLog("Polygon Versuh detected: " + record.Attribute.Versuh + " record: " + record.Id + "  type:" + record.Attribute.Luokka);
            }

            // Jos viivanlevys on -1, aluetta ei piirretä
            if (classSetting.StrokeThickness == -1)
            {
                return;
            }

            // Käydään läpi kaikki polygonin objektit (voi olla useampia, esim saari järvessä on oma objekti)
            PathFigureCollection myPathFigureCollection = new PathFigureCollection();
            for (int i = 0; i < record.Objects.Length; i++)
            {
                // Lisätää pisteet segmenttiin

                PathFigure myPathFigure = new PathFigure();
                PathSegmentCollection myPathSegmentCollection = new PathSegmentCollection();

                int pointsNum = record.Objects[i].Points.Length;

                // Alkaa ykkösestä, alkupiste annetaan erikseen
                for (int j = 1; j < pointsNum; j++)
                {
                    // Seuravat pisteet menevät suoraan listaan
                    var myLineSegment = new LineSegment();
                    myLineSegment.Point = GetRelativePoint(record.Objects[i].Points[j].X, record.Objects[i].Points[j].Y);
                    myPathSegmentCollection.Add(myLineSegment);
                }
                // Alkupiste pitää antaa erikseen
                myPathFigure.StartPoint = GetRelativePoint(record.Objects[i].Points[0].X, record.Objects[i].Points[0].Y);
                // Ja segmentit
                myPathFigure.Segments = myPathSegmentCollection;
                myPathFigureCollection.Add(myPathFigure);
            }

            var myPathGeometry = new PathGeometry();
            myPathGeometry.Figures = myPathFigureCollection;

            // Tehdään alue, jossa on mukana segmentit
            Path myPath = new Path();
            myPath.Stroke = classSetting.Stroke;
            myPath.StrokeThickness = GetStrokeThicknessRelative(classSetting.StrokeThickness);

            // Millä värillä väritetään?
            if (classSetting.Fill != null)
            {
                myPath.Fill = classSetting.Fill;
            }

            myPathGeometry.FillRule = FillRule.Nonzero;
            myPathGeometry.Freeze();
            myPath.Data = myPathGeometry;

            // Lisätään elementti canvakselle
            AddItem(myPath);
        }
    }
}
