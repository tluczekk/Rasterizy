using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CG3JTluczek
{
    [Serializable]
    public class Filler
    {
        public Point startPoint;
        public Color fillColor;
        public Filler(Point p, Color c)
        {
            this.startPoint = p;
            this.fillColor = c;
        }
    }
}
