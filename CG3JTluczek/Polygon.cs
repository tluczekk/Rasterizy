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
        public Color fillCol;
        public List<AETnode> polygonTable = new List<AETnode>();
        private int yMinTemp;
        Dictionary<int, List<AETnode>> edgeTable = new Dictionary<int, List<AETnode>>();
        public Bitmap filImg;
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
            vertexLines.Clear();
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

        // Checking if polygon is convex 
        // http://csharphelper.com/blog/2014/07/determine-whether-a-polygon-is-convex-in-c/
        // Return True if the polygon is convex.
        public bool PolygonIsConvex()
        {
            bool got_negative = false;
            bool got_positive = false;
            int num_points = polyVertices.Count;
            int B, C;
            for (int A = 0; A < num_points; A++)
            {
                B = (A + 1) % num_points;
                C = (B + 1) % num_points;

                float cross_product =
                    CrossProductLength(
                        polyVertices[A].X, polyVertices[A].Y,
                        polyVertices[B].X, polyVertices[B].Y,
                        polyVertices[C].X, polyVertices[C].Y);
                if (cross_product < 0)
                {
                    got_negative = true;
                }
                else if (cross_product > 0)
                {
                    got_positive = true;
                }
                if (got_negative && got_positive) return false;
            }

            return true;
        }
        public static float CrossProductLength(float Ax, float Ay,
            float Bx, float By, float Cx, float Cy)
        {
            float BAx = Ax - Bx;
            float BAy = Ay - By;
            float BCx = Cx - Bx;
            float BCy = Cy - By;

            return (BAx * BCy - BAy * BCx);
        }

        public List<Point> pointsToFill()
        {
            AET tempAET = new AET(this);
            List<AETnode> temp = tempAET.nodes.OrderBy(x => x.yMin).ToList();
            yMinTemp = temp[0].yMin;
            int tempY=yMinTemp;
            while (temp.Count != 0)
            {
                if (temp.Where(x => x.yMin == tempY).Count() != 0)
                {
                    List<AETnode> bucketTable = temp.Where(x => x.yMin == tempY).ToList();
                    temp.RemoveAll(x => x.yMin == tempY);
                    edgeTable.Add(tempY, bucketTable.OrderBy(x => x.xVal).ToList());
                }
                tempY++;
            }
            List<Point> pointsFill = new List<Point>();
            int y = yMinTemp;
            polygonTable.Clear();
            do
            {
                List<AETnode> bucket;
                if (edgeTable.TryGetValue(y, out bucket))
                {
                    polygonTable.AddRange(bucket);

                }
                polygonTable = polygonTable.OrderBy(x => x.xVal).ToList();
                for (int i = 0; i < polygonTable.Count - 1; i += 2)
                {
                    int x = (int)polygonTable[i].xVal;
                    while (x != polygonTable[i + 1].xVal)
                    {
                        pointsFill.Add(new Point(x, y));
                        x++;
                    }
                }
                y++;
                polygonTable.RemoveAll(x => x.yMax == y);
                foreach (AETnode n in polygonTable)
                {
                    n.dummy += n.dx;
                    if (n.dummy > n.dy)
                    {
                        int k;
                        try
                        {
                            k = n.dummy / n.dy;
                        }catch (DivideByZeroException e)
                        {
                            k = int.MaxValue;
                        }
                        n.xVal += k * n.signCoeff;
                        n.dummy -= k * n.dy;
                    }
                }
            }
            while (polygonTable.Count > 0);
            edgeTable.Clear();
            return pointsFill;
        }
    }
}
