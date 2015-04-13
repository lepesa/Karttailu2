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
using System.Windows.Controls;
using System.Windows.Media;


namespace Karttailu2.Graphics.GenericWpf
{
    public partial class MapVisualiser : IMapVisualiser
    {
        DrawingSettings dwSettings = null;
        Karttailu2.Data.Generic.MBR mapMBR = null;

        Dictionary<int, Karttailu2.Graphics.ClassSettings> drawClasSettings = null;

        Dictionary<int, SolidColorBrush> drawReunaviivaSettings = null;
        Dictionary<int, SolidColorBrush> drawEpamaarainenReunaviivaSettings = null;

        Dictionary<string, Karttailu2.Data.Generic.Record> graphicsElements = new Dictionary<string, Data.Generic.Record>();

        ILog logger = null;

        double scaleX, scaleY;
        double topX, topY;

        /// <summary>
        /// Palauttaa elementin, joka tunnisteellä löytyy
        /// </summary>
        /// <param name="id">elementin tunniste</param>
        /// <returns>Elementti</returns>
        public Karttailu2.Data.Generic.Record GetGraphicsElement(string id)
        {
            if (graphicsElements.ContainsKey(id))
            {
                return graphicsElements[id];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Kirjoittaa logiin tekstin
        /// </summary>
        /// <param name="text">Lokiin menevä teksti</param>
        private void WriteLog(string text)
        {
            logger.WriteLog(text);
        }
        
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
        /// 
        public void DrawMap(Canvas canvas, List<Data.Generic.Layer> layers, DrawingSettings drawSettings, Karttailu2.Data.Generic.MBR mbr, ILog _logger, double cWidth, double cHeight)
        {
            logger = _logger;
            dwSettings = drawSettings;
            mapMBR = mbr;

            drawClasSettings = dwSettings.GetSettings();
            drawReunaviivaSettings = dwSettings.GetReunaviivaSettings();
            drawEpamaarainenReunaviivaSettings = dwSettings.GetEpamaarainenReunaviivaSettings();

            // Lasketaan rajat / skaalaus. 

            double sizeX = (mbr.MbrMaxX - mbr.MbrMinX);
            double sizeY = (mbr.MbrMaxY - mbr.MbrMinY);

            double borderSize = 64;

            scaleX = ((double)cWidth - borderSize) / (sizeX);
            scaleY = ((double)cHeight - borderSize) / (sizeY);

            topX = mbr.MbrMinX - (borderSize / 2);
            topY = mbr.MbrMaxY + (borderSize / 2);
            
            // Piirretään eri muodot:
            // Polygon
            foreach (var layer in layers)
            {
                foreach (var record in layer.PolygonRecords)
                {
                    DrawPolygon(canvas, record);
                }
            }
            
            // Polyline, ei täytettyä aluetta

            foreach (var layer in layers)
            {
                foreach (var record in layer.PolylineRecords)
                {
                    DrawPolyline(canvas, record);
                }
            }

            // Piste
            foreach (var layer in layers)
            {
                foreach (var record in layer.PointRecords)
                {
                    DrawPoint(canvas, record);
                }
            }
        }
    }
}
