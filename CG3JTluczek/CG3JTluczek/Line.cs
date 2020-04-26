using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CG3JTluczek
{
    [Serializable]
    public class Line
    {
        public Point start;
        public Point end;
        public int thickness;
        public int dx { get { return this.end.X - this.start.X; } }
        public int dy { get { return this.end.Y - this.start.Y; } }
        public Color linCol;
        public Line(Point s, Point e)
        {
            start = s;
            end = e;
            thickness = Tweakable.thicc;
            linCol = Tweakable.col;
        }

        
        // Algorithm version from 
        // https://www.geeksforgeeks.org/dda-line-generation-algorithm-computer-graphics/
        public List<Point> DDA()
        {
            List<Point> linePoints = new List<Point>();
            int step = Math.Abs(this.dx) > Math.Abs(this.dy) ? Math.Abs(this.dx) : Math.Abs(this.dy);

            float xInc = this.dx / (float)step;
            float yInc = this.dy / (float)step;

            float xIter = start.X;
            float yIter = start.Y;

            for(int i =0; i <= step; i++)
            {
                linePoints.Add(new Point((int)xIter, (int)yIter));
                xIter += xInc;
                yIter += yInc;
            }

            return linePoints;
        }
    }
}
