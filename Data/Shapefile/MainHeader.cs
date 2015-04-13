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

namespace Karttailu2.Data.Shapefile
{
    /// <summary>
    /// Toteuttaa Layerin attribuutit. Kts. Shapefilen speksit
    /// </summary>
    public struct MainHeader
    {
        public Int32 FileCode { get; set; }     // big endian
        public Int32 UnUsed1 { get; set; }      // big
        public Int32 UnUsed2 { get; set; }      // big
        public Int32 UnUsed3 { get; set; }      // big
        public Int32 UnUsed4 { get; set; }      // big
        public Int32 UnUsed5 { get; set; }      // big
        public Int32 FileLengthWords { get; set; }       // big
        public Int32 Version { get; set; }      // little endian
        public Int32 ShapeType { get; set; }    // little
        public MBR Mbr { get; set; }            // MBR data
        public double MinZ { get; set; }        // little
        public double MaxZ { get; set; }        // little
        public double MinM { get; set; }        // little
        public double MaxM { get; set; }        // little

        // custom addittional info
        public Int32 FileLength { get; set; }   // big
    }
}
