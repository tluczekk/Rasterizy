using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CG3JTluczek
{
    [Serializable]
    public class Capsule
    {
        public Point startFocus;
        public Point endFocus;
        public Point onRadius;
        public int radius;
        public List<Line> lines;
        public List<Circle> circles;
        public Color capCol;
        public int thickness;
        public Point sFtemp;
        public Point eFtemp;
        public Capsule(Point sF, Point eF, Point oR)
        {
            this.startFocus = sF;
            this.endFocus = eF;
            this.onRadius = oR;
            lines = new List<Line>();
            circles = new List<Circle>();
            var dist = Math.Sqrt(Math.Pow(this.endFocus.X - this.onRadius.X, 2)
                                            + Math.Pow(this.endFocus.Y - this.onRadius.Y, 2));
            sFtemp = sF;
            eFtemp = eF;
            int tmp;
            if (eFtemp.X < sFtemp.X)
            {
                tmp = sFtemp.X; sFtemp.X = eFtemp.X; eFtemp.X = tmp;
                tmp = sFtemp.Y; sFtemp.Y = eFtemp.Y; eFtemp.Y = tmp;
            }

            Point perp = new Point((eFtemp.Y - sFtemp.Y), -(eFtemp.X - sFtemp.X));
            var perpLength = Math.Sqrt(Math.Pow(perp.X, 2) + Math.Pow(perp.Y, 2));
            perp.X = (int)(perp.X * (dist / perpLength));
            perp.Y = (int)(perp.Y * (dist / perpLength));
            Point a1 = new Point(sFtemp.X - perp.X, sFtemp.Y - perp.Y);
            Point a2 = new Point(sFtemp.X + perp.X, sFtemp.Y + perp.Y);
            Point b2 = new Point(eFtemp.X + perp.X, eFtemp.Y + perp.Y);
            Point b1 = new Point(eFtemp.X - perp.X, eFtemp.Y - perp.Y);
            
            this.lines.Add(new Line(a1, b1));
            this.lines.Add(new Line(a2, b2));
            circles.Add(new Circle(this.startFocus, (int)dist));
            circles.Add(new Circle(this.endFocus, (int)dist));
            capCol = Tweakable.col;
            thickness = Tweakable.thicc;
        }
        public int sign(Point D, Point E, Point F)
        {
            return ((E.X - D.X) * (F.Y - D.Y) - (E.Y - D.Y) * (F.X - D.X));
        }
        
    }
}
