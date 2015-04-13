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

using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Karttailu2.Graphics
{
    public class Settings
    {
        
        // Defaultti DPI 
        public static double GlobalDPI = 96;


        //  Oletusfontti
        public static FontFamily ffLabelFont = new System.Windows.Media.FontFamily("Arial");

        // Kuinka monta astetta fonttia kallistetaan taaksepäin. Windows ei tarjoa suoraan tätä tyyliä, joten tehdään se käsin.
        public static double backSlantedAngle = 10;

        // Fontille valkoinen varjo
        public static DropShadowEffect effectLabelShadow = new DropShadowEffect { Color = Colors.White, Direction = 135, ShadowDepth = 1, Opacity = 1, BlurRadius = 0 };
        public static DropShadowEffect effectLabelShadow2 = new DropShadowEffect { Color = Colors.White, Direction = 315, ShadowDepth = 1, Opacity = 1, BlurRadius = 0 };

        public static SolidColorBrush effectFontForeground2 = Brushes.White;

        public static SolidColorBrush scbRautatieSilta = new SolidColorBrush(Color.FromRgb(8, 73, 75));
        public static SolidColorBrush scbTiesilta = new SolidColorBrush(Color.FromRgb(75, 8, 8));

        /// <summary>
        /// Katkoviivat/pisteet eri luokille.
        /// </summary>
        public static Dictionary<string, DoubleCollection> dcKatkoviivat = new Dictionary<string, DoubleCollection>()
        {
            {"Korkeuskayra25", new DoubleCollection() {28,5} },
            {"Talvitie", new DoubleCollection() { 15, 3 } },
            {"Polku6", new DoubleCollection() { 15, 3 } },
            {"Polku7", new DoubleCollection() { 8.75, 2 }},
            {"Tunneli", new DoubleCollection() { 20, 10 }},
            {"Aita9", new DoubleCollection() { 0.15, 2.5 }},
            {"Luonnonsuojelu19", new DoubleCollection() { 11.75, 2.75 }},
            {"Kuntaraja20", new DoubleCollection() { 10, 1, 1, 1 }},
            {"Jyrkanne21", new DoubleCollection() { 0.2, 0.9 }},
            {"Luiska22", new DoubleCollection() { 0.2, 2.2 }},
            {"Puro31", new DoubleCollection() { 8.75, 2 }},
            {"Suojanne32", new DoubleCollection() { 0.15, 2.2 }},
            {"Koski33", new DoubleCollection() { 0.75, 1.75 }},
            {"Syvyyskayra6", new DoubleCollection() { 14, 5 }},
            {"Syvyyskayra3", new DoubleCollection() { 7, 4 }},
            {"Syvyyskayra1", new DoubleCollection() { 2, 2 }},
            {"Puurivi34", new DoubleCollection() { 0.8, 4.5 }},
            {"Venereitti35", new DoubleCollection() { 8, 14 }},
            {"Rautatie36", new DoubleCollection() { 5, 5 }},
            {"EpamaarainenReunaviiva", new DoubleCollection() { 1.5, 2.5 }},
            {"Pensasaita38", new DoubleCollection() { 0.5, 2.25 }},
            {"SuojaAlue", new DoubleCollection() { 10, 3 }},
            {"PinnanAllaJohto", new DoubleCollection() { 25, 10 }},

        };

    }
}
