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
using System.Windows.Media;

namespace Karttailu2
{
    /// <summary>
    /// Tarjoaa luokan, joka toteuttaa kartan latauksen ja piirron käyttäen haluttuja 
    /// luokkia.
    /// </summary>
    public class MainMap
    {
        List<Karttailu2.Data.Generic.Layer> layers = new List<Data.Generic.Layer>();
        
        Karttailu2.Graphics.DrawingSettings dwSettings = null;
        Karttailu2.Data.Generic.MBR maxMBR = new Karttailu2.Data.Generic.MBR(double.MaxValue, double.MaxValue, double.MinValue, double.MinValue);
        Karttailu2.Graphics.IMapVisualiser visualiser = null;

        public MainMap()
        {
            // Haetaan piirtoasetukset ja ladataan kuvat
            dwSettings = new Karttailu2.Graphics.DrawingSettings();
            dwSettings.LoadImages();
        }

        /// <summary>
        /// Lataa kartan muistiin.
        /// </summary>
        /// <param name="mapfileType">Annettu mappityyppi. Nyt tuetaan vain Shapefile</param>
        /// <param name="name">Hakemisto, mistä kartta löytyy</param>
        public void LoadMap(Data.MapfileFactory.MapfileType mapfileType, string directoryName)
        {
            // Haetaan oikea tiedostolataaja
            var loader = Karttailu2.Data.MapfileFactory.GetMapfile(mapfileType);

            // Ladataan tiedosto
            var layer = loader.LoadMap(directoryName);
            
            // Lisätään kartta muistiin
            layers.Add(layer);

            // Lasketaan kartan yläreuna ja koko.
            maxMBR.CheckMaxMBR(layer.Mbr.MbrMinX, layer.Mbr.MbrMinY, layer.Mbr.MbrMaxX, layer.Mbr.MbrMaxY);

        }

        /// <summary>
        ///  Piirtää kartan ruudulle. Tähän käytetään annettua piirtorajapintaa.
        /// </summary>
        /// <param name="canv">Canvas, mihin piirretään</param>
        /// <param name="visualiserType">Piirtorajapinnan tyyppi</param>
        public void DrawMap(System.Windows.Controls.Canvas canv, Graphics.MapVisualiserFactory.MapVisualiserType visualiserType, ILog logger, double cHeight , double cWidth)
        {
            visualiser = Karttailu2.Graphics.MapVisualiserFactory.GetMapVisualiser(visualiserType);
            visualiser.DrawMap(canv, layers, dwSettings, maxMBR, logger, cHeight, cWidth);
        }

        /// <summary>
        /// Palauttaa kutsuvalle ohjelmalle id:n perusteella Recordin. Tätä voidaan käyttää graafisessa
        /// käyttöliittymässä hyväksi, jos elementin id:ksi on asetettu tämä. NULL tarkoittaa ,että recordia
        /// ei löytynyt.
        /// </summary>
        /// <param name="id">Recordin ID</param>
        /// <returns>Recordi</returns>
        public Karttailu2.Data.Generic.Record GetGraphicsElement(string id)
        {
            if( visualiser != null)
            {
                return visualiser.GetGraphicsElement(id);
            } 
            else
            {
                return null;
            }
        }
    }
}
