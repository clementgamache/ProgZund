using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ProgZund
{
    public class Utility
    {
        public const double TOTALWIDTH = 47.6;
        public const double TOTALHEIGHT = 32.2;
        public const double POINTSPERINCH = 2540.0;
        public static System.Drawing.Point CONVERT_XY_REAL_TO_POINT(double x, double y, System.Drawing.Size size)
        {
            System.Drawing.Point p = new System.Drawing.Point();
            p.X = size.Width-1 - (int)(x * size.Width / TOTALWIDTH);
            p.Y = size.Height-1 - (int)(y * size.Height / TOTALHEIGHT);
            return p;
        }

        public static List<List<System.Windows.Shapes.Line>> getPolylinesFromRectangle(System.Windows.Point p, System.Windows.Size s)
        {
            System.Windows.Shapes.Line line1 = new System.Windows.Shapes.Line();
            System.Windows.Shapes.Line line2 = new System.Windows.Shapes.Line();
            System.Windows.Shapes.Line line3 = new System.Windows.Shapes.Line();
            System.Windows.Shapes.Line line4 = new System.Windows.Shapes.Line();
            line1.X1 = line3.X2 = line4.X1 = line4.X2 = p.X;
            line1.X2 = line2.X1 = line2.X2 = line3.X1 = p.X + s.Width;
            line1.Y1 = line1.Y2 = line2.Y1 = line4.Y2 = p.Y;
            line2.Y2 = line3.Y1 = line3.Y2 = line4.Y1 = p.Y + s.Height;

            return convertLinesToPolylines(new List<System.Windows.Shapes.Line> { line1, line2, line3, line4 });
        }

        public static List<System.Windows.Shapes.Line> convertLineToPolyLine(System.Windows.Shapes.Line l)
        {
            return new List<System.Windows.Shapes.Line> { l };
        }

        public static List<List<System.Windows.Shapes.Line>> convertLinesToPolylines(List<System.Windows.Shapes.Line> lines)
        {
            List<List<System.Windows.Shapes.Line>> polylines = new List<List<System.Windows.Shapes.Line>>();
            foreach(System.Windows.Shapes.Line line in lines)
            {
                polylines.Add(convertLineToPolyLine(line));
            }
            return polylines;
        }

        public static string GET_FILE_BEGINNING()
        {
            return System.IO.File.ReadAllText(System.IO.Directory.GetParent(Directory.GetCurrentDirectory().ToString()).ToString() + @"\BEGIN.txt");
        }

        public static string GET_FILE_ENDING()
        {
            return System.IO.File.ReadAllText(System.IO.Directory.GetParent(Directory.GetCurrentDirectory().ToString()).ToString() + @"\END.txt");
        }

        public static string GET_PORT()
        {
            return System.IO.File.ReadAllText(System.IO.Directory.GetParent(Directory.GetCurrentDirectory().ToString()).ToString() + @"\PORT.txt");
        }

        private static string LINE_TO_PLT(System.Windows.Shapes.Line line, bool following = false)
        {
            string s = "";
            s += following ? "PD" : "PU" + (int)(line.X1 * POINTSPERINCH) + " " + (int)(line.Y1 * POINTSPERINCH) + ";" + Environment.NewLine;
            s += "PD" + (int)(line.X2 * POINTSPERINCH) + " " + (int)(line.Y2 * POINTSPERINCH) + ";" + Environment.NewLine;
            return s;
        }

        public static string POLYLINE_TO_PLT(List<System.Windows.Shapes.Line> lines)
        {
            string s = "";

            s += LINE_TO_PLT(lines[0]);
            lines.Remove(lines[0]);

            foreach (System.Windows.Shapes.Line line in lines)
            {
                s += LINE_TO_PLT(line, true);
            }
            return s;
        }

        public static System.Windows.Size getPLTSize(string filePath)
        {
            double minX, minY, maxX, maxY;

            return new System.Windows.Size(maxX - minX, maxY - minY);
        }

        public static Exception NOTNUMERICENTRYEXCEPTION = new Exception("All entries must be numeric");// = new Exception();
        public static Exception INSIDEISBIGGERTHANOUTSIDEEXCEPTION = new Exception("Inside size should be inside outside size");
        public static Exception OUTSIDEBOUNDSEXCEPTION = new Exception("Cut is outside machine bounds");
    }
}
