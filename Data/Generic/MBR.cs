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
namespace Karttailu2.Data.Generic
{
    /// <summary>
    /// Toteuttaa MBR (Minimum bounding rectangle). Tämä kertoo yleensä recordin 
    /// täyttämän alueen, eli minimi- ja maksimikoordinaatit suorakulmiolle, 
    /// minkä sisässä recordin pisteet ovat.
    /// </summary>
    public class MBR
    {
        public double MbrMinX { get; set; }  
        public double MbrMinY { get; set; }     
        public double MbrMaxX { get; set; }   
        public double MbrMaxY { get; set; }

        public MBR(double minX, double minY, double maxX, double maxY)
        {
            MbrMinX = minX;
            MbrMaxX = maxX;
            MbrMinY = minY;
            MbrMaxY = maxY;
        }

        public MBR()
        {
            // Asetetaan maksimeille minimiarvo ja minimeille maksimiarvot.
            MbrMaxX = MbrMaxY = double.MinValue;
            MbrMinX = MbrMinY = double.MaxValue;
        }

        /// <summary>
        /// Etsitään rajat tälle layerille.
        /// </summary>
        /// <param name="MbrMinX">Mbr.MinX</param>
        /// <param name="MbrMinY">Mbr.MinY</param>
        /// <param name="MbrMaxX">Mbr.MaxX</param>
        /// <param name="MbrMaxY">Mbr.MaxY</param>
        public void CheckMaxMBR(double MbrMinX, double MbrMinY, double MbrMaxX, double MbrMaxY)
        {
            this.MbrMaxX = Math.Max(this.MbrMaxX, MbrMaxX);
            this.MbrMinX = Math.Min(this.MbrMinX, MbrMinX);
            this.MbrMaxY = Math.Max(this.MbrMaxY, MbrMaxY);
            this.MbrMinY = Math.Min(this.MbrMinY, MbrMinY);
        }
    }
}
