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

namespace Karttailu2.Data.Shapefile
{
    /// <summary>
    /// Layer-luokka. Toteuttaa yhden tiedoston tiedot ja attribuutit.
    /// </summary>
    public class Layer
    {

        /// <summary>
        /// Tiedoston nimie
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Headerin tiedot
        /// </summary>
        public MainHeader Header { get; set; }

        /// <summary>
        /// Layerin sisältämät kuviot attribuutteineen.
        /// </summary>
        public List<Record> Records {get; set; }

        public Layer()
        {
            Records = new List<Record>();
        }
    }
}
