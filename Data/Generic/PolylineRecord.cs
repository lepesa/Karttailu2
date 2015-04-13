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
    /// Polyline, eli viivajoukko, jota ei täytetä.
    /// </summary>
    public class PolylineRecord : Record
    {
        // normaalit attribuutit
        public PolylineAttribute Attribute { get; set; }

        public Int32 Id { get; set; }
        public ShapeType Type { get; set; }

        // Alueen rajat, minkä sisälle piirretään
        public MBR Mbr { get; set; }
        
        // Joukon objektit
        public Object[] Objects { get; set; }
       

        public PolylineRecord()
        {
            Attribute = new PolylineAttribute();
            Mbr = new MBR();
        }

        // Stringi debuggia varten
        public override string ToString()
        {
            return "Line: " + Attribute.Luokka + " - korkeus:" + Attribute.KorArv + " - tyyppi:" + Attribute.Luokka + "ryhma: " + Attribute.Ryhma+ " - kulkutapa: " + Attribute.Kulkutapa + " - Versuh: " + Attribute.Versuh + " | " ;
        }
    }
}
