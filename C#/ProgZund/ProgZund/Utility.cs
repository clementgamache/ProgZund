using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using FSUtility;

namespace ProgZund
{
    public class Utility
    {
        //converts the point from inch scale to value that fits the screen
        public static System.Drawing.Point CONVERT_XY_REAL_TO_POINT(double x, double y, System.Drawing.Size size)
        {
            System.Drawing.Point p = new System.Drawing.Point();
            p.X = size.Width-1 - (int)(x * size.Width / Machine.TOTAL_WIDTH);
            p.Y = size.Height-1 - (int)(y * size.Height / Machine.TOTAL_HEIGHT);
            return p;
        }
        
        public static System.Windows.Size getPLTSize(string filePath)
        {
            List<PolyLine> polyLines = PolyLine.getPolylinesFromFile(filePath);
            return PolyLine.getSize(polyLines);
        }

        public static double dblEntry(string value)
        {
            double ret = 0;
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrEmpty(value)) return 0;
            if (!double.TryParse(value, out ret))
            {
                throw new Exception("This entry in not a decimal number and should be : " + value);
            }
            return ret;
        }

        public static int intEntry(string value)
        {
            int ret = 0;
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrEmpty(value)) return 0;
            if (!int.TryParse(value, out ret))
            {
                throw new Exception("This entry in not an integer number and should be : " + value);
            }
            return ret;
        }

        public static System.Windows.Rect parseRectangle(string width, string height, string xPos, string yPos)
        {
            double w, h, x, y;
            w = dblEntry(width);
            h = dblEntry(height);
            x = dblEntry(xPos);
            y = dblEntry(yPos);
            return new System.Windows.Rect(x, y, w, h);
        }


        public static string getFileBegin()
        {
            return "PU; PU; SP1; SJ1; PW 170,195,170,195; VS100; VU100; AS4; PM400; TR1; AU30; PA; ";
        }

        public static string getFileEnding()
        {
            return "PA; PU 120000,120000; SP1; ";
        }

        public static Exception NOTNUMERICENTRYEXCEPTION = new Exception("All entries must be numeric");// = new Exception
    }
    
    public class Utility<T> where T : IComparable<T>
    {
        //check if value is between 0 and max
        public static void testMax(T value, T max)
        {
            if (value.CompareTo(max) > 0)
            {
                throw OUTOFBOUNDSEXCEPTION;
            }
        }


        private static Exception OUTOFBOUNDSEXCEPTION = new Exception("Out of bounds");// = new Exception
    }
}
