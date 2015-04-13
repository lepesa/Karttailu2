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
using System.Data;
using System.Text;


namespace Karttailu2
{
    /// <summary>
    /// DBase IB käyttämät rutiinit
    /// </summary>
    public class DBaseIVHelper
    {

        private static Encoding OleDbEncoding = Encoding.GetEncoding(437); //Encoding.GetEncoding(437);
        
        /// <summary>
        /// Palautetaan connection stringi 34/64 bittiselle ajurille.
        /// </summary>
        /// <param name="filePath">DBaseIV tiedoston koko polku. Huom! Tiedoston nimeä käytetään vasta haussa, 8.3 formaatissa!</param>
        /// <returns>connection string</returns>
        public static string GetConnectionString(string filePath)
        {

            // 32 bittinen kirjasto
            string constr = String.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=dBASE IV;", filePath);

            if (Environment.Is64BitProcess)
            {
                // 64 bittinen kirjasto
                constr = String.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=dBASE IV;", filePath);
            }
            return constr;
        }
        
        /// <summary>
        /// Datatyyppi "Cnn", eli tekstimuotoinen data
        /// </summary>
        /// <param name="dr">Datarow</param>
        /// <param name="columnName">Sarakkeen nimi</param>
        /// <param name="maxLength">Maksimipituus. Vapaaehtoinen</param>
        /// <returns>Merkkijonon, mikä rivillä oli tietyssä sarakkeessa</returns>

        public static string ReadChar(DataRow dr, string columnName, int maxLength = -1)
        {
            string data = dr[columnName].ToString();
            if (maxLength > 0 && data.Length > maxLength)
            {
                throw new Exception(String.Format("DBaseIVHelper ReadChar: Colun {0} - string is too long: {1}/{2} bytes",columnName, data.Length, maxLength ));
            }

            var str = OleDbEncoding.GetBytes(data);

            // ö on jostain syystä aineistossa väärällä koodauksella
            for (int i = 0; i < str.Length-1; i++ )
            {
                if( str[i]==195 && str[i+1]==65)
                {
                    // ö
                    str[i + 1] = 164+18;
                }

                if (str[i] == 65 && str[i + 1] == 164)      // Ǥ c7a4
                {
                    str[i] = 199;
                    str[i + 1] = 164;
                }

                if (str[i] == 65 && str[i + 1] == 165)      // ǥ c7a5
                {
                    str[i] = 199;
                    str[i + 1] = 165;
                }

                if (str[i] == 65 && str[i + 1] == 166)      // Ǧ c7a6
                {
                    str[i] = 199;
                    str[i + 1] = 166;
                }

                if (str[i] == 65 && str[i + 1] == 167)      // ǧ  c7a7
                {
                    str[i] = 199;
                    str[i + 1] = 167;
                }
                if (str[i] == 65 && str[i + 1] == 168)      // Ǩ c7a8
                {
                    str[i] = 199;
                    str[i + 1] = 168;
                }

                if (str[i] == 65 && str[i + 1] == 114)      // ǩ c7a9 
                {
                    str[i] = 199;
                    str[i + 1] = 169;
                }

                if (str[i] == 65 && str[i + 1] == 174)      // Ǯ c7ae
                {
                    str[i] = 199;
                    str[i + 1] = 174;
                }
                if (str[i] == 65 && str[i + 1] == 175)      // ǯ  c7af
                {
                    str[i] = 199;
                    str[i + 1] = 175;
                }
                /*
                 * iso-8859-10         utf-8    
#      hex           unicode-hex  utf-8-hex     
ë   235    EB       ǧ    U+01E7       c7a7      Latin small letter G with caron
Ë   203    CB       Ǧ    U+01E6       c7a6      Latin capital letter G with caron
ę   234    EA       ǥ    U+01E5       C7A5      Latin small letter G with stroke
Ę   202    CA       Ǥ    U+01E4       c7a4      Latin capital letter G with stroke
ė   236    EC       ǩ    U+01E9       c7a9      Latin small letter K with caron
Ė   204    CC       Ǩ    U+01E8       01e8      Latin capital letter K with caron
ō   242    F2       Ʒ    U+0292       ca92      Latin small letter EZH
Ō   210    D2       Ʒ    U+01B7       c6b7      Latin capital letter EZH
ó   243    F3       ǯ    U+01EF       c7af      Latin small letter EZH with caron
Ó   211    D3       Ǯ    U+01EE       c7ae      Latin capital letter EZH with caron*/
            
            }
            
            // UTF-8:ksi. Ääkköset toimimaan.
            return Encoding.UTF8.GetString(str);
        }

        /// <summary>
        /// Konvertoi luvun desimaaliksi.
        /// </summary>
        /// <param name="dr">Datarow</param>
        /// <param name="columnName">Sarakkeen nimi</param>
        /// <returns>Sarakkeen sisältämän desimaaliluvun</returns>
        public static Decimal ReadDecimal(DataRow dr, string columnName)
        {
            string data = dr[columnName].ToString();

            if( data.Length == 0 )
            {
                return 0;
            }

            try
            {
                return Decimal.Parse(data);
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("DBaseIVHelper ReadDecimal: Colun {0} : cannot convert decimal '{1}': " + e.Message, columnName, data));
            }
        }

        /// <summary>
        /// Konvertoi luvun kokonaisluvuksi Int32 lukualueelle.
        /// </summary>
        /// <param name="dr">Datarow</param>
        /// <param name="columnName">Sarakkeen nimi</param>
        /// <returns>Sarakkeen sisältämän luvun</returns>
        public static int ReadInt(DataRow dr, string columnName)
        {
            string data = dr[columnName].ToString();

            if (data.Length == 0)
            {
                return 0;
            }

            try
            {
                return Int32.Parse(data);
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("DBaseIVHelper ReadInt: Colun {0} : cannot convert decimal '{1}':" + e.Message, columnName, data));
            }
        }
    }
}
