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

namespace Karttailu2.Data.Shapefile
{
    /// <summary>
    /// Toteuttaa MBR (Minimum bounding rectangle) tietorakenteen.
    /// Toisin sanoen määrittelee laatikon, jonka sisällä luvataan layerin/recordin pisteiden olevan.
    /// </summary>
    public class MBR
    {
        
        public double MbrMinX { get; set; }     // little endian
        public double MbrMinY { get; set; }     // little
        public double MbrMaxX { get; set; }     // little
        public double MbrMaxY { get; set; }     // little

        /// <summary>
        /// Asetetaan MBR:n valmiit arvot. 
        /// </summary>
        /// <param name="minX">Vasemman yläkulman X-koordinaatti</param>
        /// <param name="maxX">Oikean alakulman X-koordinaatti</param>
        /// <param name="minY">Vasemman yläkulman Y-koordinaatti</param>
        /// <param name="maxY">Oikean alakulman Y_koordinaatti</param>
        public MBR(double minX, double maxX, double minY, double maxY)
        {
            MbrMinX = minX;
            MbrMaxX = maxX;
            MbrMinY = minY;
            MbrMaxY = maxY;
        }
        
        /// <summary>
        /// Oletusmuodostin. Maksimit ja minimit kuntoon. Nämä korvaantuvat kuitenkin tiedostosta luetuilla 
        /// arvoilla.
        /// </summary>
        public MBR()
        {
            MbrMinX = MbrMinY = double.MaxValue;
            MbrMaxX = MbrMaxY = double.MinValue;
                
        }
    }
}
