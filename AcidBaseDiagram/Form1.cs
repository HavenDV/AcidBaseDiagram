using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AcidBaseDiagram
{
    public partial class Form1 : Form
    {
        static public Bitmap GetBitmap()
        {
            return new Bitmap(400, 400);
        }

        public Form1()
        {
            InitializeComponent();
            pictureBox.Image = GetBitmap();
        }
    }
}
