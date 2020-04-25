using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CG3JTluczek
{
    class Circle
    {
        public Point center;
        public int radius;
        public int thickness;
        public Color circleCol;
        public Circle(Point c, int r)
        {
            this.center = c;
            this.radius = r;
            this.thickness = Tweakable.thicc;
            this.circleCol = Tweakable.col;
        }

        public List<Point> MidPointCircle(int rad)
        {
            // version of algorithm found on
            // https://www.geeksforgeeks.org/mid-point-circle-drawing-algorithm/
            List<Point> circlePoints = new List<Point>();
            int dE = 3;
            int dSE = 5 - 2 * rad;
            int d = 1 - rad;
            int x = rad;
            int y = 0;
            // Adding points to vertices
            circlePoints.Add(new Point(center.X, center.Y + rad));
            circlePoints.Add(new Point(center.X, center.Y -rad));
            circlePoints.Add(new Point(center.X + rad, center.Y));
            circlePoints.Add(new Point(center.X -rad, center.Y));
            while (x > y)
            {
                y++;
                if (d <= 0)
                {
                    //d += 2 * y - 1;
                    d += dE;
                    dE += 2;
                    dSE += 2;
                }
                else
                {
                    d += dSE;
                    dE += 2;
                    dSE += 4;
                    x--;
                    //d += 2 * y - 2 * x + 1;
                }
                if (x < y) break;
                // Adding point to every octet of a circle simultaneously
                circlePoints.Add(new Point(x + center.X, y + center.Y));
                circlePoints.Add(new Point(x + center.X, -y + center.Y));
                circlePoints.Add(new Point(-x + center.X, y + center.Y));
                circlePoints.Add(new Point(-x + center.X, -y + center.Y));
                circlePoints.Add(new Point(y + center.X, x + center.Y));
                circlePoints.Add(new Point(y + center.X, -x + center.Y));
                circlePoints.Add(new Point(-y + center.X, x + center.Y));
                circlePoints.Add(new Point(-y + center.X, -x + center.Y));
            }
            return circlePoints;
        }
    }
}
