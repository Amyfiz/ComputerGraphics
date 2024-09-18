using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

namespace Lab2
{
    using FastBitmap;
    public partial class Form1 : Form
    {
        private Graphics graphics;
        private string path = "../../../images/ФРУКТЫ.jpg";
        public Form1()
        {
            InitializeComponent();
            graphics = this.CreateGraphics();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            int[] intensity1 = new int[256];
            int[] intensity2 = new int[256];

            Bitmap bitmap = new Bitmap(path);
            Bitmap bitmap_small = new Bitmap(bitmap, new Size((int)(bitmap.Width / 4), (int)(bitmap.Height / 4)));
            Bitmap bitmap_small2 = new Bitmap(bitmap_small);
            Bitmap bitmap_diff = new Bitmap(bitmap_small.Width, bitmap_small.Height);
            Bitmap gistogram = new Bitmap(bitmap_small.Width * 2, bitmap_small.Height);

            graphics.Clear(Color.White);
            graphics.DrawImage(bitmap_small, 0, 0);


            using (var fastBitmap_gray = new FastBitmap(bitmap_small))
            using (var fastBitmap_gray2 = new FastBitmap(bitmap_small2))
            using (var fastBitmap_diff = new FastBitmap(bitmap_diff))
            {
                for (var x = 0; x < fastBitmap_gray.Width; x++)
                {
                    for (var y = 0; y < fastBitmap_gray.Height; y++)
                    {
                        Point pixel = new Point(x, y);
                        var color = fastBitmap_gray.GetPixel(pixel);

                        int gray = (int)(color.R * 0.299 + color.G * 0.587 + color.B * 0.114);
                        int gray2 = (int)(color.R * 0.2126 + color.G * 0.7152 + color.B * 0.0722);
                        intensity1[gray]++;
                        intensity2[gray2]++;

                        fastBitmap_gray.SetPixel(pixel, Color.FromArgb(gray, gray, gray));
                        fastBitmap_gray2.SetPixel(pixel, Color.FromArgb(gray2, gray2, gray2));
                        int diff = Math.Abs(gray - gray2);

                        Color diffColor = Color.FromArgb(diff, diff, diff);
                        fastBitmap_diff.SetPixel(pixel, diffColor);

                    }
                }
            }


            graphics.DrawImage(bitmap_small, bitmap_small.Width + 10, 0);
            graphics.DrawImage(bitmap_small2, 2 * bitmap_small.Width + 20, 0);
            graphics.DrawImage(bitmap_diff, 3 * bitmap_small.Width + 30, 0);


            int max = Math.Max(intensity1.Max(), intensity2.Max());
            // Определяем коэффициент масштабирования по высоте
            double point = (double)max / bitmap_small.Height;

            // Отрисовываем столбец за столбцом нашу гистограмму с учетом масштаба
            for (int i = 0; i < bitmap_small.Width - 3; ++i)
            {
                for (int j = bitmap_small.Height - 1; j > bitmap_small.Height - intensity1[i / 3] / point; --j)
                {
                    gistogram.SetPixel(i * 2, j, Color.Red);
                    gistogram.SetPixel(i * 2 + 1, j, Color.Red);
                    gistogram.SetPixel(i * 2 + 2, j, Color.Red);
                }
                ++i;
                for (int j = bitmap_small.Height - 1; j > bitmap_small.Height - intensity2[i / 3] / point; --j)
                {
                    gistogram.SetPixel(i * 2, j, Color.Black);
                    gistogram.SetPixel(i * 2 + 1, j, Color.Black);
                    gistogram.SetPixel(i * 2 + 2, j, Color.Black);
                }
                ++i;
            }

            graphics.DrawImage(gistogram, bitmap_small.Width, bitmap_small.Height * 2);
        }
    }
}
