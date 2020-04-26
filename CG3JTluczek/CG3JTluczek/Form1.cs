using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CG3JTluczek
{
    public partial class Form1 : Form
    {
        // Whole logic of the program is contained in Mouse_Down method
        // while logic of drawing in redraw()
        public Form1()
        {
            InitializeComponent();
            backup = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Dictionary<string, string> lineDict = new Dictionary<string, string>();
            Dictionary<string, string> polyDict = new Dictionary<string, string>();
            lineDict.Add("1", "Draw");
            lineDict.Add("2", "Move");
            lineDict.Add("3", "Delete");
            lineDict.Add("4", "Thicken");
            lineDict.Add("5", "Recolor");
            polyDict.Add("1", "Draw");
            polyDict.Add("2", "Move");
            polyDict.Add("3", "Delete");
            polyDict.Add("4", "Thicken");
            polyDict.Add("5", "Recolor");
            polyDict.Add("6", "Move vertex");
            comboLine.DataSource = new BindingSource(lineDict, null);
            comboCircle.DataSource = new BindingSource(lineDict, null);
            comboPoly.DataSource = new BindingSource(polyDict, null);
            comboLine.DisplayMember = "Value";
            comboLine.ValueMember = "Key";
            comboCircle.DisplayMember = "Value";
            comboCircle.ValueMember = "Key";
            comboPoly.DisplayMember = "Value";
            comboPoly.ValueMember = "Value";
            panel1.BackColor = Tweakable.col;
        }

        // Global triggers and variables
        private bool isLineMode = false;
        private bool isCircleMode = false;
        private bool isPolygonMode = false;
        private int lineCount = 0;
        private int circleCount = 0;
        private int polyCount = 0;
        private bool isPolygonOngoing = false;
        private Point tempStartVertex;
        private List<Point> tempPolyVertices = new List<Point>();
        private List<Point> linePair = new List<Point>();
        private List<Point> circlePair = new List<Point>();
        private List<Point> polyPair = new List<Point>();
        private Bitmap backup;
        private List<Line> lines = new List<Line>();
        private List<Circle> circles = new List<Circle>();
        private List<Polygon> polygons = new List<Polygon>();
        private Line lineToMove;
        private Circle circleToMove;
        private Polygon polyToMove;
        private Point vertexToMove;
        private Point anchorPoint;
        private bool isAliasing = false;


        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (isLineMode)
            {
                if (((KeyValuePair<string, string>)comboLine.SelectedItem).Value == "Draw")
                {
                    if (lineCount == 0)
                    {
                        linePair.Add(new Point(e.X, e.Y));
                        lineCount++;
                    }
                    else if (lineCount == 1)
                    {
                        linePair.Add(new Point(e.X, e.Y));
                        lineCount = 0;
                        Line temp = new Line(linePair[0], linePair[1]);
                        linePair.Clear();
                        lines.Add(temp);
                        redraw();
                    }
                }
                else if (((KeyValuePair<string, string>)comboLine.SelectedItem).Value == "Move")
                {
                    Point pointToMove = new Point(e.X, e.Y);
                    if (pictureBox1.Image is null)
                    {

                    }
                    if (lineCount == 0)
                    {
                        foreach (Line l in lines)
                        {
                            if (((l.start.X - 5 <= pointToMove.X && l.start.X + 5 >= pointToMove.X)
                                && (l.start.Y - 5 <= pointToMove.Y && l.start.Y + 5 >= pointToMove.Y)) ||
                                ((l.end.X - 5 <= pointToMove.X && l.end.X + 5 >= pointToMove.X)
                                && (l.end.Y - 5 <= pointToMove.Y && l.end.Y + 5 >= pointToMove.Y)))
                            {
                                lineToMove = l;
                                lines.Remove(l);
                                lineCount++;
                                if ((l.start.X - 5 <= pointToMove.X && l.start.X + 5 >= pointToMove.X)
                                && (l.start.Y - 5 <= pointToMove.Y && l.start.Y + 5 >= pointToMove.Y))
                                {
                                    anchorPoint = lineToMove.end;
                                }
                                else
                                {
                                    anchorPoint = lineToMove.start;
                                }
                                break;
                            }

                        }
                    }
                    else if (lineCount == 1)
                    {
                        Point newEndpoint = new Point(e.X, e.Y);
                        if (anchorPoint == lineToMove.start)
                        {
                            lineToMove.end = newEndpoint;
                        }
                        else
                        {
                            lineToMove.start = newEndpoint;
                        }
                        lines.Add(lineToMove);
                        lineToMove = null;
                        lineCount = 0;
                        redraw();
                    }
                }
                else if (((KeyValuePair<string, string>)comboLine.SelectedItem).Value == "Delete")
                {
                    Point pointToDelete = new Point(e.X, e.Y);
                    bool isDone = false;
                    foreach (Line l in lines)
                    {
                        foreach (Point p in l.DDA())
                        {
                            var dist = Math.Sqrt(Math.Pow(pointToDelete.X - p.X, 2)
                                            + Math.Pow(pointToDelete.Y - p.Y, 2));
                            if (dist < 3)
                            {
                                lines.Remove(l);
                                isDone = true;
                                redraw();
                                break;
                            }
                        }
                        if (isDone) { break; }
                    }
                }
                else if (((KeyValuePair<string, string>)comboLine.SelectedItem).Value == "Thicken")
                {
                    Point pointToThicken = new Point(e.X, e.Y);
                    bool isDone = false;
                    foreach (Line l in lines)
                    {
                        foreach (Point p in l.DDA())
                        {
                            var dist = Math.Sqrt(Math.Pow(pointToThicken.X - p.X, 2)
                                            + Math.Pow(pointToThicken.Y - p.Y, 2));
                            if (dist < 3)
                            {
                                l.thickness = Tweakable.thicc;
                                isDone = true;
                                redraw();
                                break;
                            }
                            if (isDone) { break; }
                        }

                    }
                }
                else if (((KeyValuePair<string, string>)comboLine.SelectedItem).Value == "Recolor")
                {
                    Point pointToRecolor = new Point(e.X, e.Y);
                    bool isDone = false;
                    foreach (Line l in lines)
                    {
                        foreach (Point p in l.DDA())
                        {
                            var dist = Math.Sqrt(Math.Pow(pointToRecolor.X - p.X, 2)
                                            + Math.Pow(pointToRecolor.Y - p.Y, 2));
                            if (dist < 3)
                            {
                                l.linCol = Tweakable.col;
                                isDone = true;
                                redraw();
                                break;
                            }
                        }
                        if (isDone) { break; }
                    }
                }
            }
            else if (isCircleMode)
            {
                if (((KeyValuePair<string, string>)comboCircle.SelectedItem).Value == "Draw")
                {
                    if (circleCount == 0)
                    {
                        circlePair.Add(new Point(e.X, e.Y));
                        circleCount++;
                    }
                    else if (circleCount == 1)
                    {
                        circlePair.Add(new Point(e.X, e.Y));
                        circleCount = 0;
                        var dist = Math.Sqrt(Math.Pow(circlePair[1].X - circlePair[0].X, 2)
                                            + Math.Pow(circlePair[1].Y - circlePair[0].Y, 2));
                        Circle temp = new Circle(circlePair[0], (int)dist);
                        circlePair.Clear();
                        circles.Add(temp);
                        redraw();
                    }
                }
                else if (((KeyValuePair<string, string>)comboCircle.SelectedItem).Value == "Move")
                {
                    Point pointToMove = new Point(e.X, e.Y);
                    if (pictureBox1.Image is null)
                    {

                    }
                    if (circleCount == 0)
                    {
                        foreach (Circle c in circles)
                        {
                            foreach (Point p in c.MidPointCircle(c.radius))
                            {
                                var dist = Math.Sqrt(Math.Pow(pointToMove.X - p.X, 2)
                                            + Math.Pow(pointToMove.Y - p.Y, 2));
                                if (dist < 5)
                                {
                                    circleToMove = c;
                                    circleCount++;
                                    break;
                                }
                            }
                        }
                        circles.Remove(circleToMove);
                    }
                    else if (circleCount == 1)
                    {
                        Point newCenter = new Point(e.X, e.Y);
                        circleToMove.center = newCenter;
                        circles.Add(circleToMove);
                        circleToMove = null;
                        circleCount = 0;
                        redraw();
                    }
                }
                else if (((KeyValuePair<string, string>)comboCircle.SelectedItem).Value == "Delete")
                {
                    Point pointToDelete = new Point(e.X, e.Y);
                    bool isDone = false;
                    foreach (Circle c in circles)
                    {
                        foreach (Point p in c.MidPointCircle(c.radius))
                        {
                            var dist = Math.Sqrt(Math.Pow(pointToDelete.X - p.X, 2)
                                                + Math.Pow(pointToDelete.Y - p.Y, 2));
                            if (dist < 3)
                            {
                                circles.Remove(c);
                                isDone = true;
                                redraw();
                                break;
                            }
                        }
                        if (isDone) { break; }
                    }
                }
                else if (((KeyValuePair<string, string>)comboCircle.SelectedItem).Value == "Thicken")
                {
                    Point pointToThicken = new Point(e.X, e.Y);
                    bool isDone = false;
                    foreach (Circle c in circles)
                    {
                        foreach (Point p in c.MidPointCircle(c.radius))
                        {
                            var dist = Math.Sqrt(Math.Pow(pointToThicken.X - p.X, 2)
                                                + Math.Pow(pointToThicken.Y - p.Y, 2));
                            if (dist < 3)
                            {
                                c.thickness = Tweakable.thicc;
                                isDone = true;
                                redraw();
                                break;
                            }
                        }
                        if (isDone) { break; }
                    }
                }
                else if (((KeyValuePair<string, string>)comboCircle.SelectedItem).Value == "Recolor")
                {
                    Point pointToRecolor = new Point(e.X, e.Y);
                    bool isDone = false;
                    foreach (Circle c in circles)
                    {
                        foreach (Point p in c.MidPointCircle(c.radius))
                        {
                            var dist = Math.Sqrt(Math.Pow(pointToRecolor.X - p.X, 2)
                                                + Math.Pow(pointToRecolor.Y - p.Y, 2));
                            if (dist < 3)
                            {
                                c.circleCol = Tweakable.col;
                                isDone = true;
                                redraw();
                                break;
                            }
                        }
                        if (isDone) { break; }
                    }
                }
            }
            else if (isPolygonMode)
            {
                if (((KeyValuePair<string, string>)comboPoly.SelectedItem).Value == "Draw")
                {
                    if (!isPolygonOngoing)
                    {
                        tempStartVertex = new Point(e.X, e.Y);
                        tempPolyVertices.Add(tempStartVertex);
                        isPolygonOngoing = true;
                        Bitmap tempBit;
                        if (pictureBox1.Image is null)
                        {
                            tempBit = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                        }
                        else
                        {
                            tempBit = new Bitmap(pictureBox1.Image);
                        }
                        tempBit.SetPixel(e.X, e.Y, Color.Black);
                        pictureBox1.Image = tempBit;
                    }
                    else
                    {
                        Point pointToAdd = new Point(e.X, e.Y);
                        var dist = Math.Sqrt(Math.Pow(pointToAdd.X - tempStartVertex.X, 2)
                                            + Math.Pow(pointToAdd.Y - tempStartVertex.Y, 2));
                        if (dist < 10)
                        {
                            tempPolyVertices.Add(tempStartVertex);
                            isPolygonOngoing = false;
                            Polygon newpoly = new Polygon();
                            newpoly.startVertex = tempStartVertex;
                            newpoly.polyVertices = new List<Point>(tempPolyVertices);
                            polygons.Add(newpoly);
                            tempPolyVertices.Clear();

                            redraw();
                        }
                        else
                        {
                            tempPolyVertices.Add(new Point(e.X, e.Y));
                        }
                    }
                }
                else if (((KeyValuePair<string, string>)comboPoly.SelectedItem).Value == "Move")
                {
                    Point pointToMove = new Point(e.X, e.Y);
                    if (pictureBox1.Image is null)
                    {

                    }
                    if (polyCount == 0)
                    {
                        polyPair.Add(pointToMove);
                        foreach (Polygon poly in polygons)
                        {
                            foreach (Point p in poly.allPoints())
                            {
                                var dist = Math.Sqrt(Math.Pow(pointToMove.X - p.X, 2)
                                            + Math.Pow(pointToMove.Y - p.Y, 2));
                                if (dist < 5)
                                {
                                    polyToMove = poly;
                                    polyCount++;
                                    break;
                                }
                            }
                        }
                        polygons.Remove(polyToMove);
                        polyToMove.vertexLines.Clear();
                    }
                    else if (polyCount == 1)
                    {
                        Point newPoint = new Point(e.X, e.Y);
                        polyPair.Add(newPoint);
                        int xVecDiff = polyPair[1].X - polyPair[0].X;
                        int yVecDiff = polyPair[1].Y - polyPair[0].Y;
                        List<Point> temp = new List<Point>();
                        for (int i = 0; i < polyToMove.polyVertices.Count; i++)
                        {
                            temp.Add(new Point(polyToMove.polyVertices[i].X + xVecDiff,
                                               polyToMove.polyVertices[i].Y + yVecDiff));
                        }
                        polyToMove.polyVertices = temp;
                        polyToMove.startVertex = new Point(polyToMove.startVertex.X + xVecDiff,
                                                           polyToMove.startVertex.Y + yVecDiff);
                        polygons.Add(polyToMove);
                        polyToMove = null;
                        polyPair.Clear();
                        polyCount = 0;
                        redraw();
                    }

                }
                else if (((KeyValuePair<string, string>)comboPoly.SelectedItem).Value == "Delete")
                {
                    Point pointToDelete = new Point(e.X, e.Y);
                    bool isFound = false;
                    foreach (Polygon poly in polygons)
                    {
                        foreach (Point p in poly.allPoints())
                        {
                            var dist = Math.Sqrt(Math.Pow(pointToDelete.X - p.X, 2)
                                                + Math.Pow(pointToDelete.Y - p.Y, 2));
                            if (dist < 3)
                            {
                                polygons.Remove(poly);
                                redraw();
                                isFound = true;
                                break;
                            }
                        }
                        if (isFound) { break; }
                    }
                }
                else if (((KeyValuePair<string, string>)comboPoly.SelectedItem).Value == "Thicken")
                {
                    Point pointToThicken = new Point(e.X, e.Y);
                    foreach (Polygon poly in polygons)
                    {
                        foreach (Point p in poly.allPoints())
                        {
                            var dist = Math.Sqrt(Math.Pow(pointToThicken.X - p.X, 2)
                                                + Math.Pow(pointToThicken.Y - p.Y, 2));
                            if (dist < 3)
                            {
                                foreach (Line l in poly.vertexLines)
                                {
                                    l.thickness = Tweakable.thicc;
                                }
                                redraw();
                                break;
                            }
                        }

                    }
                }
                else if (((KeyValuePair<string, string>)comboPoly.SelectedItem).Value == "Recolor")
                {
                    Point pointToRecolor = new Point(e.X, e.Y);
                    foreach (Polygon poly in polygons)
                    {
                        foreach (Point v in poly.allPoints())
                        {
                            var dist = Math.Sqrt(Math.Pow(pointToRecolor.X - v.X, 2)
                                                + Math.Pow(pointToRecolor.Y - v.Y, 2));
                            if (dist < 3)
                            {
                                poly.polyColor = Tweakable.col;
                                redraw();
                                break;
                            }
                        }
                    }
                }
                else if (((KeyValuePair<string, string>)comboPoly.SelectedItem).Value == "Move vertex")
                {
                    if (polyCount == 0)
                    {
                        Point pointToRevertex = new Point(e.X, e.Y);
                        foreach (Polygon poly in polygons)
                        {
                            foreach (Point v in poly.polyVertices)
                            {
                                var dist = Math.Sqrt(Math.Pow(pointToRevertex.X - v.X, 2)
                                                + Math.Pow(pointToRevertex.Y - v.Y, 2));
                                if (dist < 5)
                                {
                                    polyToMove = poly;
                                    vertexToMove = v;
                                    polyCount++;
                                    polyToMove.vertexLines.Clear();
                                    break;
                                }
                            }
                        }
                        polygons.Remove(polyToMove);

                    }
                    else if (polyCount == 1)
                    {
                        Point newVertex = new Point(e.X, e.Y);
                        int index = polyToMove.polyVertices.FindIndex(x => x == vertexToMove);
                        polyToMove.polyVertices[index] = newVertex;
                        if (vertexToMove == polyToMove.startVertex)
                        {
                            polyToMove.polyVertices[0] = newVertex;
                            polyToMove.polyVertices[polyToMove.polyVertices.Count - 1] = newVertex;
                        }
                        polygons.Add(polyToMove);
                        polyToMove = null;
                        polyCount = 0;
                        redraw();
                    }
                }
            }      
        }

        public void redraw()
        {
            pictureBox1.Image = backup;
            Bitmap tempBit;
            if (pictureBox1.Image is null)
            {
                tempBit = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            }
            else
            {
                tempBit = new Bitmap(pictureBox1.Image);
            }
            foreach (Line l in lines)
            {
                if (isAliasing)
                {
                    // http://rosettacode.org/wiki/Xiaolin_Wu%27s_line_algorithm#C.23
                    Point tempStart = l.start;
                    Point tempEnd = l.end;
                    bool steep = Math.Abs(tempEnd.Y - tempStart.Y) > Math.Abs(tempEnd.X - tempStart.X);
                    int temp;
                    if (steep)
                    {
                        temp = tempStart.X; tempStart.X = tempStart.Y; tempStart.Y = temp;
                        temp = tempEnd.X; tempEnd.X = tempEnd.Y; tempEnd.Y = temp;
                    }
                    if (tempStart.X > tempEnd.X)
                    {
                        temp = tempStart.X; tempStart.X = tempEnd.X; tempEnd.X = temp;
                        temp = tempStart.Y; tempStart.Y = tempEnd.Y; tempEnd.Y = temp;
                    }

                    double dx = tempEnd.X - tempStart.X;
                    double dy = tempEnd.Y - tempStart.Y;
                    double gradient = dy / dx;

                    double xEnd = round(tempStart.X);
                    double yEnd = tempStart.Y + gradient * (xEnd - tempStart.X);
                    double xGap = rfpart(tempStart.X + 0.5);
                    double xPixel1 = xEnd;
                    double yPixel1 = ipart(yEnd);

                    if (steep)
                    {
                        plot(tempBit, yPixel1, xPixel1, rfpart(yEnd) * xGap);
                        plot(tempBit, yPixel1 + 1, xPixel1, fpart(yEnd) * xGap);
                    }
                    else
                    {
                        plot(tempBit, xPixel1, yPixel1, rfpart(yEnd) * xGap);
                        plot(tempBit, xPixel1, yPixel1 + 1, fpart(yEnd) * xGap);
                    }
                    double intery = yEnd + gradient;

                    xEnd = round(tempEnd.X);
                    yEnd = tempEnd.Y + gradient * (xEnd - tempEnd.X);
                    xGap = fpart(tempEnd.X + 0.5);
                    double xPixel2 = xEnd;
                    double yPixel2 = ipart(yEnd);
                    if (steep)
                    {
                        plot(tempBit, yPixel2, xPixel2, rfpart(yEnd) * xGap);
                        plot(tempBit, yPixel2 + 1, xPixel2, fpart(yEnd) * xGap);
                    }
                    else
                    {
                        plot(tempBit, xPixel2, yPixel2, rfpart(yEnd) * xGap);
                        plot(tempBit, xPixel2, yPixel2 + 1, fpart(yEnd) * xGap);
                    }

                    if (steep)
                    {
                        for (int x = (int)(xPixel1 + 1); x <= xPixel2 - 1; x++)
                        {
                            plot(tempBit, ipart(intery), x, rfpart(intery));
                            plot(tempBit, ipart(intery) + 1, x, fpart(intery));
                            intery += gradient;
                        }
                    }
                    else
                    {
                        for (int x = (int)(xPixel1 + 1); x <= xPixel2 - 1; x++)
                        {
                            plot(tempBit, x, ipart(intery), rfpart(intery));
                            plot(tempBit, x, ipart(intery) + 1, fpart(intery));
                            intery += gradient;
                        }
                    }

                }
                else
                {
                    foreach (Point p in l.DDA())
                    {
                        tempBit.SetPixel(p.X, p.Y, l.linCol);
                        if (l.thickness > 1)
                        {

                            for (int i = 2; i <= l.thickness; i++)
                            {
                                try
                                {
                                    if (Math.Abs(l.dx) >= Math.Abs(l.dy))
                                    {
                                        tempBit.SetPixel(p.X, p.Y + (i - 1), l.linCol);
                                        tempBit.SetPixel(p.X, p.Y - (i - 1), l.linCol);
                                    }
                                    else
                                    {
                                        tempBit.SetPixel(p.X + (i - 1), p.Y, l.linCol);
                                        tempBit.SetPixel(p.X - (i - 1), p.Y, l.linCol);
                                    }
                                }
                                catch (ArgumentOutOfRangeException e)
                                {
                                    continue;
                                }
                            }
                        }
                    }
                }
            }
            foreach(Circle c in circles)
            {
                if (isAliasing)
                {
                    // https://yellowsplash.wordpress.com/2009/10/23/fast-antialiased-circles-and-ellipses-from-xiaolin-wus-concepts/
                    int Lr = c.circleCol.R;
                    int Lg = c.circleCol.G;
                    int Lb = c.circleCol.B;
                    int Br = 255;
                    int Bg = 255;
                    int Bb = 255;
                    int x = c.radius;
                    int y = 0;

                    Bitmap circleBit = new Bitmap(tempBit);

                    try
                    {
                        tempBit.SetPixel(c.center.X + c.radius, c.center.Y + c.radius, c.circleCol);
                    }
                    catch (ArgumentOutOfRangeException e) { }
                    
                    try
                    {
                        tempBit.SetPixel(c.center.X + c.radius, c.center.Y - c.radius, c.circleCol);
                    }
                    catch (ArgumentOutOfRangeException e) { }
                    
                    try
                    {
                        tempBit.SetPixel(c.center.X - c.radius, c.center.Y + c.radius, c.circleCol);
                    }
                    catch (ArgumentOutOfRangeException e) { }
                    
                    try
                    {
                        tempBit.SetPixel(c.center.X - c.radius, c.center.Y - c.radius, c.circleCol);
                    }
                    catch (ArgumentOutOfRangeException e) { }

                    
                    try
                    {
                        tempBit.SetPixel(c.center.X + c.radius, c.center.Y, c.circleCol);
                    }
                    catch (ArgumentOutOfRangeException e) { }
                    
                    try
                    {
                        tempBit.SetPixel(c.center.X, c.center.Y + c.radius, c.circleCol);
                    }
                    catch (ArgumentOutOfRangeException e) { }

                    
                    try
                    {
                        tempBit.SetPixel(c.center.X - c.radius, c.center.Y, c.circleCol);
                    }
                    catch (ArgumentOutOfRangeException e) { }
                    
                    try
                    {
                        tempBit.SetPixel(c.center.X, c.center.Y - c.radius, c.circleCol);
                    }
                    catch (ArgumentOutOfRangeException e) { }

                    while (x >= y)
                    {
                        ++y;

                        x = (int)Math.Ceiling(Math.Sqrt(c.radius * c.radius - y * y));
                        double T = D(c.radius, y);
                        int c2r = (int)(Lr * (1 - T) + Br * T);
                        int c1r = (int)(Lr * T + Br * (1 - T));
                        int c2g = (int)(Lg * (1 - T) + Bg * T);
                        int c1g = (int)(Lg * T + Bg * (1 - T));
                        int c2b = (int)(Lb * (1 - T) + Bb * T);
                        int c1b = (int)(Lb * T + Bb * (1 - T));


                        try
                        {
                            tempBit.SetPixel(c.center.X + x, c.center.Y + y, Color.FromArgb(c2r, c2g, c2b));
                        }
                        catch (ArgumentOutOfRangeException e) { }
                        
                        try
                        {
                            tempBit.SetPixel(c.center.X + x - 1, c.center.Y + y, Color.FromArgb(c1r, c1g, c1b));
                        }
                        catch (ArgumentOutOfRangeException e) { }

                        
                        try
                        {
                            tempBit.SetPixel(c.center.X + y, c.center.Y + x, Color.FromArgb(c2r, c2g, c2b));
                        }
                        catch (ArgumentOutOfRangeException e) { }
                        
                        try
                        {
                            tempBit.SetPixel(c.center.X + y, c.center.Y + x - 1, Color.FromArgb(c1r, c1g, c1b));
                        }
                        catch (ArgumentOutOfRangeException e) { }

                        
                        try
                        {
                            tempBit.SetPixel(c.center.X - x, c.center.Y - y, Color.FromArgb(c2r, c2g, c2b));
                        }
                        catch (ArgumentOutOfRangeException e) { }
                        
                        try
                        {
                            tempBit.SetPixel(c.center.X - x + 1, c.center.Y - y, Color.FromArgb(c1r, c1g, c1b));
                        }
                        catch (ArgumentOutOfRangeException e) { }

                        
                        try
                        {
                            tempBit.SetPixel(c.center.X - y, c.center.Y - x, Color.FromArgb(c2r, c2g, c2b));
                        }
                        catch (ArgumentOutOfRangeException e) { }
                        
                        try
                        {
                            tempBit.SetPixel(c.center.X - y, c.center.Y - x + 1, Color.FromArgb(c1r, c1g, c1b));
                        }
                        catch (ArgumentOutOfRangeException e) { }

                        
                        try
                        {
                            tempBit.SetPixel(c.center.X + x, c.center.Y - y, Color.FromArgb(c2r, c2g, c2b));
                        }
                        catch (ArgumentOutOfRangeException e) { }
                        
                        try
                        {
                            tempBit.SetPixel(c.center.X + x - 1, c.center.Y - y, Color.FromArgb(c1r, c1g, c1b));
                        }
                        catch (ArgumentOutOfRangeException e) { }

                        
                        try
                        {
                            tempBit.SetPixel(c.center.X + y, c.center.Y - x, Color.FromArgb(c2r, c2g, c2b));
                        }
                        catch (ArgumentOutOfRangeException e) { }
                        
                        try
                        {
                            tempBit.SetPixel(c.center.X + y, c.center.Y - x + 1, Color.FromArgb(c1r, c1g, c1b));
                        }
                        catch (ArgumentOutOfRangeException e) { }

                        
                        try
                        {
                            tempBit.SetPixel(c.center.X - x, c.center.Y + y, Color.FromArgb(c2r, c2g, c2b));
                        }
                        catch (ArgumentOutOfRangeException e) { }
                        
                        try
                        {
                            tempBit.SetPixel(c.center.X - x + 1, c.center.Y + y, Color.FromArgb(c1r, c1g, c1b));
                        }
                        catch (ArgumentOutOfRangeException e) { }

                        
                        try
                        {
                            tempBit.SetPixel(c.center.X - y, c.center.Y + x, Color.FromArgb(c2r, c2g, c2b));
                        }
                        catch (ArgumentOutOfRangeException e) { }
                        
                        try
                        {
                            tempBit.SetPixel(c.center.X - y, c.center.Y + x - 1, Color.FromArgb(c1r, c1g, c1b));
                        }
                        catch (ArgumentOutOfRangeException e) { }

                    }
                }
                else
                {
                    foreach (Point p in c.MidPointCircle(c.radius))
                    {
                        try
                        {
                            tempBit.SetPixel(p.X, p.Y, c.circleCol);
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            continue;
                        }

                        if (c.thickness >= 2)
                        {
                            for (int i = 2; i <= c.thickness && i < c.radius - 1; i++)
                            {
                                foreach (Point p2 in c.MidPointCircle(c.radius + 1 - i))
                                {
                                    try
                                    {
                                        tempBit.SetPixel(p2.X, p2.Y, c.circleCol);
                                    }
                                    catch (ArgumentOutOfRangeException e)
                                    {
                                        continue;
                                    }
                                }
                                foreach (Point p3 in c.MidPointCircle(c.radius - 1 + i))
                                {
                                    try
                                    {
                                        tempBit.SetPixel(p3.X, p3.Y, c.circleCol);
                                    }
                                    catch (ArgumentOutOfRangeException e)
                                    {
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            foreach(Polygon poly in polygons)
            {
                foreach (Line linPoly in poly.getPolyLines())
                {
                    if (isAliasing)
                    {
                        Point tempStart = linPoly.start;
                        Point tempEnd = linPoly.end;
                        bool steep = Math.Abs(tempEnd.Y - tempStart.Y) > Math.Abs(tempEnd.X - tempStart.X);
                        int temp;
                        if (steep)
                        {
                            temp = tempStart.X; tempStart.X = tempStart.Y; tempStart.Y = temp;
                            temp = tempEnd.X; tempEnd.X = tempEnd.Y; tempEnd.Y = temp;
                        }
                        if (tempStart.X > tempEnd.X)
                        {
                            temp = tempStart.X; tempStart.X = tempEnd.X; tempEnd.X = temp;
                            temp = tempStart.Y; tempStart.Y = tempEnd.Y; tempEnd.Y = temp;
                        }

                        double dx = tempEnd.X - tempStart.X;
                        double dy = tempEnd.Y - tempStart.Y;
                        double gradient = dy / dx;

                        double xEnd = round(tempStart.X);
                        double yEnd = tempStart.Y + gradient * (xEnd - tempStart.X);
                        double xGap = rfpart(tempStart.X + 0.5);
                        double xPixel1 = xEnd;
                        double yPixel1 = ipart(yEnd);

                        if (steep)
                        {
                            plot(tempBit, yPixel1, xPixel1, rfpart(yEnd) * xGap);
                            plot(tempBit, yPixel1 + 1, xPixel1, fpart(yEnd) * xGap);
                        }
                        else
                        {
                            plot(tempBit, xPixel1, yPixel1, rfpart(yEnd) * xGap);
                            plot(tempBit, xPixel1, yPixel1 + 1, fpart(yEnd) * xGap);
                        }
                        double intery = yEnd + gradient;

                        xEnd = round(tempEnd.X);
                        yEnd = tempEnd.Y + gradient * (xEnd - tempEnd.X);
                        xGap = fpart(tempEnd.X + 0.5);
                        double xPixel2 = xEnd;
                        double yPixel2 = ipart(yEnd);
                        if (steep)
                        {
                            plot(tempBit, yPixel2, xPixel2, rfpart(yEnd) * xGap);
                            plot(tempBit, yPixel2 + 1, xPixel2, fpart(yEnd) * xGap);
                        }
                        else
                        {
                            plot(tempBit, xPixel2, yPixel2, rfpart(yEnd) * xGap);
                            plot(tempBit, xPixel2, yPixel2 + 1, fpart(yEnd) * xGap);
                        }

                        if (steep)
                        {
                            for (int x = (int)(xPixel1 + 1); x <= xPixel2 - 1; x++)
                            {
                                plot(tempBit, ipart(intery), x, rfpart(intery));
                                plot(tempBit, ipart(intery) + 1, x, fpart(intery));
                                intery += gradient;
                            }
                        }
                        else
                        {
                            for (int x = (int)(xPixel1 + 1); x <= xPixel2 - 1; x++)
                            {
                                plot(tempBit, x, ipart(intery), rfpart(intery));
                                plot(tempBit, x, ipart(intery) + 1, fpart(intery));
                                intery += gradient;
                            }
                        }
                    }
                    else
                    {
                        foreach (Point p in linPoly.DDA())
                        {
                            try
                            {
                                tempBit.SetPixel(p.X, p.Y, poly.polyColor);
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                continue;
                            }
                            if (poly.thickness > 1)
                            {

                                for (int i = 2; i <= poly.thickness; i++)
                                {
                                    try
                                    {
                                        if (Math.Abs(linPoly.dx) >= Math.Abs(linPoly.dy))
                                        {
                                            tempBit.SetPixel(p.X, p.Y + (i - 1), poly.polyColor);
                                            tempBit.SetPixel(p.X, p.Y - (i - 1), poly.polyColor);
                                        }
                                        else
                                        {
                                            tempBit.SetPixel(p.X + (i - 1), p.Y, poly.polyColor);
                                            tempBit.SetPixel(p.X - (i - 1), p.Y, poly.polyColor);
                                        }
                                    }
                                    catch (ArgumentOutOfRangeException e)
                                    {
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            pictureBox1.Image = tempBit;
        }

        private void buttonLine_Click(object sender, EventArgs e)
        {
            isCircleMode = false;
            isPolygonMode = false;
            isLineMode = true;
            labelInfo.Text = "LINE   MODE";
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            lines.Clear();
            circles.Clear();
            polygons.Clear();
            pictureBox1.Image = backup;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Tweakable.thicc = (int)numericUpDown1.Value;
        }

        private void buttonCircle_Click(object sender, EventArgs e)
        {
            isCircleMode = true;
            isPolygonMode = false;
            isLineMode = false;
            labelInfo.Text = "CIRCLE MODE";
        }

        private void buttonPoly_Click(object sender, EventArgs e)
        {
            isPolygonMode = true;
            isCircleMode = false;
            isLineMode = false;
            labelInfo.Text = "POLY MODE";
        }

        private void buttonAliasing_Click(object sender, EventArgs e)
        {
            if (isAliasing)
            {
                labelAliasing.Text = "ANTIALIASING OFF";
                isAliasing = false;
                redraw();
            }
            else
            {
                labelAliasing.Text = "ANTIALIASING ON";
                isAliasing = true;
                redraw();
            }
        }

        private void buttonCol_Click(object sender, EventArgs e)
        {
            ColorDialog MyDialog = new ColorDialog();
            MyDialog.AllowFullOpen = true;
            MyDialog.ShowHelp = true;
            MyDialog.Color = Tweakable.col;

            if (MyDialog.ShowDialog() == DialogResult.OK)
            {
                Tweakable.col = MyDialog.Color;
                panel1.BackColor = Tweakable.col;
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            Serializer ser = new Serializer();
            SaveFileDialog saveDialog = new SaveFileDialog();
            RasterGraphicsWrapper ras = new RasterGraphicsWrapper(lines, circles, polygons);
            saveDialog.InitialDirectory = "C:\\Documents";
            saveDialog.Filter = "Raster graphics CG (*.minicg)|*.minicg";
            saveDialog.DefaultExt = "dat";
            saveDialog.AddExtension = true;
            saveDialog.Title = "Save Raster Graphics";
            saveDialog.ShowDialog();

            if(saveDialog.FileName != "")
            {
                ser.Save(saveDialog.FileName, ras);
            }
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            Serializer ser = new Serializer();
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.InitialDirectory = "C:\\Documents";
            openDialog.Filter = "Raster graphics CG (*.minicg)|*.minicg";
            openDialog.FilterIndex = 2;
            openDialog.RestoreDirectory = true;
            if(openDialog.ShowDialog() == DialogResult.OK)
            {
                lines.Clear();
                circles.Clear();
                polygons.Clear();
                RasterGraphicsWrapper ras = ser.Load(openDialog.FileName);
                lines = ras.lines;
                circles = ras.circles;
                polygons = ras.polygons;
                redraw();
            }
            else
            {
                return;
            }
        }

        // XIAOLIN WU HELPER FUNCTIONS
        // http://rosettacode.org/wiki/Xiaolin_Wu%27s_line_algorithm#C.23
        private void plot(Bitmap bitmap, double x, double y, double c)
        {
            int alpha = (int)(c * 255);
            if (alpha > 255) alpha = 255;
            if (alpha < 0) alpha = 0;
            Color color = Color.FromArgb(alpha, Tweakable.col);
            try
            {
                bitmap.SetPixel((int)x, (int)y, color);
            }catch(IndexOutOfRangeException e)
            {
                return;
            }
        }

        int ipart(double x) { return (int)x; }

        int round(double x) { return ipart(x + 0.5); }

        double fpart(double x)
        {
            if (x < 0) return (1 - (x - Math.Floor(x)));
            return (x - Math.Floor(x));
        }

        double rfpart(double x)
        {
            return 1 - fpart(x);
        }
        double D(int r, int y)
        {
            return Math.Ceiling(Math.Sqrt(r * r - y * y)) - Math.Sqrt(r * r - y * y);
        }
    }
}
