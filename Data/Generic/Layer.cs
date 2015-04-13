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

namespace Karttailu2.Data.Generic
{
    /// <summary>
    /// Toteuttaa layerin, jonka alle kaikki recordit ja objektit tulevat. 
    /// Jokainen recordityyppi on omassa listassaan. Tämä rakenen pohjautuu 
    /// ShapeFilen rakenteeseen mutta ei ole 1:1.
    /// </summary>
    public class Layer
    {
        public string FileName { get; set; }

        public MBR Mbr { get; set; }

        public List<PointRecord> PointRecords { get; set; }
        public List<PolylineRecord> PolylineRecords { get; set; }
        public List<PolygonRecord> PolygonRecords { get; set; }

        public Layer()
        {
            PointRecords = new List<PointRecord>();
            PolylineRecords = new List<PolylineRecord>();
            PolygonRecords = new List<PolygonRecord>();
            Mbr = new MBR();
        }



    }
}
