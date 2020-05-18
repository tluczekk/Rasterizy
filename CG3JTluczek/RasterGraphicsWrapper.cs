using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CG3JTluczek
{
    [Serializable]
    public class RasterGraphicsWrapper
    {
        public List<Line> lines;
        public List<Circle> circles;
        public List<Polygon> polygons;
        public List<Capsule> capsules;
        public List<Rectangle> rectangles;
        public List<Filler> fillers;
        public RasterGraphicsWrapper() { }
        public RasterGraphicsWrapper(List<Line> l, List<Circle> c, List<Polygon> p, List<Capsule> caps,
                                    List<Rectangle> rect, List<Filler> fil)
        {
            lines = new List<Line>(l);
            circles = new List<Circle>(c);
            polygons = new List<Polygon>(p);
            capsules = new List<Capsule>(caps);
            rectangles = new List<Rectangle>(rect);
            fillers = new List<Filler>();
        }
    }
}
