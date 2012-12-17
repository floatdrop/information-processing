using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auto
{
    class Geometry
    {
        public static Point GetCenter(Rectangle rect)
        {
            return new Point(rect.X + rect.Width/2, rect.Y + rect.Height/2);
        }

        public static double GetDistance(Rectangle rect1, Rectangle rect2)
        {
            var center1 = Geometry.GetCenter(rect1);
            var center2 = Geometry.GetCenter(rect2);
            return Math.Sqrt(Math.Pow(center1.X - center2.X, 2) + Math.Pow(center1.Y - center2.Y, 2));
        }
    }
}
