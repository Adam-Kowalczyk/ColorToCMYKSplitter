using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorSplitter
{
    public class CMYKState
    {
        public CMYKState(int width, int height)
        {
            Boundries = new Rectangle(0, 0, width, height);
            InitializeCurves();
            SelectedCurve = CyanCurve;
        }
        public BezierCurve CyanCurve;
        public BezierCurve MagentaCurve;
        public BezierCurve YellowCurve;
        public BezierCurve BlackCurve;

        public void InitializeCurves()
        {
            CyanCurve = new BezierCurve(new DragablePoint(0,300), new DragablePoint(109, 191), new DragablePoint(201, 64), new DragablePoint(300, 71)) { CurveColor = Color.Cyan, Channel = CMYKChannel.Cyan };
            MagentaCurve = new BezierCurve(new DragablePoint(0, 300), new DragablePoint(128, 217), new DragablePoint(203, 97), new DragablePoint(300, 95)) { CurveColor = Color.Magenta, Channel = CMYKChannel.Magenta };
            YellowCurve = new BezierCurve(new DragablePoint(0, 300), new DragablePoint(145, 209), new DragablePoint(221, 97), new DragablePoint(300, 101)) { CurveColor = Color.Yellow, Channel = CMYKChannel.Yellow };
            BlackCurve = new BezierCurve(new DragablePoint(0, 300), new DragablePoint(254, 300), new DragablePoint(240, 273), new DragablePoint(300, 0)) { CurveColor = Color.Black, Channel = CMYKChannel.Black };
        }

        public Rectangle Boundries;

        public BezierCurve SelectedCurve { get; set; } 

        public void SelectCurve(int i)
        {
            switch(i)
            {
                case 0:
                    {
                        SelectedCurve = CyanCurve;
                    }
                    break;
                case 1:
                    {
                        SelectedCurve = MagentaCurve;
                    }
                    break;
                case 2:
                    {
                        SelectedCurve = YellowCurve;
                    }
                    break;
                case 3:
                    {
                        SelectedCurve = BlackCurve;
                    }
                    break;
                default:
                    {
                        SelectedCurve = CyanCurve;
                    }
                    break;
            }
        }

        public void SaveCurves(string fileName)
        {
            var csv = new StringBuilder();
            csv.AppendLine(CyanCurve.ToString());
            csv.AppendLine(MagentaCurve.ToString());
            csv.AppendLine(YellowCurve.ToString());
            csv.AppendLine(BlackCurve.ToString());
            File.WriteAllText(fileName, csv.ToString());
        }

        public void LoadCurves(string fileName)
        {
            using (var reader = new StreamReader(fileName))
            {
                CyanCurve.FromString(reader.ReadLine());
                MagentaCurve.FromString(reader.ReadLine());
                YellowCurve.FromString(reader.ReadLine());
                BlackCurve.FromString(reader.ReadLine());
            }

        }

        public Bitmap InputImage { get; set; }

        public (int, int, int, int) Convert2CMYK(int r, int g, int b)
        {
            
            var c = 255 - r;
            var m  = 255 - g;
            var y = 255 - b;
            var k = Math.Min(Math.Min(c, m), y);
            c = c - k + (int)(CyanCurve.GetYValue((double)k/ 255) * k);
            m = m - k + (int)(MagentaCurve.GetYValue((double)k / 255) * k);
            y = y - k + (int)(YellowCurve.GetYValue((double)k / 255) * k);
            k = (int)(BlackCurve.GetYValue((double)k / 255) * k);
            return (c, m, y, k);
        }

        public Color ToRGB((int, int, int, int) value, CMYKChannel channel)
        {
            switch (channel)
            {
                case CMYKChannel.Cyan:
                    return Color.FromArgb(255 - value.Item1, 255, 255);
                case CMYKChannel.Magenta:
                    return Color.FromArgb(255, 255 - value.Item2, 255);
                case CMYKChannel.Yellow:
                    return Color.FromArgb(255, 255, 255 - value.Item3);
                case CMYKChannel.Black:
                    return Color.FromArgb(255 - value.Item4, 255 - value.Item4, 255 - value.Item4);
                case CMYKChannel.All:
                    var r = 255 * (1 - (double)value.Item1 / 255) * (1 - (double)value.Item4 / 255);
                    var g = 255 * (1 - (double)value.Item2 / 255) * (1 - (double)value.Item4 / 255);
                    var b = 255 * (1 - (double)value.Item3 / 255) * (1 - (double)value.Item4 / 255);
                    return Color.FromArgb((int)r, (int)g, (int)b);
                default:
                    return Color.FromArgb(255 - value.Item4, 255 - value.Item4, 255 - value.Item4);
            }
        }

        public Bitmap GeneratImage(CMYKChannel channel)
        {
            var newBitmap = new Bitmap(InputImage.Width, InputImage.Height);
            for(int i = 0; i< newBitmap.Width; i++)
            {
                for(int j = 0; j < newBitmap.Height; j++)
                {
                    var pxl = InputImage.GetPixel(i, j);
                    var cmykCol = Convert2CMYK(pxl.R, pxl.G, pxl.B);
                    newBitmap.SetPixel(i, j, ToRGB(cmykCol, channel));
                    
                }
            }
            return newBitmap;
        }

        public Bitmap[] GenerateAllImages()
        {
            var cyanBitmap = new Bitmap(InputImage.Width, InputImage.Height);
            var magentaBitmap = new Bitmap(InputImage.Width, InputImage.Height);
            var yellowBitmap = new Bitmap(InputImage.Width, InputImage.Height);
            var blackBitmap = new Bitmap(InputImage.Width, InputImage.Height);
            for (int i = 0; i < InputImage.Width; i++)
            {
                for (int j = 0; j < InputImage.Height; j++)
                {
                    var pxl = InputImage.GetPixel(i, j);
                    var cmykCol = Convert2CMYK(pxl.R, pxl.G, pxl.B);
                    cyanBitmap.SetPixel(i, j, ToRGB(cmykCol, CMYKChannel.Cyan));
                    magentaBitmap.SetPixel(i, j, ToRGB(cmykCol, CMYKChannel.Magenta));
                    yellowBitmap.SetPixel(i, j, ToRGB(cmykCol, CMYKChannel.Yellow));
                    blackBitmap.SetPixel(i, j, ToRGB(cmykCol, CMYKChannel.Black));
                }
            }
            return new Bitmap[] { cyanBitmap, magentaBitmap, yellowBitmap, blackBitmap };
        }

        public Bitmap GenerateMerged()
        {
            var bitmap = new Bitmap(InputImage.Width, InputImage.Height);
            for (int i = 0; i < InputImage.Width; i++)
            {
                for (int j = 0; j < InputImage.Height; j++)
                {
                    var pxl = InputImage.GetPixel(i, j);
                    var cmykCol = Convert2CMYK(pxl.R, pxl.G, pxl.B);
                    bitmap.SetPixel(i, j, ToRGB(cmykCol, CMYKChannel.All));
                }
            }
            return bitmap;
        }

        public Bitmap GenerateBlackAndWhite()
        {
            var newBitmap = new Bitmap(InputImage.Width, InputImage.Height);
            for (int i = 0; i < newBitmap.Width; i++)
            {
                for (int j = 0; j < newBitmap.Height; j++)
                {
                    var pxl = InputImage.GetPixel(i, j);
                    var avg = (pxl.R + pxl.G + pxl.B) / 3;
                    newBitmap.SetPixel(i, j, Color.FromArgb(255, avg, avg, avg));

                }
            }
            return newBitmap;
        }

        public Bitmap GenerateRGBOctagon(int width,int height)
        {
            var output = new Bitmap(width, height);
            var octagon = new Bitmap(width/2, height/2);
            var graphics = Graphics.FromImage(octagon);
            graphics.FillPolygon(Brushes.Red, new Point[] { new Point(0, 0), new Point(octagon.Width/2, octagon.Height/2), new Point(0, octagon.Height / 2) });
            graphics.FillPolygon(new SolidBrush(Color.FromArgb(0,255,0)), new Point[] { new Point(0, 0), new Point(octagon.Width / 2, octagon.Height / 2), new Point(octagon.Width / 2, 0) });
            graphics.FillPolygon(Brushes.Blue, new Point[] { new Point(octagon.Width, 0), new Point(octagon.Width / 2, octagon.Height / 2), new Point(octagon.Width / 2, 0) });
            graphics.FillPolygon(Brushes.Yellow, new Point[] { new Point(octagon.Width, 0), new Point(octagon.Width / 2, octagon.Height / 2), new Point(octagon.Width, octagon.Height/2) });
            graphics.FillPolygon(Brushes.Magenta, new Point[] { new Point(octagon.Width, octagon.Height), new Point(octagon.Width / 2, octagon.Height / 2), new Point(octagon.Width, octagon.Height / 2) });
            graphics.FillPolygon(Brushes.Cyan, new Point[] { new Point(octagon.Width, octagon.Height), new Point(octagon.Width / 2, octagon.Height / 2), new Point(octagon.Width/2, octagon.Height) });
            graphics.FillPolygon(Brushes.Black, new Point[] { new Point(0, octagon.Height), new Point(octagon.Width / 2, octagon.Height / 2), new Point(octagon.Width / 2, octagon.Height) });
            graphics.FillPolygon(Brushes.White, new Point[] { new Point(0, octagon.Height), new Point(octagon.Width / 2, octagon.Height / 2), new Point(0, octagon.Height/2) });

            var graphics_out = Graphics.FromImage(output);
            graphics_out.DrawImage(octagon, 0, 0);
            var standardGray = ConvertToGrayStandard(octagon);
            graphics_out.DrawImage(standardGray, width/2, 0);
            var oneThirdGray = ConvertToGrayOneThird(octagon);
            graphics_out.DrawImage(oneThirdGray, 0, height / 2);
            var minMaxGray = ConvertToGrayMinMax(octagon);
            graphics_out.DrawImage(minMaxGray, width / 2, height / 2);

            return output;
        }

        public Bitmap ConvertToGrayStandard(Bitmap input)
        {
            var gray = new Bitmap(input.Width, input.Height);
            for(int i = 0; i<gray.Width; i++)
            {
                for(int j = 0; j< gray.Height; j++)
                {
                    var col = input.GetPixel(i, j);
                    var c = col.R * 0.299 + col.G * 0.587 + col.B * 0.114;
                    var converted = Color.FromArgb((int)c,(int) c, (int)c);
                    gray.SetPixel(i, j, converted);
                }
            }
            return gray;
        }

        public Bitmap ConvertToGrayOneThird(Bitmap input)
        {
            var gray = new Bitmap(input.Width, input.Height);
            for (int i = 0; i < gray.Width; i++)
            {
                for (int j = 0; j < gray.Height; j++)
                {
                    var col = input.GetPixel(i, j);
                    var c = (col.R + col.G + col.B )/3;
                    var converted = Color.FromArgb((int)c, (int)c, (int)c);
                    gray.SetPixel(i, j, converted);
                }
            }
            return gray;
        }

        public Bitmap ConvertToGrayMinMax(Bitmap input)
        {
            var gray = new Bitmap(input.Width, input.Height);
            for (int i = 0; i < gray.Width; i++)
            {
                for (int j = 0; j < gray.Height; j++)
                {
                    var col = input.GetPixel(i, j);
                    var c = (Math.Min(col.R,Math.Min(col.G,col.B)) + Math.Max(col.R, Math.Max(col.G, col.B)))/2;
                    var converted = Color.FromArgb((int)c, (int)c, (int)c);
                    gray.SetPixel(i, j, converted);
                }
            }
            return gray;
        }
    }

    public enum CMYKChannel
    {
        Cyan,
        Magenta,
        Yellow,
        Black,
        All
    }
}
