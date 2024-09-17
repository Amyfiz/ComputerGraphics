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
        private int BMWidth;
        private int BMHeight;
        
        Bitmap bitmap;

        private int Hue = 180;
        private int Saturation = 50;
        private int Value = 50;

        private Graphics graphics;

        private string initialImagePath = "../../../images/ФРУКТЫ.jpg";
        private string finalImagePath = "../../../images/RESULT.jpg";

        ValueTuple<double, double, double>[,] HSVData;
        ValueTuple<double, double, double>[,] HSVDataModified;

        double[,] BMHue;
        double[,] BMSaturation;
        double[,] BMValue;

        public Form3()
        {
            InitializeComponent();
            graphics = panel1.CreateGraphics();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bitmap = new Bitmap(initialImagePath);
            BMWidth = bitmap.Width;
            BMHeight = bitmap.Height;

            bitmap = new Bitmap(bitmap, BMWidth, BMHeight);
            graphics.DrawImage(bitmap, (panel1.Height - Height) / 2, (panel1.Width - Width) / 2);

            BMHue = new double[BMWidth, BMHeight];
            BMSaturation = new double[BMWidth, BMHeight];
            BMValue = new double[BMWidth, BMHeight];

            RGB_To_HSV(bitmap);
        }

        private void RGB_To_HSV(Bitmap bitmap)
        {
            using (var fastBitmap = new FastBitmap(bitmap))
                for (var x = 0; x < fastBitmap.Width; x++)
                    for (var y = 0; y < fastBitmap.Height; y++)
                    {
                        var color = fastBitmap[x, y];
                        int max = Math.Max(color.R, Math.Max(color.G, color.B));
                        int min = Math.Min(color.R, Math.Min(color.G, color.B));

                        if (max == 0) { BMSaturation[x, y] = 0; }
                        else { BMSaturation[x, y] = 1d - min / max; }

                        BMValue[x, y] = max / 255d;

                        if (max == min) { BMHue[x, y] = 0; continue; }
                        if (max == color.R && color.G >= color.B) { BMHue[x, y] = 60 * (color.G - color.B) / (max - min); continue; }
                        if (max == color.R && color.G < color.B) { BMHue[x, y] = 60 * (color.G - color.B) / (max - min) + 360; continue; }
                        if (max == color.G) { BMHue[x, y] = 60 * (color.B - color.R) / (max - min) + 120; }
                        if (max == color.B) { BMHue[x, y] = 60 * (color.R - color.G) / (max - min) + 240; }

                    }
        }

        private void HSV_To_RGB(Bitmap bitmap)
        {

            using (var fastBitmap = new FastBitmap(bitmap))
            {
                for (var x = 0; x < fastBitmap.Width; x++)
                    for (var y = 0; y < fastBitmap.Height; y++)
                    {
                        int hi = Convert.ToInt32(Math.Floor(BMHue[x, y] / 60.0)) % 6;
                        double f = BMHue[x, y] / 60d - Math.Floor(BMHue[x, y] / 60.0);

                        double value = BMValue[x, y] * 255d;
                        int v = Convert.ToInt32(value);
                        int p = Convert.ToInt32(value * (1d - BMSaturation[x, y]));
                        int q = Convert.ToInt32(value * (1d - f * BMSaturation[x, y]));
                        int t = Convert.ToInt32(value * (1d - (1d - f) * BMSaturation[x, y]));


                        if (hi == 0)
                            fastBitmap[x, y] = Color.FromArgb(v, t, p);
                        else if (hi == 1)
                            fastBitmap[x, y] = Color.FromArgb(q, v, p);
                        else if (hi == 2)
                            fastBitmap[x, y] = Color.FromArgb(p, v, t);
                        else if (hi == 3)
                            fastBitmap[x, y] = Color.FromArgb(p, q, v);
                        else if (hi == 4)
                            fastBitmap[x, y] = Color.FromArgb(t, p, v);
                        else
                            fastBitmap[x, y] = Color.FromArgb(v, p, q);
                    }
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            int delta = trackBar1.Value - Hue;
            for (int x = 0; x < BMWidth; x++)
                for (int y = 0; y < BMHeight; y++)
                    BMHue[x, y] = (BMHue[x, y] + delta < 360) ? BMHue[x, y] + delta : BMHue[x, y] + delta - 360;

            Hue = trackBar1.Value;

            HSV_To_RGB(bitmap);

            graphics.DrawImage(bitmap, (panel1.Height - BMHeight) / 2, (panel1.Width - BMWidth) / 2);
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            double delta = (trackBar2.Value - Saturation) / 100d;
            for (int x = 0; x < BMSaturation.GetLength(0); x++)
                for (int y = 0; y < BMSaturation.GetLength(1); y++)
                    BMSaturation[x, y] = Math.Min(1, Math.Max(BMSaturation[x, y] + delta, 0d));

            Saturation = trackBar2.Value;

            HSV_To_RGB(bitmap);

            graphics.DrawImage(bitmap, (panel1.Height - BMHeight) / 2, (panel1.Width - BMWidth) / 2);
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            double delta = (trackBar3.Value - Value) / 100d;
            for (int x = 0; x < BMValue.GetLength(0); x++)
                for (int y = 0; y < BMValue.GetLength(1); y++)
                    BMValue[x, y] = Math.Min(1, Math.Max(BMValue[x, y] + delta, 0));

            Value = trackBar3.Value;

            HSV_To_RGB(bitmap);

            graphics.DrawImage(bitmap, (panel1.Height - BMHeight) / 2, (panel1.Width - BMWidth) / 2);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HSV_To_RGB(bitmap);
            bitmap.Save(finalImagePath);
        }
    }
}
