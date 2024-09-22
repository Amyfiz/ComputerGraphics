using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab2
{
    using FastBitmap;
    using System.Diagnostics;
    using static System.Windows.Forms.VisualStyles.VisualStyleElement;

    public partial class Form3 : Form
    {
        private Image image;
        private Bitmap initialImage;
        private Bitmap newImage;

        private string initialImagePath = "../../../images/ФРУКТЫ.jpg";
        private string finalImagePath = "../../../images/RESULT.jpg";

        public Form3()
        {
            InitializeComponent();
            image = Image.FromFile(initialImagePath);
            pictureBox1.Image = image;

            initialImage = new Bitmap(image);
            newImage = new Bitmap(initialImage);

            trackBar1.ValueChanged += (s, e) => HSVAdjustments();
            trackBar2.ValueChanged += (s, e) => HSVAdjustments();
            trackBar3.ValueChanged += (s, e) => HSVAdjustments();
        }

        private void HSVAdjustments()
        {
            Bitmap bitmap = new Bitmap(initialImage);

            using (var fastBitmap = new FastBitmap(bitmap))
            {
                var hsvBitmap = fastBitmap.Select(color =>
                {
                    var (hue, saturation, value) = RGBtoHSV(color.R, color.G, color.B);
                    hue = (hue + trackBar1.Value - 180) % 360;

                    if (hue < 0) 
                        hue += 360;

                    saturation = Math.Min(1, Math.Max(saturation + ((trackBar2.Value - 50) / 50d), 0));
                    value = Math.Min(1, Math.Max(value + ((trackBar3.Value - 50) / 50d), 0));

                    return HSVtoRGB(hue, saturation, value);
                });

                newImage = hsvBitmap;
                pictureBox1.Image = newImage;
            }
        }

        private (double hue, double saturation, double value) RGBtoHSV(int r, int g, int b)
        {
            double red = r / 255.0;
            double green = g / 255.0;
            double blue = b / 255.0;

            double max = Math.Max(Math.Max(red, green), blue);
            double min = Math.Min(Math.Min(red, green), blue);

            double delta = max - min;

            double saturation = (max == 0) ? 0 : delta / max;
            double value = max;
            double hue;

            if (delta == 0)
                hue = 0;
            else if (max == red)
                hue = (green - blue) / delta + (green < blue ? 6 : 0);
            else if (max == green)
                hue = (blue - red) / delta + 2;
            else
                hue = (red - green) / delta + 4;
            hue *= 60;

            return (hue, saturation, value);
        }

        private Color HSVtoRGB(double hue, double saturation, double value)
        {
            int hi = (int)Math.Floor(hue / 60) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);
            double p = value * (1 - saturation);
            double q = value * (1 - f * saturation);
            double t = value * (1 - (1 - f) * saturation);

            double redBase, greenBase, blueBase;

            switch (hi)
            {
                case 0:
                    redBase = value;
                    greenBase = t;
                    blueBase = p;
                    break;
                case 1:
                    redBase = q;
                    greenBase = value;
                    blueBase = p;
                    break;
                case 2:
                    redBase = p;
                    greenBase = value;
                    blueBase = t;
                    break;
                case 3:
                    redBase = p;
                    greenBase = q;
                    blueBase = value;
                    break;
                case 4:
                    redBase = t;
                    greenBase = p;
                    blueBase = value;
                    break;
                case 5:
                    redBase = value;
                    greenBase = p;
                    blueBase = q;
                    break;
                default:
                    redBase = greenBase = blueBase = 0;
                    break;
            }

            int R = (int)((redBase * 255) + 0.5);
            int G = (int)((greenBase * 255) + 0.5);
            int B = (int)((blueBase * 255) + 0.5);

            return Color.FromArgb(R, G, B);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            newImage.Save(finalImagePath, System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}
