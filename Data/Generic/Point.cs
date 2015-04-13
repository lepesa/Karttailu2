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

namespace Karttailu2.Data.Generic
{
    /// <summary>
    /// Toteuttaa 2D pisteen
    /// </summary>
  
    // A point consists of a pair of double-precision coordinates in the order X,Y
    public class Point
    {
        public double X { get; set; }       // little
        public double Y { get; set; }       // little

        public Point()
        {
            X = Y = 0;
        }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Point Clone()
        {
            var po = new Point();
            po.X = this.X;
            po.Y = this.Y;
            return po;
        }

    }
    
}
 
