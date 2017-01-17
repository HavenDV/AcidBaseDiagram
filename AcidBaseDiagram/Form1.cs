using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AcidBaseLibrary;

namespace AcidBaseDiagram
{
    public partial class Form1 : Form
    {
        static public Color DataToColor(Tuple<double, double> data, double pH, double PCO2)
        {
            var sbe = data.Item1;
            var bic = data.Item2;

            var red = (int)(PCO2 * 3.0) % 30 == 0 ? 255 : 0;
            //var red = (int)(pH * 1000.0) % 100 == 0 ? 255 : 0;
            var blue = (int) (bic*10.0)%100 == 0 ? 255 : 0;
            var green = (int)(sbe * 10.0) % 100 == 0 ? 255 : 0;

            return Color.FromArgb(red, green, blue);//(int)(127 + sbe * 2)
        }

        static public Bitmap GetBitmap(int width, int height)
        {
            var bitmap = new Bitmap(width, height);
            var text = string.Empty;
            for (int x = 0; x < width; ++x)
            {
                var PCO2 = 10.0 + 90.0 * x / width;
                for (int y = 0; y < height; ++y)
                {
                    var BE = -30.0 + 60.0 * (height - y) / height;
                    var pH = Diagram.PCO2andBEtoPH(PCO2, BE);
                    var data = Diagram.GetData(pH, PCO2);
                    bitmap.SetPixel(x, y, DataToColor(data, pH, PCO2));
                }
                text += PCO2 + " ";
            }
            Console.WriteLine(text);
            return bitmap;
        }

        public Form1()
        {
            InitializeComponent();
            pictureBox.Image = GetBitmap(400, 400);
        }
    }
}
