using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorSplitter
{
    public class BezierCurve
    {
        public BezierCurve(DragablePoint p0, DragablePoint p1, DragablePoint p2, DragablePoint p3)
        {
            Points[0] = p0;
            Points[1] = p1;
            Points[2] = p2;
            Points[3] = p3;
            SetValues();
        }
        public DragablePoint[] Points = new DragablePoint[4];

        public CMYKChannel Channel { get; set; }
        public Color CurveColor { get; set; }

        public void Draw(Graphics graphics, bool showPoints = false)
        {
            var pen = new Pen(CurveColor);
            graphics.DrawBezier(pen, Points[0].X, Points[0].Y, Points[1].X, Points[1].Y, Points[2].X, Points[2].Y, Points[3].X, Points[3].Y);
            if (showPoints)
            {
                var pen_circle = new Pen(Color.Black);
                for (int i = 0; i < 4; i++)
                {
                    graphics.DrawEllipse(pen_circle, new Rectangle((int)Points[i].X - 5, (int)Points[i].Y - 5, 10, 10));
                }
            }
        }
        public override string ToString()
        {
            var str = new StringBuilder();
            for(int i=0; i< 4; i++)
            {
                str.Append(Points[i].X + ";");
                str.Append(Points[i].Y + ";");
            }
            return str.ToString();
        }

        public void FromString(string line)
        {
            var values = line.Split(';');
            for(int i =0; i< 4; i++)
            {
                Points[i].X = float.Parse(values[i * 2]);
                Points[i].Y = float.Parse(values[i * 2 + 1]);
            }
            SetValues();
        }

        public List<(double, double)> Values { get; set; }
        public void SetValues()
        {
            Values = new List<(double, double)>();
            double u = 0;
            double d = 0.001;
            Values.Add((Points[0].X, Points[0].Y));
            u = u + d;
            while(u <= 1)
            {
                Values.Add(CalculatePoint(u));
                u = u + d;
            }
        }

        public (double, double) CalculatePoint(double u)
        {
            var p = 1 - u;
            var u2 = u * u;
            var u3 = u2 * u;
            var p2 = p * p;
            var p3 = p2 * p;

            var x = p3 * Points[0].X + 3 * p2 * u * Points[1].X + 3 * p * u2 * Points[2].X + u3 * Points[3].X;
            var y = p3 * Points[0].Y + 3 * p2 * u * Points[1].Y + 3 * p * u2 * Points[2].Y + u3 * Points[3].Y;
            return (x, y);
        }

        public double GetYValue(double p)
        {
            var x = p * (Points[3].X - Points[0].X);
            int a = 0;
            int b = Values.Count - 1;
            while(b - a > 1)
            {
                var c = a + (b - a) / 2;
                if(Values[c].Item1>x)
                {
                    b = c;
                }
                else
                {
                    a = c;
                }
            }
            return (Points[0].Y - Values[b].Item2)/ Points[0].Y;
        }
    }
}
