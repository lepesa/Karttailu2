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

namespace Karttailu2.Data.Generic
{

    /// <summary>
    /// Pisteen tiedot. 
    /// </summary>
     
    public class PointRecord : Record
    {   
        
        // Normaali 2d koordinaatti
        public Point Point { get; set; }
    
        // Pisteen attribuutit
        public PointAttribute Attribute { get; set; }
        public Int32 Id { get; set; }
        public ShapeType Type { get; set; }
        
        public PointRecord()
        {
            Attribute =  new PointAttribute();
            Point = new Point();
        }

        // Stringi debuggia varten
        public override string ToString()
        {
            return "Point: " + Attribute.Luokka + " - korkeus:" + Attribute.KorArv + " - tyyppi:" + Attribute.Luokka + "ryhma: " + Attribute.Ryhma + " . Versuh:"  + Attribute.Versuh;
        }
    }
}
