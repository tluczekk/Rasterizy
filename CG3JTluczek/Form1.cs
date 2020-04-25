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
        // TODO:
        // - Xiaolin Wu antialiasing algorithm
        // - serialization
        // - moving vertices of polygons
        public Form1()
        {
            InitializeComponent();
            backup = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Dictionary<string, string> lineDict = new Dictionary<string, string>();
            lineDict.Add("1", "Draw");
            lineDict.Add("2", "Move");
            lineDict.Add("3", "Delete");
            lineDict.Add("4", "Thicken");
            lineDict.Add("5", "Recolor");
            comboLine.DataSource = new BindingSource(lineDict, null);
            comboCircle.DataSource = new BindingSource(lineDict, null);
            comboPoly.DataSource = new BindingSource(lineDict, null);
            comboLine.DisplayMember = "Value";
            comboLine.ValueMember = "Key";
            comboCircle.DisplayMember = "Value";
            comboCircle.ValueMember = "Key";
            comboPoly.DisplayMember = "Value";
            comboPoly.ValueMember = "Value";
            panel1.BackColor = Tweakable.col;
        }

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
                    foreach (Line l in lines)
                    {
                        if (l.DDA().Contains(pointToDelete))
                        {
                            lines.Remove(l);
                            redraw();
                            break;
                        }
                    }
                }
                else if (((KeyValuePair<string, string>)comboLine.SelectedItem).Value == "Thicken")
                {
                    Point pointToThicken = new Point(e.X, e.Y);
                    foreach (Line l in lines)
                    {
                        if (l.DDA().Contains(pointToThicken))
                        {
                            l.thickness = Tweakable.thicc;
                            redraw();
                            break;
                        }
                    }
                }
                else if (((KeyValuePair<string, string>)comboLine.SelectedItem).Value == "Recolor")
                {
                    Point pointToRecolor = new Point(e.X, e.Y);
                    foreach (Line l in lines)
                    {
                        if (l.DDA().Contains(pointToRecolor))
                        {
                            l.linCol = Tweakable.col;
                            redraw();
                            break;
                        }
                    }
                }
            } else if (isCircleMode)
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
                            foreach(Point p in c.MidPointCircle(c.radius))
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
                    foreach (Circle c in circles)
                    {
                        if (c.MidPointCircle(c.radius).Contains(pointToDelete))
                        {
                            circles.Remove(c);
                            redraw();
                            break;
                        }
                    }
                }
                else if (((KeyValuePair<string, string>)comboCircle.SelectedItem).Value == "Thicken")
                {
                    Point pointToThicken = new Point(e.X, e.Y);
                    foreach (Circle c in circles)
                    {
                        if (c.MidPointCircle(c.radius).Contains(pointToThicken))
                        {
                            c.thickness = Tweakable.thicc;
                            redraw();
                            break;
                        }
                    }
                }
                else if (((KeyValuePair<string, string>)comboCircle.SelectedItem).Value == "Recolor")
                {
                    Point pointToRecolor = new Point(e.X, e.Y);
                    foreach (Circle c in circles)
                    {
                        if (c.MidPointCircle(c.radius).Contains(pointToRecolor))
                        {
                            c.circleCol = Tweakable.col;
                            redraw();
                            break;
                        }
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
                    if(polyCount == 0)
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
                                    //circles.Remove(c);
                                    polyCount++;
                                    break;
                                }
                            }
                        }
                        polygons.Remove(polyToMove);
                        polyToMove.vertexLines.Clear();
                    }
                    else if(polyCount == 1)
                    {
                        Point newPoint = new Point(e.X, e.Y);
                        polyPair.Add(newPoint);
                        int xVecDiff = polyPair[1].X - polyPair[0].X;
                        int yVecDiff = polyPair[1].Y - polyPair[0].Y;
                        List<Point> temp = new List<Point>();
                        for(int i=0;i< polyToMove.polyVertices.Count;i++)
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
                    foreach (Polygon poly in polygons)
                    {
                        if (poly.allPoints().Contains(pointToDelete))
                        {
                            polygons.Remove(poly);
                            redraw();
                            break;
                        }
                    }
                }
                else if (((KeyValuePair<string, string>)comboPoly.SelectedItem).Value == "Thicken")
                {
                    Point pointToThicken = new Point(e.X, e.Y);
                    foreach (Polygon poly in polygons)
                    {
                        if (poly.allPoints().Contains(pointToThicken))
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
                else if (((KeyValuePair<string, string>)comboPoly.SelectedItem).Value == "Recolor")
                {
                    Point pointToRecolor = new Point(e.X, e.Y);
                    foreach (Polygon poly in polygons)
                    {
                        if (poly.allPoints().Contains(pointToRecolor))
                        {
                            poly.polyColor = Tweakable.col;
                            redraw();
                            break;
                        }
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
                    // XIAOMI CODE HERE
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
                    // XIAOLIN CODE HERE
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
                        // SHAOLIN CODE HERE
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
                            if (linPoly.thickness > 1)
                            {

                                for (int i = 2; i <= linPoly.thickness; i++)
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
            }
            else
            {
                labelAliasing.Text = "ANTIALIASING ON";
                isAliasing = true;
            }
        }

        private void buttonCol_Click(object sender, EventArgs e)
        {
            ColorDialog MyDialog = new ColorDialog();
            MyDialog.AllowFullOpen = false;
            MyDialog.ShowHelp = true;
            MyDialog.Color = Tweakable.col;

            if (MyDialog.ShowDialog() == DialogResult.OK)
            {
                Tweakable.col = MyDialog.Color;
                panel1.BackColor = Tweakable.col;
            }
        }
    }
}
