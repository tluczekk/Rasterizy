using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CG3JTluczek
{
    [Serializable]
    public class AET
    {
        Polygon polygon;
        public List<AETnode> nodes = new List<AETnode>();
        public List<AETnode> sortedNodes = new List<AETnode>();
        public int minimalY;
        public int maximalY;
        public AET(Polygon poly)
        {
            polygon = poly;
            for (int i = 0; i < polygon.polyVertices.Count - 1; i++)
            {
                nodes.Add(new AETnode(polygon.polyVertices[i].Y, polygon.polyVertices[i + 1].Y,
                                      polygon.polyVertices[i].X, polygon.polyVertices[i + 1].X)); 
            }
            sortedNodes = nodes.Where(x => x.isSlopeZero == false).
                                OrderBy(x => x.yMin).
                                ThenBy(x => x.yMax).
                                ThenBy(x=>x.xVal).ToList();
            minimalY = sortedNodes.Min(x => x.yMin);
            maximalY = sortedNodes.Max(x => x.yMax);
        }
    }
    [Serializable]
    public class AETnode
    {
        public int yMin;
        public int yMax;
        public double xVal;
        public double xMax;
        public int dx;
        public int dy;
        public int signCoeff;
        public int dummy=0;

        public double mInv 
        { 
            get 
            {
                return ((xMax - xVal) / (yMax - yMin)); 
            } 
            set { } 
        }
        public bool isSlopeZero{ get { return (yMax - yMin == 0 ? true : false); } set { } }
        public AETnode(int y1, int y2, int x1, int x2)
        {
            if (y1 < y2)
            {
                yMin = y1;
                yMax = y2;
                xVal = x1;
                xMax = x2;
            }
            else if(y1>y2)
            {
                yMin = y2;
                yMax = y1;
                xVal = x2;
                xMax = x1;
            }
            else
            {
                yMin = yMax = y1;
                xVal = x1 <= x2 ? x1 : x2;
                xMax = x1 <= x2 ? x2 : x1;
            }
            if (xMax > xVal)
            {
                signCoeff = 1;
            }
            else
            {
                signCoeff = -1;
            }
            dx = Math.Abs(x1 - x2);
            dy = Math.Abs(y1 - y2);

            
        }
    }
}
