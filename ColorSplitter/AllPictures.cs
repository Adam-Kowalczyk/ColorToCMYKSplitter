using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorSplitter
{
    public partial class AllPictures : Form
    {
        public AllPictures()
        {
            InitializeComponent();
        }

        public void AddImages(Bitmap[] images)
        {
            pictureBox1.Image = images[0];
            pictureBox2.Image = images[1];
            pictureBox3.Image = images[2];
            pictureBox4.Image = images[3];

        }
    }
}
