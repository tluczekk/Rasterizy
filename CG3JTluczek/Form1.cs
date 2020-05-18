using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
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
        // In mouse down, each if-block represents according mode
        // In redraw, each foreach statement represents drawing of one set of shapes at a time
        // Code and manual(README) can be found on my repository:
        // https://github.com/tluczekk/Rasterizy/tree/master
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
            polyDict.Add("7", "Clip");
            comboLine.DataSource = new BindingSource(lineDict, null);
            comboCircle.DataSource = new BindingSource(lineDict, null);
            comboPoly.DataSource = new BindingSource(polyDict, null);
            comboCapsule.DataSource = new BindingSource(lineDict, null);
            comboRectangle.DataSource = new BindingSource(polyDict, null);
            comboLine.DisplayMember = "Value";
            comboLine.ValueMember = "Key";
            comboCircle.DisplayMember = "Value";
            comboCircle.ValueMember = "Key";
            comboPoly.DisplayMember = "Value";
            comboPoly.ValueMember = "Value";
            comboCapsule.DisplayMember = "Value";
            comboCapsule.ValueMember = "Key";
            comboRectangle.DisplayMember = "Value";
            comboRectangle.ValueMember = "Key";
            panel1.BackColor = Tweakable.col;
        }

        // Global triggers and variables
        #region triggers
        private bool isLineMode = false;
        private bool isCircleMode = false;
        private bool isPolygonMode = false;
        private bool isCapsuleMode = false;
        private bool isRectangleMode = false;
        private bool isFillPolyMode = false;
        private bool isFillImgMode = false;
        private bool isFloodFillMode = false;

        private int lineCount = 0;
        private int circleCount = 0;
        private int polyCount = 0;
        private int capsuleCount = 0;
        private int rectangleCount = 0;

        private bool isPolygonOngoing = false;
        private Point tempStartVertex;

        private List<Point> tempPolyVertices = new List<Point>();
        private List<Point> linePair = new List<Point>();
        private List<Point> circlePair = new List<Point>();
        private List<Point> polyPair = new List<Point>();
        private List<Point> capsulePoints = new List<Point>();
        private List<Point> rectanglePair = new List<Point>();

        private Bitmap backup;

        private List<Line> lines = new List<Line>();
        private List<Circle> circles = new List<Circle>();
        private List<Polygon> polygons = new List<Polygon>();
        private List<Capsule> capsules = new List<Capsule>();
        private List<Rectangle> rectangles = new List<Rectangle>();
        private List<Filler> fillers = new List<Filler>();

        private Line lineToMove;
        private Circle circleToMove;
        private Polygon polyToMove;
        private Rectangle rectangleToMove;
        private Point vertexToMove;
        private Point recVertexToMove;
        private Point anchorPoint;

        private bool isAliasing = false;
        public List<Polygon> polyToClip = new List<Polygon>();
        private Bitmap bitToFill;
        #endregion

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            // If either trigger is on, program operates on a given mode
            if (isLineMode)
            {
                drawLine(sender, e);
            }
            else if (isCircleMode)
            {
                drawCircle(sender, e);
            }
            else if (isPolygonMode)
            {
                drawPolygon(sender, e);
            }
            else if (isCapsuleMode)
            {
                drawCapsule(sender, e);
            }
            else if (isRectangleMode)
            {
                drawRectangle(sender, e);
            }
            else if (isFillPolyMode)
            {
                detectFillPoly(sender, e);
            }
            else if (isFillImgMode)
            {
                detectFillPoly(sender, e);
            }
            else if (isFloodFillMode)
            {
                detectFillFlood(sender, e);
            }
        }

        // 
        // METHODS CREATING SHAPES
        //
        #region drawing
        private void drawLine(object sender, MouseEventArgs e)
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

        private void drawCircle(object sender, MouseEventArgs e)
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

        private void drawPolygon(object sender, MouseEventArgs e)
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
                    //polyToMove.vertexLines.Clear();
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
                            poly.thickness = Tweakable.thicc;
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
            else if (((KeyValuePair<string, string>)comboPoly.SelectedItem).Value == "Clip")
            {
                if (polyToClip.Count == 0)
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
                            polyToClip.Add(newpoly);
                            tempPolyVertices.Clear();

                            redraw();
                        }
                        else
                        {
                            tempPolyVertices.Add(new Point(e.X, e.Y));
                        }
                    }
                }
                else if (polyToClip.Count == 1)
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
                            if (!newpoly.PolygonIsConvex())
                            {
                                MessageBox.Show("Polygon is not convex\nTry again", "Error",
                                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                                tempPolyVertices.Clear();
                                return;
                            }
                            polygons.Add(newpoly);
                            polyToClip.Add(newpoly);
                            tempPolyVertices.Clear();

                            // CLIPPING
                            Point[] clippedVertices = SutherlandHodgman.GetIntersectedPolygon
                                            (polyToClip[0].polyVertices.ToArray(),
                                            polyToClip[1].polyVertices.ToArray());
                            Polygon clipped = new Polygon();
                            clipped.polyColor = polyToClip[0].polyColor;
                            clipped.polyVertices = clippedVertices.ToList();
                            clipped.startVertex = clipped.polyVertices[0];
                            clipped.polyVertices.Add(clipped.startVertex);
                            polygons.Remove(polyToClip[0]);
                            polygons.Add(clipped);
                            polyToClip.Clear();

                            redraw();
                        }
                        else
                        {
                            tempPolyVertices.Add(new Point(e.X, e.Y));
                        }
                    }
                }
            }
        }

        private void drawCapsule(object sender, MouseEventArgs e)
        {
            if (((KeyValuePair<string, string>)comboPoly.SelectedItem).Value == "Draw")
            {
                if (capsuleCount == 0 || capsuleCount == 1)
                {
                    capsulePoints.Add(new Point(e.X, e.Y));
                    capsuleCount++;
                }
                else if (capsuleCount == 2)
                {
                    capsulePoints.Add(new Point(e.X, e.Y));
                    capsuleCount = 0;
                    capsules.Add(new Capsule(capsulePoints[0], capsulePoints[1], capsulePoints[2]));
                    capsulePoints.Clear();
                    redraw();
                }
            }
        }

        private void drawRectangle(object sender, MouseEventArgs e)
        {
            if (((KeyValuePair<string, string>)comboRectangle.SelectedItem).Value == "Draw")
            {
                if (rectangleCount == 0)
                {
                    rectanglePair.Add(new Point(e.X, e.Y));
                    rectangleCount++;
                }
                else if (rectangleCount == 1)
                {
                    rectanglePair.Add(new Point(e.X, e.Y));
                    rectangleCount = 0;
                    Rectangle temp = new Rectangle(rectanglePair[0], rectanglePair[1]);
                    rectanglePair.Clear();
                    rectangles.Add(temp);
                    redraw();
                }
            }
            else if (((KeyValuePair<string, string>)comboRectangle.SelectedItem).Value == "Move")
            {
                if (pictureBox1.Image is null)
                {

                }
                if (rectangleCount == 0)
                {
                    Point pointToMove = new Point(e.X, e.Y);
                    rectanglePair.Add(pointToMove);
                    foreach (Rectangle rect in rectangles)
                    {
                        foreach (Point p in rect.allPoints())
                        {
                            var dist = Math.Sqrt(Math.Pow(pointToMove.X - p.X, 2)
                                        + Math.Pow(pointToMove.Y - p.Y, 2));
                            if (dist < 5)
                            {
                                rectangleToMove = rect;
                                rectangleCount++;
                                break;
                            }
                        }
                    }
                    rectangles.Remove(rectangleToMove);
                }
                else if (rectangleCount == 1)
                {
                    Point newPoint = new Point(e.X, e.Y);
                    rectanglePair.Add(newPoint);
                    int xVecDiff = rectanglePair[1].X - rectanglePair[0].X;
                    int yVecDiff = rectanglePair[1].Y - rectanglePair[0].Y;
                    List<Point> temp = new List<Point>();
                    for (int i = 0; i < rectangleToMove.points.Count; i++)
                    {
                        temp.Add(new Point(rectangleToMove.points[i].X + xVecDiff,
                                           rectangleToMove.points[i].Y + yVecDiff));
                    }
                    rectangleToMove.points = temp;
                    rectangleToMove.updateLines();
                    rectangles.Add(rectangleToMove);
                    rectangleToMove = null;
                    rectanglePair.Clear();
                    rectangleCount = 0;
                    redraw();
                }
            }
            else if (((KeyValuePair<string, string>)comboRectangle.SelectedItem).Value == "Delete")
            {
                Point pointToDelete = new Point(e.X, e.Y);
                bool isFound = false;
                foreach (Rectangle rect in rectangles)
                {
                    foreach (Point p in rect.allPoints())
                    {
                        var dist = Math.Sqrt(Math.Pow(pointToDelete.X - p.X, 2)
                                            + Math.Pow(pointToDelete.Y - p.Y, 2));
                        if (dist < 3)
                        {
                            rectangles.Remove(rect);
                            redraw();
                            isFound = true;
                            break;
                        }
                    }
                    if (isFound) { break; }
                }
            }
            else if (((KeyValuePair<string, string>)comboRectangle.SelectedItem).Value == "Thicken")
            {
                Point pointToThicken = new Point(e.X, e.Y);
                foreach (Rectangle rect in rectangles)
                {
                    foreach (Point p in rect.allPoints())
                    {
                        var dist = Math.Sqrt(Math.Pow(pointToThicken.X - p.X, 2)
                                            + Math.Pow(pointToThicken.Y - p.Y, 2));
                        if (dist < 3)
                        {
                            rect.thickness = Tweakable.thicc;
                            redraw();
                            break;
                        }
                    }

                }
            }
            else if (((KeyValuePair<string, string>)comboRectangle.SelectedItem).Value == "Recolor")
            {
                Point pointToRecolor = new Point(e.X, e.Y);
                foreach (Rectangle rect in rectangles)
                {
                    foreach (Point v in rect.allPoints())
                    {
                        var dist = Math.Sqrt(Math.Pow(pointToRecolor.X - v.X, 2)
                                            + Math.Pow(pointToRecolor.Y - v.Y, 2));
                        if (dist < 3)
                        {
                            rect.recColor = Tweakable.col;
                            redraw();
                            break;
                        }
                    }
                }
            }
            else if (((KeyValuePair<string, string>)comboRectangle.SelectedItem).Value == "Move vertex")
            {
                if (rectangleCount == 0)
                {
                    Point pointToRevertex = new Point(e.X, e.Y);
                    foreach (Rectangle rect in rectangles)
                    {
                        foreach (Point v in rect.points)
                        {
                            var dist = Math.Sqrt(Math.Pow(pointToRevertex.X - v.X, 2)
                                            + Math.Pow(pointToRevertex.Y - v.Y, 2));
                            if (dist < 5)
                            {
                                rectangleToMove = rect;
                                recVertexToMove = v;
                                rectangleCount++;
                                //rectangleToMove.lines.Clear();
                                break;
                            }
                        }
                    }
                    rectangles.Remove(rectangleToMove);
                }
                else if (rectangleCount == 1)
                {
                    Point newVertex = new Point(e.X, e.Y);
                    int index = rectangleToMove.points.FindIndex(x => x == recVertexToMove);
                    // index assigned as pivot for compilation purposes
                    // it's going to be changed inside switch statement
                    int pivot = index;

                    switch (index)
                    {
                        case 0:
                            pivot = 2;
                            break;
                        case 1:
                            pivot = 3;
                            break;
                        case 2:
                            pivot = 0;
                            break;
                        case 3:
                            pivot = 1;
                            break;
                        default:
                            break;
                    }
                    for (int i = 0; i < rectangleToMove.points.Count; i++)
                    {
                        if (i == index || i == pivot) continue;
                        if (rectangleToMove.points[i].X == rectangleToMove.points[pivot].X)
                        {
                            Point temp = rectangleToMove.points[i];
                            temp.Y = newVertex.Y;
                            rectangleToMove.points[i] = temp;
                        }
                        else if (rectangleToMove.points[i].Y == rectangleToMove.points[pivot].Y)
                        {
                            Point temp = rectangleToMove.points[i];
                            temp.X = newVertex.X;
                            rectangleToMove.points[i] = temp;
                        }
                    }
                    rectangleToMove.points[index] = newVertex;
                    rectangleToMove.updateLines();
                    rectangles.Add(rectangleToMove);
                    rectangleToMove = null;
                    rectangleCount = 0;
                    redraw();
                }
            }
        }

        private void detectFillPoly(object sender, MouseEventArgs e)
        {
            Point pointToFill = new Point(e.X, e.Y);
            foreach (Polygon poly in polygons)
            {
                foreach (Point p in poly.allPoints())
                {
                    var dist = Math.Sqrt(Math.Pow(pointToFill.X - p.X, 2)
                                            + Math.Pow(pointToFill.Y - p.Y, 2));
                    if (dist < 5)
                    {
                        if (isFillPolyMode)
                        {
                            poly.fillCol = Tweakable.col;
                        }else if (isFillImgMode)
                        {
                            poly.filImg = bitToFill;
                        }
                        redraw();
                        return;
                    }
                }
            }
        }

        private void detectFillFlood(object sender, MouseEventArgs e)
        {
            Point startFiller = new Point(e.X, e.Y);
            Filler tmp = new Filler(startFiller, Tweakable.col);
            fillers.Add(tmp);
            redraw();
        }
        #endregion

        // DRAWING LOGIC
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
            // Drawing each set of shapes
            foreach (Line l in lines)
            {
                if (isAliasing)
                {
                    // Xiaolin Wu algorithm
                    // http://rosettacode.org/wiki/Xiaolin_Wu%27s_line_algorithm#C.23
                    if (l.start == l.end)
                    {
                        tempBit.SetPixel(l.start.X, l.start.Y, l.linCol);
                        continue;
                    }
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

                    double xEnd = XiaolinWu.round(tempStart.X);
                    double yEnd = tempStart.Y + gradient * (xEnd - tempStart.X);
                    double xGap = XiaolinWu.rfpart(tempStart.X + 0.5);
                    double xPixel1 = xEnd;
                    double yPixel1 = XiaolinWu.ipart(yEnd);

                    if (steep)
                    {
                        XiaolinWu.plot(tempBit, yPixel1, xPixel1, XiaolinWu.rfpart(yEnd) * xGap, l.linCol);
                        XiaolinWu.plot(tempBit, yPixel1 + 1, xPixel1, XiaolinWu.fpart(yEnd) * xGap, l.linCol);
                    }
                    else
                    {
                        XiaolinWu.plot(tempBit, xPixel1, yPixel1, XiaolinWu.rfpart(yEnd) * xGap, l.linCol);
                        XiaolinWu.plot(tempBit, xPixel1, yPixel1 + 1, XiaolinWu.fpart(yEnd) * xGap, l.linCol);
                    }
                    double intery = yEnd + gradient;

                    xEnd = XiaolinWu.round(tempEnd.X);
                    yEnd = tempEnd.Y + gradient * (xEnd - tempEnd.X);
                    xGap = XiaolinWu.fpart(tempEnd.X + 0.5);
                    double xPixel2 = xEnd;
                    double yPixel2 = XiaolinWu.ipart(yEnd);
                    if (steep)
                    {
                        XiaolinWu.plot(tempBit, yPixel2, xPixel2, XiaolinWu.rfpart(yEnd) * xGap, l.linCol);
                        XiaolinWu.plot(tempBit, yPixel2 + 1, xPixel2, XiaolinWu.fpart(yEnd) * xGap, l.linCol);
                    }
                    else
                    {
                        XiaolinWu.plot(tempBit, xPixel2, yPixel2, XiaolinWu.rfpart(yEnd) * xGap, l.linCol);
                        XiaolinWu.plot(tempBit, xPixel2, yPixel2 + 1, XiaolinWu.fpart(yEnd) * xGap, l.linCol);
                    }

                    if (steep)
                    {
                        for (int x = (int)(xPixel1 + 1); x <= xPixel2 - 1; x++)
                        {
                            XiaolinWu.plot(tempBit, XiaolinWu.ipart(intery), x, XiaolinWu.rfpart(intery), l.linCol);
                            XiaolinWu.plot(tempBit, XiaolinWu.ipart(intery) + 1, x, XiaolinWu.fpart(intery), l.linCol);
                            intery += gradient;
                        }
                    }
                    else
                    {
                        for (int x = (int)(xPixel1 + 1); x <= xPixel2 - 1; x++)
                        {
                            XiaolinWu.plot(tempBit, x, XiaolinWu.ipart(intery), XiaolinWu.rfpart(intery), l.linCol);
                            XiaolinWu.plot(tempBit, x, XiaolinWu.ipart(intery) + 1, XiaolinWu.fpart(intery), l.linCol);
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
            foreach (Circle c in circles)
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
                        double T = XiaolinWu.D(c.radius, y);
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
            foreach (Polygon poly in polygons)
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

                        double xEnd = XiaolinWu.round(tempStart.X);
                        double yEnd = tempStart.Y + gradient * (xEnd - tempStart.X);
                        double xGap = XiaolinWu.rfpart(tempStart.X + 0.5);
                        double xPixel1 = xEnd;
                        double yPixel1 = XiaolinWu.ipart(yEnd);

                        if (steep)
                        {
                            XiaolinWu.plot(tempBit, yPixel1, xPixel1, XiaolinWu.rfpart(yEnd) * xGap, poly.polyColor);
                            XiaolinWu.plot(tempBit, yPixel1 + 1, xPixel1, XiaolinWu.fpart(yEnd) * xGap, poly.polyColor);
                        }
                        else
                        {
                            XiaolinWu.plot(tempBit, xPixel1, yPixel1, XiaolinWu.rfpart(yEnd) * xGap, poly.polyColor);
                            XiaolinWu.plot(tempBit, xPixel1, yPixel1 + 1, XiaolinWu.fpart(yEnd) * xGap, poly.polyColor);
                        }
                        double intery = yEnd + gradient;

                        xEnd = XiaolinWu.round(tempEnd.X);
                        yEnd = tempEnd.Y + gradient * (xEnd - tempEnd.X);
                        xGap = XiaolinWu.fpart(tempEnd.X + 0.5);
                        double xPixel2 = xEnd;
                        double yPixel2 = XiaolinWu.ipart(yEnd);
                        if (steep)
                        {
                            XiaolinWu.plot(tempBit, yPixel2, xPixel2, XiaolinWu.rfpart(yEnd) * xGap, poly.polyColor);
                            XiaolinWu.plot(tempBit, yPixel2 + 1, xPixel2, XiaolinWu.fpart(yEnd) * xGap, poly.polyColor);
                        }
                        else
                        {
                            XiaolinWu.plot(tempBit, xPixel2, yPixel2, XiaolinWu.rfpart(yEnd) * xGap, poly.polyColor);
                            XiaolinWu.plot(tempBit, xPixel2, yPixel2 + 1, XiaolinWu.fpart(yEnd) * xGap, poly.polyColor);
                        }

                        if (steep)
                        {
                            for (int x = (int)(xPixel1 + 1); x <= xPixel2 - 1; x++)
                            {
                                XiaolinWu.plot(tempBit, XiaolinWu.ipart(intery), x, XiaolinWu.rfpart(intery), poly.polyColor);
                                XiaolinWu.plot(tempBit, XiaolinWu.ipart(intery) + 1, x, XiaolinWu.fpart(intery), poly.polyColor);
                                intery += gradient;
                            }
                        }
                        else
                        {
                            for (int x = (int)(xPixel1 + 1); x <= xPixel2 - 1; x++)
                            {
                                XiaolinWu.plot(tempBit, x, XiaolinWu.ipart(intery), XiaolinWu.rfpart(intery), poly.polyColor);
                                XiaolinWu.plot(tempBit, x, XiaolinWu.ipart(intery) + 1, XiaolinWu.fpart(intery), poly.polyColor);
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
                List<Point> toFill = new List<Point>();
                toFill = poly.pointsToFill();
                if (poly.filImg != null)
                {
                    foreach(Point p in toFill)
                    {
                        tempBit.SetPixel(p.X, p.Y, poly.filImg.GetPixel(p.X % poly.filImg.Width, p.Y % poly.filImg.Height));
                    }
                }
                else if (!poly.fillCol.IsEmpty)
                {
                    
                    foreach (Point p in toFill)
                    {
                        tempBit.SetPixel(p.X, p.Y, poly.fillCol);
                    }
                }
            }
            foreach (Capsule cap in capsules)
            {
                if (isAliasing)
                {
                    foreach (Line l in cap.lines)
                    {
                        if (l.start == l.end)
                        {
                            tempBit.SetPixel(l.start.X, l.start.Y, l.linCol);
                            continue;
                        }
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

                        double xEnd = XiaolinWu.round(tempStart.X);
                        double yEnd = tempStart.Y + gradient * (xEnd - tempStart.X);
                        double xGap = XiaolinWu.rfpart(tempStart.X + 0.5);
                        double xPixel1 = xEnd;
                        double yPixel1 = XiaolinWu.ipart(yEnd);

                        if (steep)
                        {
                            XiaolinWu.plot(tempBit, yPixel1, xPixel1, XiaolinWu.rfpart(yEnd) * xGap, l.linCol);
                            XiaolinWu.plot(tempBit, yPixel1 + 1, xPixel1, XiaolinWu.fpart(yEnd) * xGap, l.linCol);
                        }
                        else
                        {
                            XiaolinWu.plot(tempBit, xPixel1, yPixel1, XiaolinWu.rfpart(yEnd) * xGap, l.linCol);
                            XiaolinWu.plot(tempBit, xPixel1, yPixel1 + 1, XiaolinWu.fpart(yEnd) * xGap, l.linCol);
                        }
                        double intery = yEnd + gradient;

                        xEnd = XiaolinWu.round(tempEnd.X);
                        yEnd = tempEnd.Y + gradient * (xEnd - tempEnd.X);
                        xGap = XiaolinWu.fpart(tempEnd.X + 0.5);
                        double xPixel2 = xEnd;
                        double yPixel2 = XiaolinWu.ipart(yEnd);
                        if (steep)
                        {
                            XiaolinWu.plot(tempBit, yPixel2, xPixel2, XiaolinWu.rfpart(yEnd) * xGap, l.linCol);
                            XiaolinWu.plot(tempBit, yPixel2 + 1, xPixel2, XiaolinWu.fpart(yEnd) * xGap, l.linCol);
                        }
                        else
                        {
                            XiaolinWu.plot(tempBit, xPixel2, yPixel2, XiaolinWu.rfpart(yEnd) * xGap, l.linCol);
                            XiaolinWu.plot(tempBit, xPixel2, yPixel2 + 1, XiaolinWu.fpart(yEnd) * xGap, l.linCol);
                        }

                        if (steep)
                        {
                            for (int x = (int)(xPixel1 + 1); x <= xPixel2 - 1; x++)
                            {
                                XiaolinWu.plot(tempBit, XiaolinWu.ipart(intery), x, XiaolinWu.rfpart(intery), l.linCol);
                                XiaolinWu.plot(tempBit, XiaolinWu.ipart(intery) + 1, x, XiaolinWu.fpart(intery), l.linCol);
                                intery += gradient;
                            }
                        }
                        else
                        {
                            for (int x = (int)(xPixel1 + 1); x <= xPixel2 - 1; x++)
                            {
                                XiaolinWu.plot(tempBit, x, XiaolinWu.ipart(intery), XiaolinWu.rfpart(intery), l.linCol);
                                XiaolinWu.plot(tempBit, x, XiaolinWu.ipart(intery) + 1, XiaolinWu.fpart(intery), l.linCol);
                                intery += gradient;
                            }
                        }
                    }
                    foreach (Circle c in cap.circles)
                    {
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
                            double T = XiaolinWu.D(c.radius, y);
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
                }
                else
                {
                    foreach (Line capL in cap.lines)
                    {
                        foreach (Point p in capL.DDA())
                        {
                            try
                            {
                                tempBit.SetPixel(p.X, p.Y, cap.capCol);
                            }
                            catch (ArgumentOutOfRangeException e) { }
                            if (cap.thickness > 1)
                            {

                                for (int i = 2; i <= cap.thickness; i++)
                                {
                                    try
                                    {
                                        if (Math.Abs(capL.dx) >= Math.Abs(capL.dy))
                                        {
                                            tempBit.SetPixel(p.X, p.Y + (i - 1), cap.capCol);
                                            tempBit.SetPixel(p.X, p.Y - (i - 1), cap.capCol);
                                        }
                                        else
                                        {
                                            tempBit.SetPixel(p.X + (i - 1), p.Y, cap.capCol);
                                            tempBit.SetPixel(p.X - (i - 1), p.Y, cap.capCol);
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
                    foreach (Circle capC in cap.circles)
                    {
                        foreach (Point p in capC.MidPointCircle(capC.radius))
                        {
                            //Point tempE = new Point();
                            //foreach(Line capLi in cap.lines)
                            //{
                            //    if (capC.MidPointCircle(capC.radius).Contains(capLi.start))
                            //    {
                            //        //tempE = capLi.start;
                            //    }else if (capC.MidPointCircle(capC.radius).Contains(capLi.end)) {
                            //        tempE = capLi.end;
                            //        break;
                            //    }
                            //}
                            if (/*cap.sign(capC.center,tempE ,p) > 0*/ true /*
                            ((p.Y - cap.lines[0].start.Y) * (cap.lines[0].end.X - cap.lines[0].start.X) -
                            (cap.lines[0].end.Y - cap.lines[0].start.Y) * (p.X - cap.lines[0].start.X)) *
                            ((p.Y - cap.lines[1].start.Y) * (cap.lines[1].end.X - cap.lines[1].start.X) -
                            (cap.lines[1].end.Y - cap.lines[1].start.Y) * (p.X - cap.lines[1].start.X))>=0*/)
                            {
                                try
                                {
                                    tempBit.SetPixel(p.X, p.Y, cap.capCol);
                                }
                                catch (ArgumentOutOfRangeException e)
                                {
                                    continue;
                                }

                                if (cap.thickness >= 2)
                                {
                                    for (int i = 2; i <= cap.thickness && i < capC.radius - 1; i++)
                                    {
                                        foreach (Point p2 in capC.MidPointCircle(capC.radius + 1 - i))
                                        {
                                            try
                                            {
                                                tempBit.SetPixel(p2.X, p2.Y, cap.capCol);
                                            }
                                            catch (ArgumentOutOfRangeException e)
                                            {
                                                continue;
                                            }
                                        }
                                        foreach (Point p3 in capC.MidPointCircle(capC.radius - 1 + i))
                                        {
                                            try
                                            {
                                                tempBit.SetPixel(p3.X, p3.Y, cap.capCol);
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
                }
            }
            foreach (Rectangle rect in rectangles)
            {
                if (isAliasing)
                {
                    foreach (Line l in rect.lines)
                    {
                        if (l.start == l.end)
                        {
                            tempBit.SetPixel(l.start.X, l.start.Y, rect.recColor);
                            continue;
                        }
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

                        double xEnd = XiaolinWu.round(tempStart.X);
                        double yEnd = tempStart.Y + gradient * (xEnd - tempStart.X);
                        double xGap = XiaolinWu.rfpart(tempStart.X + 0.5);
                        double xPixel1 = xEnd;
                        double yPixel1 = XiaolinWu.ipart(yEnd);

                        if (steep)
                        {
                            XiaolinWu.plot(tempBit, yPixel1, xPixel1, XiaolinWu.rfpart(yEnd) * xGap, rect.recColor);
                            XiaolinWu.plot(tempBit, yPixel1 + 1, xPixel1, XiaolinWu.fpart(yEnd) * xGap, rect.recColor);
                        }
                        else
                        {
                            XiaolinWu.plot(tempBit, xPixel1, yPixel1, XiaolinWu.rfpart(yEnd) * xGap, rect.recColor);
                            XiaolinWu.plot(tempBit, xPixel1, yPixel1 + 1, XiaolinWu.fpart(yEnd) * xGap, rect.recColor);
                        }
                        double intery = yEnd + gradient;

                        xEnd = XiaolinWu.round(tempEnd.X);
                        yEnd = tempEnd.Y + gradient * (xEnd - tempEnd.X);
                        xGap = XiaolinWu.fpart(tempEnd.X + 0.5);
                        double xPixel2 = xEnd;
                        double yPixel2 = XiaolinWu.ipart(yEnd);
                        if (steep)
                        {
                            XiaolinWu.plot(tempBit, yPixel2, xPixel2, XiaolinWu.rfpart(yEnd) * xGap, rect.recColor);
                            XiaolinWu.plot(tempBit, yPixel2 + 1, xPixel2, XiaolinWu.fpart(yEnd) * xGap, rect.recColor);
                        }
                        else
                        {
                            XiaolinWu.plot(tempBit, xPixel2, yPixel2, XiaolinWu.rfpart(yEnd) * xGap, rect.recColor);
                            XiaolinWu.plot(tempBit, xPixel2, yPixel2 + 1, XiaolinWu.fpart(yEnd) * xGap, rect.recColor);
                        }

                        if (steep)
                        {
                            for (int x = (int)(xPixel1 + 1); x <= xPixel2 - 1; x++)
                            {
                                XiaolinWu.plot(tempBit, XiaolinWu.ipart(intery), x, XiaolinWu.rfpart(intery), rect.recColor);
                                XiaolinWu.plot(tempBit, XiaolinWu.ipart(intery) + 1, x, XiaolinWu.fpart(intery), rect.recColor);
                                intery += gradient;
                            }
                        }
                        else
                        {
                            for (int x = (int)(xPixel1 + 1); x <= xPixel2 - 1; x++)
                            {
                                XiaolinWu.plot(tempBit, x, XiaolinWu.ipart(intery), XiaolinWu.rfpart(intery), rect.recColor);
                                XiaolinWu.plot(tempBit, x, XiaolinWu.ipart(intery) + 1, XiaolinWu.fpart(intery), rect.recColor);
                                intery += gradient;
                            }
                        }
                    }
                }
                else
                {
                    foreach (Line l in rect.lines)
                    {
                        foreach (Point p in l.DDA())
                        {
                            try
                            {
                                tempBit.SetPixel(p.X, p.Y, rect.recColor);
                            } catch (ArgumentOutOfRangeException e) { }
                            if (rect.thickness > 1)
                            {

                                for (int i = 2; i <= rect.thickness; i++)
                                {
                                    try
                                    {
                                        if (Math.Abs(l.dx) >= Math.Abs(l.dy))
                                        {
                                            tempBit.SetPixel(p.X, p.Y + (i - 1), rect.recColor);
                                            tempBit.SetPixel(p.X, p.Y - (i - 1), rect.recColor);
                                        }
                                        else
                                        {
                                            tempBit.SetPixel(p.X + (i - 1), p.Y, rect.recColor);
                                            tempBit.SetPixel(p.X - (i - 1), p.Y, rect.recColor);
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
            foreach(Filler f in fillers)
            {
                tempBit = floodFill(tempBit, f.startPoint, f.fillColor);
            }
            pictureBox1.Image = tempBit;
        }

        public Bitmap floodFill(Bitmap bit, Point p, Color col)
        {
            if(p.Y < 0 || p.Y > bit.Height - 1 || p.X < 0 || p.X > bit.Width - 1)
            {
                throw new Exception("Point not inside bitmap");
            }
            Bitmap filling = new Bitmap(bit);
            Color seed = filling.GetPixel(p.X, p.Y);
            Stack<Point> pointsStack = new Stack<Point>();
            pointsStack.Push(p);
            while(pointsStack.Count > 0)
            {
                Point r = pointsStack.Pop();
                if (r.Y < 0 || r.Y > bit.Height - 1 || r.X < 0 || r.X > bit.Width - 1)
                {
                    continue;
                }
                Color pixCol = filling.GetPixel(r.X, r.Y);
                if (pixCol == seed)
                {
                    filling.SetPixel(r.X, r.Y, col);
                    pointsStack.Push(new Point(r.X + 1, r.Y));
                    pointsStack.Push(new Point(r.X - 1, r.Y));
                    pointsStack.Push(new Point(r.X, r.Y + 1));
                    pointsStack.Push(new Point(r.X, r.Y - 1));
                }
            }
            return filling;

        }


        //
        // BUTTON METHODS
        //
        #region buttons
        private void buttonLine_Click(object sender, EventArgs e)
        {
            isCircleMode = false;
            isPolygonMode = false;
            isLineMode = true;
            isCapsuleMode = false;
            isFillPolyMode = false;
            isFillImgMode = false;
            isRectangleMode = false;
            isFloodFillMode = false;
            labelInfo.Text = "LINE   MODE";
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            lines.Clear();
            circles.Clear();
            polygons.Clear();
            capsules.Clear();
            rectangles.Clear();
            fillers.Clear();
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
            isCapsuleMode = false;
            isRectangleMode = false;
            isFillPolyMode = false;
            isFillImgMode = false;
            isFloodFillMode = false;
            labelInfo.Text = "CIRCLE MODE";
        }

        private void buttonPoly_Click(object sender, EventArgs e)
        {
            isPolygonMode = true;
            isCircleMode = false;
            isLineMode = false;
            isCapsuleMode = false;
            isRectangleMode = false;
            isFillPolyMode = false;
            isFillImgMode = false;
            isFloodFillMode = false;
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
            RasterGraphicsWrapper ras = new RasterGraphicsWrapper(lines, circles, polygons, capsules, rectangles, fillers);
            saveDialog.InitialDirectory = "C:\\Documents";
            saveDialog.Filter = "Raster graphics CG (*.minicg)|*.minicg";
            saveDialog.DefaultExt = "dat";
            saveDialog.AddExtension = true;
            saveDialog.Title = "Save Raster Graphics";
            saveDialog.ShowDialog();

            if (saveDialog.FileName != "")
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
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                lines.Clear();
                circles.Clear();
                polygons.Clear();
                capsules.Clear();
                rectangles.Clear();
                fillers.Clear();
                RasterGraphicsWrapper ras = ser.Load(openDialog.FileName);
                lines = ras.lines;
                circles = ras.circles;
                polygons = ras.polygons;
                capsules = ras.capsules;
                rectangles = ras.rectangles;
                fillers = ras.fillers;
                redraw();
            }
            else
            {
                return;
            }
        }

        private void buttonCapsule_Click(object sender, EventArgs e)
        {
            isPolygonMode = false;
            isCircleMode = false;
            isLineMode = false;
            isRectangleMode = false;
            isCapsuleMode = true;
            isFillPolyMode = false;
            isFillImgMode = false;
            isFloodFillMode = false;
            labelInfo.Text = "CAPSULE MODE";
        }

        private void buttonRectangle_Click(object sender, EventArgs e)
        {
            isPolygonMode = false;
            isCircleMode = false;
            isLineMode = false;
            isCapsuleMode = false;
            isRectangleMode = true;
            isFillPolyMode = false;
            isFillImgMode = false;
            isFloodFillMode = false;
            labelInfo.Text = "RECT MODE";
        }

        private void buttonFill_Click(object sender, EventArgs e)
        {
            isPolygonMode = false;
            isCircleMode = false;
            isLineMode = false;
            isFillImgMode = false;
            isCapsuleMode = false;
            isRectangleMode = false;
            isFillPolyMode = true;
            isFloodFillMode = false;
            labelInfo.Text = "FILL MODE";
        }

        private void buttonFillImg_Click(object sender, EventArgs e)
        {
            isLineMode = false;
            isCircleMode = false;
            isPolygonMode = false;
            isCapsuleMode = false;
            isRectangleMode = false;
            isFillPolyMode = false;
            isFillImgMode = true;
            isFloodFillMode = false;
            labelInfo.Text = "POLY MODE";
            openFile();

        }
        private void openFile()
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp, *.png)|*.jpg; *.jpeg; *.gif; *.bmp; *.png";
            if (open.ShowDialog() == DialogResult.OK)
            {
                bitToFill = new Bitmap(open.FileName);
            }
        }
        

        private void buttonManual_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/tluczekk/Rasterizy/blob/master/README.md");
        }

        private void buttonFloodFill_Click(object sender, EventArgs e)
        {
            isPolygonMode = false;
            isCircleMode = false;
            isLineMode = false;
            isFillImgMode = false;
            isCapsuleMode = false;
            isRectangleMode = false;
            isFillPolyMode = false;
            isFloodFillMode = true;
            labelInfo.Text = "FILL MODE";
        }
        #endregion
    }
}
