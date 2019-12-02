using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorSplitter
{
    public class DragablePoint
    {
        public DragablePoint(float x, float y)
        {
            X = x;
            Y = y;
        }
        public float X { get; set; }
        public float Y { get; set; }

        public bool IsHit(float x, float y, float dist = 5)
        {
            return Math.Sqrt(Math.Pow((X - x), 2) + Math.Pow((Y - y), 2)) < 5;
        }
    }
}
