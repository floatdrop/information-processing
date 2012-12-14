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
    }
}
