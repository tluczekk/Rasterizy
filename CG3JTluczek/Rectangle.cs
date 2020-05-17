using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CG3JTluczek
{
    [Serializable]
    public class Rectangle
    {
        //public Point[] points = new Point[4];
        public List<Point> points = new List<Point>();
        public int thickness;
        public Color recColor;
        public List<Line> lines = new List<Line>();

        public Rectangle(Point first, Point second)
        {
            int diffX = second.X - first.X;
            int diffY = second.Y - first.Y;
            //points[0] = first;
            //points[1] = new Point(first.X, first.Y + diffY);
            //points[2] = second;
            //points[3] = new Point(first.X + diffX, first.Y); 
            points.Add(first);
            points.Add(new Point(first.X, first.Y + diffY));
            points.Add(second);
            points.Add(new Point(first.X + diffX, first.Y));
            this.updateLines();
            recColor = Tweakable.col;
            thickness = Tweakable.thicc;
        }
        public List<Point> allPoints()
        {
            List<Point> temp = new List<Point>();
            foreach(Line l in this.lines)
            {
                foreach(Point p in l.DDA())
                {
                    temp.Add(p);
                }
            }

            return temp;
        }
        public void updateLines()
        {
            if (lines.Count != 0) { lines.Clear(); }
            lines.Add(new Line(points[0], points[1]));
            lines.Add(new Line(points[1], points[2]));
            lines.Add(new Line(points[2], points[3]));
            lines.Add(new Line(points[3], points[0]));
        }
    }
}
