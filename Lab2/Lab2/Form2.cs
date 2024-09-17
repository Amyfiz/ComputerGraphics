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
    using static System.Windows.Forms.AxHost;
    using System.Diagnostics;

    public partial class Form2 : Form
    {
        private Graphics graphics;
        private string path = "../../images/ФРУКТЫ.jpg";
        public Form2()
        {
            InitializeComponent();
            graphics = this.CreateGraphics();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            graphics.Clear(Color.White);

            Bitmap bitmap_orig = new Bitmap("../../../../images/ФРУКТЫ.jpg");
            Bitmap bitmap = new Bitmap(bitmap_orig, new Size((int)(bitmap_orig.Width / 3f), (int)(bitmap_orig.Height / 3f)));
            Bitmap bitmap_red = new Bitmap(bitmap);
            Bitmap bitmap_green = new Bitmap(bitmap);
            Bitmap bitmap_blue = new Bitmap(bitmap);

            int w = bitmap.Width;
            int h = bitmap.Height;
            int start = w;

            graphics.DrawImage(bitmap, start - 225, 20);

            Debug.WriteLine(bitmap.Width * bitmap.Height);

            //ValueTuple<short, short, short>[] arr = new ValueTuple<short, short, short>[width * height];
            int[] arr_r = new int[256];
            int[] arr_g = new int[256];
            int[] arr_b = new int[256];
            int j = 0;

            using (var fastBitmap_red = new FastBitmap(bitmap_red))
            using (var fastBitmap_green = new FastBitmap(bitmap_green))
            using (var fastBitmap_blue = new FastBitmap(bitmap_blue))
            {
                for (var x = 0; x < fastBitmap_red.Width; x++)
                    for (var y = 0; y < fastBitmap_red.Height; y++)
                    {
                        var color = fastBitmap_red[x, y];
                        arr_r[color.R]++;
                        arr_g[color.G]++;
                        arr_b[color.B]++;
                        j++;
                        fastBitmap_red[x, y] = Color.FromArgb(color.R, 0, 0);
                        fastBitmap_green[x, y] = Color.FromArgb(0, color.G, 0);
                        fastBitmap_blue[x, y] = Color.FromArgb(0, 0, color.B);
                    }
            }

            for (int i = 0; i < 256; i++)
            {
                arr_r[i] /= 10;
                arr_g[i] /= 10;
                arr_b[i] /= 10;
            }

            graphics.DrawImage(bitmap_red, start + 50, 20);
            graphics.DrawImage(bitmap_green, start + 325, 20);
            graphics.DrawImage(bitmap_blue, start + 600, 20);

            float boldness = 2f;
            Pen red_pen = new Pen(Color.Red, boldness);
            Pen green_pen = new Pen(Color.Green, boldness);
            Pen blue_pen = new Pen(Color.Blue, boldness);

            int shift_vert = 20;
            int shift_hor = 10;
            int shift_left = 20;

            for (int i = 2; i < 256; i++)
            {
                graphics.DrawLine(red_pen, new Point(shift_left + (i - 1) * shift_hor, this.Height - arr_r[i - 1] - shift_vert), new Point(shift_left + i * shift_hor, this.Height - arr_r[i] - shift_vert));
                graphics.DrawLine(green_pen, new Point(shift_left + (i - 1) * shift_hor, this.Height - arr_g[i - 1] - shift_vert), new Point(shift_left + i * shift_hor, this.Height - arr_g[i] - shift_vert));
                graphics.DrawLine(blue_pen, new Point(shift_left + (i - 1) * shift_hor, this.Height - arr_b[i - 1] - shift_vert), new Point(shift_left + i * shift_hor, this.Height - arr_b[i] - shift_vert));
            }
        }
    }
}
