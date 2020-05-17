using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CG3JTluczek
{
    public class XiaolinWu
    {
        // XIAOLIN WU HELPER FUNCTIONS
        // http://rosettacode.org/wiki/Xiaolin_Wu%27s_line_algorithm#C.23
        public static void plot(Bitmap bitmap, double x, double y, double c, Color baseCol)
        {
            int alpha = (int)(c * 255);
            if (alpha > 255) alpha = 255;
            if (alpha < 0) alpha = 0;
            Color color = Color.FromArgb(alpha, baseCol);
            try
            {
                bitmap.SetPixel((int)x, (int)y, color);
            }
            catch (IndexOutOfRangeException e)
            {
                return;
            }
        }

        public static int ipart(double x) { return (int)x; }

        public static int round(double x) { return ipart(x + 0.5); }

        public static double fpart(double x)
        {
            if (x < 0) return (1 - (x - Math.Floor(x)));
            return (x - Math.Floor(x));
        }

        public static double rfpart(double x)
        {
            return 1 - fpart(x);
        }
        public static double D(int r, int y)
        {
            return Math.Ceiling(Math.Sqrt(r * r - y * y)) - Math.Sqrt(r * r - y * y);
        }
    }
}
