using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test3D4
{
    public class Vector3ClockCompare : IComparer<Vector3>
    {
        public Vector3 center = new Vector3();
        int IComparer<Vector3>.Compare(Vector3 a, Vector3 b)
        {
            if (a.X - center.X >= 0 && b.X - center.X < 0)
                return 1;
            if (a.X - center.X < 0 && b.X - center.X >= 0)
                return -1;
            if (a.X - center.X == 0 && b.X - center.X == 0)
            {
                if (a.Y - center.Y >= 0 || b.Y - center.Y >= 0)
                    return (int)(a.Y - b.Y);
                return (int)(b.Y - a.Y);
            }

            // compute the cross product of vectors (center -> a) x (center -> b)
            var det = (a.X - center.X) * (b.Y - center.Y) - (b.X - center.X) * (a.Y - center.Y);
            if (det < 0)
                return 1;
            if (det > 0)
                return -1;

            // points a and b are on the same line from the center
            // check which point is closer to the center
            var d1 = (a.X - center.X) * (a.X - center.X) + (a.Y - center.Y) * (a.Y - center.Y);
            var d2 = (b.X - center.X) * (b.X - center.X) + (b.Y - center.Y) * (b.Y - center.Y);
            return (int)(d1 - d2);
        }
    }
}
