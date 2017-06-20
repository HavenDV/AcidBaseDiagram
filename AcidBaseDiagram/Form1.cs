using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using AcidBaseLibrary;

namespace AcidBaseDiagram
{
    public partial class Form1 : Form
    {
        static public Color FromRgbNormalized(int red, int green, int blue)
        {
            red = Math.Min(Math.Max(red, 0), 255);
            green = Math.Min(Math.Max(green, 0), 255);
            blue = Math.Min(Math.Max(blue, 0), 255);
            return Color.FromArgb(red, green, blue);
        }

        static public Color DataToColor(Parameters parameters)
        {
            var be = parameters.SBE;
            var bic = parameters.Bic;
            var PCO2 = parameters.PCO2;
            var pH = parameters.pH;

            var blue = (int)(bic * 10.0) % 50 == 0 ? (int)(60 + bic * 4) : 0;
            var green = (int)(be * 10.0) % 10 == 0 ? (int)(180 + be * 2) : 0;
            green = 0;

            return FromRgbNormalized(0, green, blue);//(int)(127 + sbe * 2)
        }

        static public Bitmap GetBitmap(int width, int height)
        {
            var bitmap = new Bitmap(width, height);
            var graphics = Graphics.FromImage(bitmap);
            for (int x = 0; x < width; ++x)
            {
                var PCO2 = 10.0 + 90.0 * x / width;
                for (int y = 0; y < height; ++y)
                {
                    var BE = -30.0 + 60.0 * (height - y) / height;
                    var pH = Diagram.PCO2andBEtoPH(PCO2, BE);
                    var parameters = Parameters.FromPCO2AndPH(PCO2, pH);
                    bitmap.SetPixel(x, y, DataToColor(parameters));
                }
            }

            var font = new Font(FontFamily.GenericSansSerif, 16);
            //Draw SBE diagonals
            var green = Color.FromArgb(0, 255, 0);
            graphics.DrawString("SBE", font, new SolidBrush(green), 0, 0);
            for (var diagonal = -30.0; diagonal < 30.0; diagonal += 10.0)
            {
                var parameters1 = Parameters.FromPCO2AndSBE(40.0, diagonal);
                var y = parameters1.ToY(height);
                graphics.DrawLine(new Pen(green), 0, y, width, y);
                graphics.DrawString(diagonal.ToString(), font, new SolidBrush(green), 0, y);
            }

            //Draw PCO2 diagonals
            graphics.DrawString("PCO2", font, new SolidBrush(Color.Red), width - 70, height - 60);
            for (var diagonal = 10.0; diagonal < 100.0; diagonal += 10.0)
            {
                var parameters1 = Parameters.FromPCO2AndPH(diagonal, 7.4);
                var x = parameters1.ToX(width);
                graphics.DrawLine(new Pen(Color.Red), x, 0, x, height);
                graphics.DrawString(diagonal.ToString(), font, new SolidBrush(Color.Red), x, height - 30);
            }

            //Draw pH diagonals
            for (var diagonal = 7.0; diagonal < 8.0; diagonal += 0.1)
            {
                var parameters1 = Parameters.FromPCO2AndPH(10.0, diagonal);
                var parameters2 = Parameters.FromPCO2AndPH(100.0, diagonal);
                var y1 = parameters1.ToY(height);
                var y2 = parameters2.ToY(height);
                graphics.DrawLine(new Pen(Color.Yellow), 0, y1, height - 1, y2);
                var x2 = parameters2.ToX(width);
                //x2 = y2 < 0 ? (int)(x2 - 0.01 * y2 * (y2 - y1) / 90.0): x2;
                //y2 = Math.Max(y2, 0);
                graphics.DrawString(diagonal.ToString("##.0"), font,
                    new SolidBrush(Color.Yellow),
                    x2 - 40, y2 + 20);
            }

            return bitmap;
        }

        public Form1()
        {
            InitializeComponent();
            pictureBox.Image = GetBitmap(1100, 1100);
        }
    }
}
