using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorSplitter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            State = new CMYKState(300, 300);
            LoadDefaultImage();

        }

        public CMYKState State;

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawLine(new Pen(Color.Black), 0, 0, 0, State.Boundries.Height);
            e.Graphics.DrawLine(new Pen(Color.Black), 0, State.Boundries.Height, State.Boundries.Width, State.Boundries.Height);
            if (!checkBox1.Checked)
            {
                State.SelectedCurve.Draw(e.Graphics, true);
            }
            else
            {
                State.CyanCurve.Draw(e.Graphics, State.SelectedCurve == State.CyanCurve);
                State.MagentaCurve.Draw(e.Graphics, State.SelectedCurve == State.MagentaCurve);
                State.YellowCurve.Draw(e.Graphics, State.SelectedCurve == State.YellowCurve);
                State.BlackCurve.Draw(e.Graphics, State.SelectedCurve == State.BlackCurve);
            }

        }


        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                State.SelectCurve(0);
                pictureBox1.Invalidate();
                UpdateOutputImage();
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                State.SelectCurve(1);
                pictureBox1.Invalidate();
                UpdateOutputImage();
            }
                
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                State.SelectCurve(2);
                pictureBox1.Invalidate();
                UpdateOutputImage();
            }
                
        }
        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
            {
                State.SelectCurve(3);
                pictureBox1.Invalidate();
                UpdateOutputImage();
            }
        }

        DragablePoint point = null;
        bool isOnEdge = true;
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (point != null) return;
            for(int i = 1; i < 4; i++)
            {
                if(State.SelectedCurve.Points[i].IsHit(e.X, e.Y))
                {
                    point = State.SelectedCurve.Points[i];
                    isOnEdge = i == 3;
                    break;
                }
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (point == null) return;
            if (!isOnEdge)
                point.X = e.X;
            if (point.X < State.Boundries.Left)
                point.X = State.Boundries.Left;
            if (point.X > State.Boundries.Right)
                point.X = State.Boundries.Right;
            point.Y = e.Y;
            if (point.Y < State.Boundries.Top)
                point.Y = State.Boundries.Top;
            if (point.Y > State.Boundries.Bottom)
                point.Y = State.Boundries.Bottom;
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (point == null) return;
            if(!isOnEdge)
                point.X = e.X;
            if (point.X < State.Boundries.Left)
                point.X = State.Boundries.Left;
            if (point.X > State.Boundries.Right)
                point.X = State.Boundries.Right;
            point.Y = e.Y;
            if (point.Y < State.Boundries.Top)
                point.Y = State.Boundries.Top;
            if (point.Y > State.Boundries.Bottom)
                point.Y = State.Boundries.Bottom;
            point = null;
            isOnEdge = true;
            pictureBox1.Invalidate();
            State.SelectedCurve.SetValues();
            UpdateOutputImage();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            pictureBox1.Invalidate();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "CSV file|*.csv";
            sfd.Title = "Save Curves";
            sfd.ShowDialog();
            if (sfd.FileName != "")
            {
                State.SaveCurves(sfd.FileName);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "CSV file|*.csv";
            ofd.Title = "Load Curves";
            ofd.ShowDialog();
            if (ofd.FileName != "")
            {
                State.LoadCurves(ofd.FileName);
                pictureBox1.Invalidate();
                UpdateOutputImage();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            // image filters  
            ofd.Filter = "Image Files(*.jpg; *.jpeg; *.bmp)|*.jpg; *.jpeg; *.bmp";
            ofd.ShowDialog();
            if (ofd.FileName != "")
            {
                // display image in picture box  
                State.InputImage = new Bitmap(Image.FromFile(ofd.FileName), pictureBox2.Size);
                pictureBox2.Image = State.InputImage;

                UpdateOutputImage();
            }
        }

        public void UpdateOutputImage()
        {
            if (State.InputImage == null) return;
            pictureBox3.Image = State.GeneratImage(State.SelectedCurve.Channel);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (State.InputImage == null) return;
            pictureBox3.Image = State.GenerateBlackAndWhite();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (State.InputImage == null) return;
            var form = new AllPictures();
            form.AddImages(State.GenerateAllImages());
            form.Show();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (State.InputImage == null) return;
            var sfd = new SaveFileDialog();
            sfd.ShowDialog();
            if(sfd.FileName != "")
            {
                var imgs = State.GenerateAllImages();
                imgs[0].Save(sfd.FileName + "_Cyan.jpg", ImageFormat.Jpeg);
                imgs[1].Save(sfd.FileName + "_Magenta.jpg", ImageFormat.Jpeg);
                imgs[2].Save(sfd.FileName + "_Yellow.jpg", ImageFormat.Jpeg);
                imgs[3].Save(sfd.FileName + "_Black.jpg", ImageFormat.Jpeg);
                
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string fileName = @"..\..\PredefinedCurves\0percBack.csv";
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, fileName);
            if (File.Exists(path))
            {
                State.LoadCurves(path);
                pictureBox1.Invalidate();
                UpdateOutputImage();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string fileName = @"..\..\PredefinedCurves\100percBack.csv";
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, fileName);
            if (File.Exists(path))
            {
                State.LoadCurves(path);
                pictureBox1.Invalidate();
                UpdateOutputImage();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string fileName = @"..\..\PredefinedCurves\UCR.csv";
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, fileName);
            if (File.Exists(path))
            {
                State.LoadCurves(path);
                pictureBox1.Invalidate();
                UpdateOutputImage();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string fileName = @"..\..\PredefinedCurves\GCR.csv";
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, fileName);
            if (File.Exists(path))
            {
                State.LoadCurves(path);
                pictureBox1.Invalidate();
                UpdateOutputImage();
            }
        }

        private void LoadDefaultImage()
        {
            string fileName = @"..\..\cat.jpg";
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, fileName);
            if (File.Exists(path))
            {
                State.InputImage = new Bitmap(Image.FromFile(path), pictureBox2.Size);
                pictureBox2.Image = State.InputImage;

                UpdateOutputImage();
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            var bmp = State.GenerateRGBOctagon(pictureBox2.Width, pictureBox2.Height);
            pictureBox2.Image = bmp;
            State.InputImage = bmp;
            UpdateOutputImage();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (State.InputImage == null) return;
            pictureBox3.Image = State.GenerateMerged();
        }
    }
}
