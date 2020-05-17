using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace CG3JTluczek
{
    public static class SutherlandHodgman
    {
        //
        // https://rosettacode.org/wiki/Sutherland-Hodgman_polygon_clipping
        //
        public class Edge
        {
            public Edge(Point from, Point to)
            {
                this.From = from;
                this.To = to;
            }

            public readonly Point From;
            public readonly Point To;
        }

        public static Point[] GetIntersectedPolygon(Point[] subjectPoly, Point[] clipPoly)
        {
            if (subjectPoly.Length < 3 || clipPoly.Length < 3)
            {
                throw new ArgumentException(string.Format("The polygons passed in must have at least 3 points: subject={0}, clip={1}", subjectPoly.Length.ToString(), clipPoly.Length.ToString()));
            }

            List<Point> outputList = subjectPoly.ToList();

            if (!IsClockwise(subjectPoly))
            {
                outputList.Reverse();
            }

            foreach (Edge clipEdge in IterateEdgesClockwise(clipPoly))
            {
                List<Point> inputList = outputList.ToList();	
                outputList.Clear();

                if (inputList.Count == 0)
                {
                    
                    break;
                }

                Point S = inputList[inputList.Count - 1];

                foreach (Point E in inputList)
                {
                    if (IsInside(clipEdge, E))
                    {
                        if (!IsInside(clipEdge, S))
                        {
                            Point? point = GetIntersect(S, E, clipEdge.From, clipEdge.To);
                            if (point == null)
                            {
                                throw new ApplicationException("Line segments don't intersect");
                            }
                            else
                            {
                                outputList.Add(point.Value);
                            }
                        }

                        outputList.Add(E);
                    }
                    else if (IsInside(clipEdge, S))
                    {
                        Point? point = GetIntersect(S, E, clipEdge.From, clipEdge.To);
                        if (point == null)
                        {
                            throw new ApplicationException("Line segments don't intersect");		
                        }
                        else
                        {
                            outputList.Add(point.Value);
                        }
                    }

                    S = E;
                }
            }

            return outputList.ToArray();
        }


        private static IEnumerable<Edge> IterateEdgesClockwise(Point[] polygon)
        {
            if (IsClockwise(polygon))
            {

                for (int cntr = 0; cntr < polygon.Length - 1; cntr++)
                {
                    yield return new Edge(polygon[cntr], polygon[cntr + 1]);
                }

                yield return new Edge(polygon[polygon.Length - 1], polygon[0]);

            }
            else
            {

                for (int cntr = polygon.Length - 1; cntr > 0; cntr--)
                {
                    yield return new Edge(polygon[cntr], polygon[cntr - 1]);
                }

                yield return new Edge(polygon[0], polygon[polygon.Length - 1]);

            }
        }

        private static Point? GetIntersect(Point line1From, Point line1To, Point line2From, Point line2To)
        {
            Point direction1 = new Point(line1To.X - line1From.X, line1To.Y - line1From.Y);
            Point direction2 = new Point(line2To.X - line2From.X, line2To.Y - line2From.Y);
            double dotPerp = (direction1.X * direction2.Y) - (direction1.Y * direction2.X);

            if (IsNearZero(dotPerp))
            {
                return null;
            }

            Point c = new Point(line2From.X - line1From.X, line2From.Y - line1From.Y);
            double t = (c.X * direction2.Y - c.Y * direction2.X) / dotPerp;

            Point temp = new Point(line1From.X + (int)(t * direction1.X), line1From.Y + (int)(t * direction1.Y));
            return temp;
        }

        private static bool IsInside(Edge edge, Point test)
        {
            bool? isLeft = IsLeftOf(edge, test);
            if (isLeft == null)
            {
                return true;
            }

            return !isLeft.Value;
        }
        private static bool IsClockwise(Point[] polygon)
        {
            for (int cntr = 2; cntr < polygon.Length; cntr++)
            {
                bool? isLeft = IsLeftOf(new Edge(polygon[0], polygon[1]), polygon[cntr]);
                if (isLeft != null)		
                {
                    return !isLeft.Value;
                }
            }

            throw new ArgumentException("All the points in the polygon are colinear");
        }

        private static bool? IsLeftOf(Edge edge, Point test)
        {
            Point tmp1 = new Point(edge.To.X - edge.From.X, edge.To.Y - edge.From.Y);
            Point tmp2 = new Point(test.X - edge.To.X, test.Y - edge.To.Y);

            double x = (tmp1.X * tmp2.Y) - (tmp1.Y * tmp2.X);     

            if (x < 0)
            {
                return false;
            }
            else if (x > 0)
            {
                return true;
            }
            else
            {
                return null;
            }
        }

        private static bool IsNearZero(double testValue)
        {
            return Math.Abs(testValue) <= .000000001d;
        }

        
    }
}
