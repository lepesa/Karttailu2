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
    /// Normaali polygon. Koostuu viivoista, jotka sulkevat alueen ja alue täytetään VOi koostua 
    /// useasta objektista, jotka leikkaavat alueesta osia pois. Esim. järvi ja sen saaret.
    /// </summary>
    public class PolygonRecord : Record
    {
        // Vakioattribuutit
        public PolygonAttribute Attribute { get; set; }

        public Int32 Id { get; set; }
        public ShapeType Type { get; set; }

        // Polygonin alue
        public MBR Mbr { get; set; }
     
        // Polygonin sisältämät objektit
        public Object[] Objects { get; set; }

        public PolygonRecord()
        {
            Attribute = new PolygonAttribute();
        }

        // Stringi debuggia varten
        public override string ToString()
        {
            return "Polygon: " + Attribute.Luokka + " - korkeus:" + Attribute.KorArv + " - tyyppi:" + Attribute.Luokka + "ryhma: " + Attribute.Ryhma + " . Versuh: " + Attribute.Versuh;
        }
    }
}
