using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CG3JTluczek
{
    [Serializable]
    public class Polygon
    {
        public List<Point> polyVertices;
        public bool isComplete;
        public Point startVertex;
        public List<Line> vertexLines;
        public Color polyColor;
        public int thickness;
        public Polygon() 
        {
            polyVertices = new List<Point>();
            isComplete = false;
            vertexLines = new List<Line>();
            polyColor = Tweakable.col;
            thickness = Tweakable.thicc;
        }
        public List<Point> allPoints()
        {
            List<Point> temp = new List<Point>();
            for(int i = 0; i < polyVertices.Count-1; i++)
            {
                vertexLines.Add(new Line(polyVertices[i], polyVertices[i + 1]));
            }
            foreach(Line l in vertexLines)
            {
                foreach(Point p in l.DDA())
                {
                    temp.Add(p);
                }
            }
            return temp;
        }
        public List<Line> getPolyLines()
        {
            for (int i = 0; i < polyVertices.Count - 1; i++)
            {
                vertexLines.Add(new Line(polyVertices[i], polyVertices[i + 1]));
            }
            return vertexLines;
        }
        // method for temp purpose in Form1.cs
        public void  clearPoly()
        {
            polyVertices.Clear();
            isComplete = false;
            vertexLines.Clear();
        }
    }
}
