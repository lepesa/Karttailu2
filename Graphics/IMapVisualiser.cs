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
using System.Windows.Controls;

namespace Karttailu2.Graphics
{
    /// <summary>
    /// Interface kartan lataamiselle eri tiedostotyypeistä.
    /// </summary>
    public interface IMapVisualiser
    {
        // Toteuttaa kartan piirron Canvakselle
        void DrawMap(Canvas canvas, List<Data.Generic.Layer> layers, DrawingSettings drawSettings, Karttailu2.Data.Generic.MBR mbr, ILog _logger, double cWidth, double cHeight);
        
        // Palauttaa klikatun elementin recordin. Voi palauttaa NULL:n, jos ei haluta toteuttaa/ei voida toteutta
        Karttailu2.Data.Generic.Record GetGraphicsElement(string id);
    }
}