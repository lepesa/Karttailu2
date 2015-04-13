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

using Karttailu2.Data.Shapefile;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Karttailu2.Data.Shapefile
{
    /// <summary>
    /// Polygonin toteuttava recordi. Sisältää attribuutit ja polygonin datat. Polygoni on vähintään neljän pisteen viiva ja se voi olla useammista osista koottu.
    /// Esim. järvi ja siellä olevat saaret. Nämä sitten maalataan 
    /// </summary>

    public class PolygonRecord : Record
    {
        public int Number { get; set; }


        public PolygonAttribute Attribute { get; set; }

        public MBR Box { get; set; }
        public Int32 NumParts { get; set; }
        public Int32 NumPoints { get; set; }
        public PointPart[] Parts { get; set; }
        
        public PolygonRecord()
        {
            Attribute = new PolygonAttribute();
        }

        /// <summary>
        /// Aseta polylinet attribuutit oikeiksi 
        /// </summary>
        /// <param name="dt">Attribuuttirivi Dbase IV -tiedostossa</param>
        public void ReadAttributes(DataRow dr)
        {
            Attribute.Teksti = DBaseIVHelper.ReadChar(dr, "TEKSTI", 80);
            Attribute.Ryhma = DBaseIVHelper.ReadInt(dr, "RYHMA");
            Attribute.Luokka = DBaseIVHelper.ReadInt(dr, "LUOKKA");
            Attribute.Tastar = DBaseIVHelper.ReadDecimal(dr, "TASTAR");
            Attribute.Kortar = DBaseIVHelper.ReadDecimal(dr, "KORTAR");
            Attribute.KorArv = DBaseIVHelper.ReadDecimal(dr, "KORARV");
            Attribute.Kulkutapa = DBaseIVHelper.ReadDecimal(dr, "KULKUTAPA");
            Attribute.Kohdeoso = DBaseIVHelper.ReadDecimal(dr, "KOHDEOSO");
            Attribute.Ainlahde = DBaseIVHelper.ReadDecimal(dr, "AINLAHDE");
            Attribute.Syntyhetki = DBaseIVHelper.ReadChar(dr, "SYNTYHETKI", 8);
            Attribute.Kuolhetki = DBaseIVHelper.ReadChar(dr, "KUOLHETKI", 8);
            Attribute.Kartoglk = DBaseIVHelper.ReadDecimal(dr, "KARTOGLK");
            Attribute.Aluejakoon = DBaseIVHelper.ReadDecimal(dr, "ALUEJAKOON");
            Attribute.Versuh = DBaseIVHelper.ReadDecimal(dr, "VERSUH");
            Attribute.Suunta = DBaseIVHelper.ReadDecimal(dr, "SUUNTA");
            Attribute.Siirt_Dx = DBaseIVHelper.ReadDecimal(dr, "SIIRT_DX");
            Attribute.Siirt_Dy = DBaseIVHelper.ReadDecimal(dr, "SIIRT_DY");
            Attribute.Korkeus = DBaseIVHelper.ReadDecimal(dr, "KORKEUS");
            Attribute.Attr2 = DBaseIVHelper.ReadDecimal(dr, "ATTR2");
            Attribute.Attr3 = DBaseIVHelper.ReadDecimal(dr, "ATTR3");
        }
    }
}
