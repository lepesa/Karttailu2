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
using System.Runtime.InteropServices;
using System.Text;

namespace Karttailu2
{
    public class FileNameHelper
    {
        /// <summary>
        /// Haetaan tiedostolle 8.3 nimi. Tätä käytetään DBASEn taulun nimenä.
        /// </summary>
        /// <param name="longPath">Tiedoston mimi kokonaan</param>
        /// <returns>Tiedoston nimen 8.3 formaatissa</returns>
        public static String GetShortPathName(String directory, String fileName)
        {
            string path = System.IO.Path.Combine(directory, fileName);

            StringBuilder shortPath = new StringBuilder(path.Length + 1);

            if (0 == NativeMethods.GetShortPathName(path, shortPath, shortPath.Capacity))
            {
                return fileName;
            }

            return System.IO.Path.GetFileName(shortPath.ToString());
        }
        
        /// <summary>
        /// Haetaan tiedostolle 8.3 nimi. Tähän joudutaan käyttämään kernel32.dll:n tarjoamaa rajapintaa.
        /// </summary>
        internal static class NativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            [return: MarshalAs(UnmanagedType.I4)]
            internal static extern Int32 GetShortPathName(String path, StringBuilder shortPath, Int32 shortPathLength);
        }

    }
}
