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

namespace Karttailu2.Graphics
{
    /// <summary>
    /// Factory-toteutus eri piirtotavoille. Nyt vasta WPF UI element
    /// </summary>
    public static class MapVisualiserFactory
    {
        public enum MapVisualiserType
        {
            GenericWPF,
            FastWPF
        };

        /// <summary>
        /// Palauttaa olion, joka toteuttaa kartan piirron tietyllä tavalla Canvakselle.
        /// </summary>
        /// <param name="type">Piirtotyyppi</param>
        /// <returns>IVisualiser interface toteuttavan olion</returns>
        public static IMapVisualiser GetMapVisualiser(MapVisualiserType type)
        {
            if (type == MapVisualiserType.GenericWPF)
            {
                return new Graphics.GenericWpf.MapVisualiser();
            }

            if (type == MapVisualiserType.FastWPF)
            {
                return new Graphics.Wpf2.MapVisualiser();
            }

            throw new Exception("Visualiser not supported.");
        }
    }
}