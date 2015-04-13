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

namespace Karttailu2.Graphics.GenericWpf
{
    public partial class MapVisualiser
    {

        /// <summary>
        /// Palauttaa luokan(kuvion) asetukset piirtämistä varten
        /// </summary>
        /// <param name="luokka">Luokan tunnus</param>
        /// <returns>Luokan piirtoasetukset</returns>
        private Karttailu2.Graphics.ClassSettings GetSettings(int luokka, int ryhma)
        {

            // Palautetaan luokat tiedot, jos sille on asetukset

            if (drawClasSettings.ContainsKey(luokka))
            {
                if (drawClasSettings[luokka].Group == ryhma)
                {
                    return drawClasSettings[luokka];
                }
                else
                {
                    // Ei ollut oikeaa luokkaa. Käydään hakemassa seuraava osuma tälle
                    // Luokka alle 100 000, niin normi. Kerrotaan kymmenellä, jotta saadaan seuraavat tämän luokan osumat
                    if (luokka < 99999)
                    {
                        luokka = luokka * 10;
                    }
                    // seuraava osuma on +1. eli avaimet ovat: 44300, 443001, 443002... 
                    luokka++;

                    // Rekursiivinen kutsu. näitä ei pitäisi olla kuitenkaan kuin kolme kappaletta, joten ei ongelmaa.
                    return GetSettings(luokka, ryhma);
                }
            }

            logger.WriteLog("--Tuntematon luokka:" + luokka + " ryhmä: " + ryhma);

            // Jos luokkaa ei ollut, palautetaan default-asetukset, jotka ovat tunnuksella 0.

            return drawClasSettings[0];
        }


        /// <summary>
        ///  Palauttaa DPI:n ja kartan mittakaavan vaatiman viivan paksuuden.
        /// </summary>
        /// <param name="thickness">Asetuksissa oleva viivan paksuus</param>
        /// <returns>Piirretävän viivan paksuus</returns>
        private double GetStrokeThicknessRelative(double thickness)
        {
            return thickness;
        }


        /// <summary>
        /// Palauttaa zIndeksin muutokset verSuh-tiedon perusteella.
        /// -1 maan alla (tai tie sillan alla)
        /// 0 = normaali
        /// 1,2,3,4,5 tasoja maanpinnan yläpuolella
        /// -11 = tunneli
        /// </summary>
        /// <param name="attr">Tiedostosta saatu VerSuh</param>
        /// <param name="verSuh">Piirtotietojen asetus</param>
        /// <param name="zIndex">Elementin zIndex</param>
        /// <returns>Uuden zIndeksin</returns>
        private int GetVerSuhZIndex(int attr, Graphics.VerSuh verSuh, int zIndex)
        {
            // Jos 
            if (verSuh == Graphics.VerSuh.Normaali || verSuh == Graphics.VerSuh.EiMitaan || verSuh == Graphics.VerSuh.Erikois)
            {
                return zIndex;
            }
            if (verSuh == Graphics.VerSuh.LaskutOk && attr == -1)
            {
                return zIndex - 20;
            }
            if (verSuh == Graphics.VerSuh.NousutOk && attr >= 1)
            {
                return zIndex + attr * 20;
            }
            if (verSuh == Graphics.VerSuh.MolemmatOk && (attr >= 1 || attr == -1))
            {
                return zIndex + attr * 20;
            }

            throw new Exception("Not supported GetVerSuhIndex value:" + attr + " versuh value:" + verSuh);
        }

        /// <summary>
        /// Palauttaa relatiivisen pisteen ruudulla. Vähentää koordinaatista yläkulman ja kertoo sen skaalauksella. 
        /// Tällöin saadaan piste ruudulla.
        /// </summary>
        /// <param name="p1">Piste kartalla</param>
        /// <returns>Piste ruudulla</returns>
        private System.Windows.Point GetRelativePoint(System.Windows.Point p1)
        {
            p1.X = (p1.X - topX) * scaleX;
            p1.Y = (topY - p1.Y) * scaleY;
            return p1;
        }

        /// <summary>
        /// Palauttaa relatiivisen pisteen ruudulla. Vähentää koordinaatista yläkulman ja kertoo sen skaalauksella. 
        /// Tällöin saadaan piste ruudulla.
        /// </summary>
        /// <param name="x">Pisteen X-koordinaatti kartalla</param>
        /// <param name="y">Pisteen Y-koordinaatti kartalla</param>
        /// <returns>Piste ruudulla</returns>
        private System.Windows.Point GetRelativePoint(double x, double y)
        {
            return new System.Windows.Point((x - topX) * scaleX, (topY - y) * scaleY);
        }


        // dx = mm
        // dy = mm
        // 1 m = 1000mm
        /// <summary>
        ///  Palauttaa metrit millimetreistä. Käytetään tekstisiirtojen kanssa
        /// </summary>
        /// <param name="dx">Siirto X-suuntaan</param>
        /// <param name="dy">Siirty Y-suuntaan</param>
        /// <returns></returns>
        private System.Windows.Point GetDxDyPoint(decimal dx, decimal dy)
        {
            return new System.Windows.Point((double)dx / 1000, (double)dy / 1000);
        }
    }
}
