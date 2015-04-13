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
    public class PolygonAttribute
    {
        public string Teksti { get; set; }      // C80
        public int Ryhma { get; set; }          // N11
        public int Luokka { get; set; }         // N11
        public Decimal Tastar { get; set; }     // N11: Sijaintitarkkuus-luokkakoodi
        public Decimal Kortar { get; set; }     // N11: Korkeustarkkuus-luokkakoodi
        public Decimal KorArv { get; set; }     // N11,1: Korkeusarvo
        public Decimal Kulkutapa { get; set; }  // N11: Kulkutapakoodi : 1 = murto, 2 = käyrä, 4 = tihentämätön käyrä 
        public int     Versuh { get; set; }     // N11: Vertikaalisuhde-koodi
        public Decimal Suunta { get; set; }     // N11: Symbolin suunta 1/10 000 -osa radiaaneina
        public Decimal Siirt_Dx { get; set; }   // N11: Siirtymä itään (mm)
        public Decimal Siirt_Dy { get; set; }   // N11: Siirtymä pohjoiseen (mm)
        public Decimal Korkeus { get; set; }    // N7,3
    }
}
