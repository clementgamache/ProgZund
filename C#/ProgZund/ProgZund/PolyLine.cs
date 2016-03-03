using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProgZund
{
    public class PolyLine
    {
        public PolyLine()
        {
            lines_ = new List<System.Windows.Shapes.Line>();
        }
        public PolyLine(List<System.Windows.Shapes.Line> lines)
        {
            lines_ = new List<System.Windows.Shapes.Line>();
            foreach (System.Windows.Shapes.Line line in lines)
            {
                if (lines_.Count == 0)
                {
                    lines_.Add(line);
                }
                else if (
                    lines_.Last().X2 == line.X1 &&
                    lines_.Last().Y2 == line.Y1)
                {
                    lines_.Add(line);
                }
                else
                {
                    throw new Exception("Entry is not a polyLine");
                }
            }
        }
        public PolyLine(System.Windows.Shapes.Line line)
        {
            lines_ = new List<System.Windows.Shapes.Line>(new System.Windows.Shapes.Line[] { line });
        }
        
        public string getPlt()
        {
            string s = "";
            string opCode = "PU";
            double modifier = Machine.POINTSPERINCH;
            foreach (System.Windows.Shapes.Line line in lines_)
            {
                s += opCode + (int)(line.X1*modifier) + " " + (int)(line.Y1*modifier) + ";\n";
                opCode = "PD";
                s += opCode + (int)(line.X2*modifier) + " " + (int)(line.Y2*modifier) + ";\n";
            }

            return s;
        }
        public List<System.Windows.Shapes.Line> getLines()
        {
            return lines_;
        }

        public static void moveTo(List<PolyLine> polylines, System.Windows.Point newPosition)
        {
            System.Windows.Point currentPosition = getPosition(polylines);
            double offsetX, offsetY;
            offsetX = newPosition.X - currentPosition.X;
            offsetY = newPosition.Y - currentPosition.Y;

            foreach (PolyLine polyLine in polylines)
            {
                polyLine.translate(offsetX, offsetY);
            }
        }
        public static void setSize(List<PolyLine> polylines, System.Windows.Size newSize)
        {

            System.Windows.Size currentSize = getSize(polylines);

            double scaleX, scaleY;
            scaleX = newSize.Width / currentSize.Width;
            scaleY = newSize.Height / currentSize.Height;

            foreach (PolyLine polyLine in polylines)
            {
                polyLine.scale(scaleX, scaleY);
            }
        }

        public static List<PolyLine> convertRectangleToPolyLines(System.Windows.Rect rectangle, bool machineRemoval)
        {
            double removal = machineRemoval ? Machine.INSIDEKNIFEPOINTSREMOVAL / Machine.POINTSPERINCH : 0;
            System.Windows.Shapes.Line left, right, top, bottom;
            left = new System.Windows.Shapes.Line();
            right = new System.Windows.Shapes.Line();
            top = new System.Windows.Shapes.Line();
            bottom = new System.Windows.Shapes.Line();
            bottom.X1 = top.X2 = rectangle.Right - removal;
            bottom.X2 = top.X1 = rectangle.Left + removal;
            left.Y1 = right.Y2 = rectangle.Bottom + removal;
            left.Y2 = right.Y1 = rectangle.Top - removal;
            bottom.Y1 = bottom.Y2 = rectangle.Bottom;
            top.Y1 = top.Y2 = rectangle.Top;
            left.X1 = left.X2 = rectangle.Left;
            right.X1 = right.X2 = rectangle.Right + 0.01;
            return convertLinesToPolyLines(new List<System.Windows.Shapes.Line>(new System.Windows.Shapes.Line[] { bottom, left, top, right }));
        }
        public static List<System.Windows.Shapes.Line> getAllLines(List<PolyLine> polyLines)
        {
            List<System.Windows.Shapes.Line> ret = new List<System.Windows.Shapes.Line>();
            foreach (PolyLine polyLine in polyLines)
            {
                ret.AddRange(polyLine.lines_);
            }
            return ret;
        }
        public static List<PolyLine> convertLinesToPolyLines(List<System.Windows.Shapes.Line> lines)
        {
            List<PolyLine> ret = new List<PolyLine>();
            PolyLine currentPolyLine = new PolyLine();
            foreach (System.Windows.Shapes.Line line in lines)
            {
                if (currentPolyLine.lines_.Count == 0)
                {
                    currentPolyLine.lines_.Add(line);
                }
                else if (
                    currentPolyLine.lines_.Last().X2 == line.X1 &&
                    currentPolyLine.lines_.Last().Y2 == line.Y1 )
                {
                    currentPolyLine.lines_.Add(line);
                }
                else
                {
                    PolyLine addon = new PolyLine();
                    addon.lines_.AddRange(currentPolyLine.lines_);
                    currentPolyLine.lines_.Clear();
                    ret.Add(addon);
                    currentPolyLine.lines_.Add(line);
                }
            }
            if (currentPolyLine.lines_.Count > 0) ret.Add(currentPolyLine);
            return ret;
        }
        public static List<PolyLine> getPolylinesFromFile(string filePath)
        {
            string allCode = File.ReadAllText(filePath);
            allCode = Regex.Replace(allCode, "\n", "");
            allCode = Regex.Replace(allCode, "\r", "");
            allCode = Regex.Replace(allCode, ",", " ");
            while (true)
            {
                string current = allCode;
                allCode = Regex.Replace(allCode, "  ", " ");
                if (current == allCode) break;
            }
            string[] commands = allCode.Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            List<System.Windows.Shapes.Line> lines = new List<System.Windows.Shapes.Line>();
            for (int i = 0; i < commands.Length - 1; i++)
            {
                HPGLCommand c1 = new HPGLCommand(commands[i]);
                HPGLCommand c2 = new HPGLCommand(commands[i + 1]);
                System.Windows.Shapes.Line l = HPGLCommand.getPLTLine(c1, c2);
                lines.Add(l);
            }
            return convertLinesToPolyLines(lines);
        }

        public static System.Windows.Size getSize(List<PolyLine> polylines)
        {
            double minX, minY, maxX, maxY;
            minX = minY = 99999999999;
            maxX = maxY = -99999999999;
            foreach (System.Windows.Shapes.Line line in getAllLines(polylines))
            {
                minX = Math.Min(Math.Min(minX, line.X1), line.X2);
                maxX = Math.Max(Math.Max(maxX, line.X1), line.X2);
                minY = Math.Min(Math.Min(minY, line.Y1), line.Y2);
                maxY = Math.Max(Math.Max(maxY, line.Y1), line.Y2);
            }
            return new System.Windows.Size(maxX - minX, maxY - minY);
        }
        private static System.Windows.Point getPosition(List<PolyLine> polylines)
        {
            double minX, minY;
            minX = minY = 99999999999;
            foreach (System.Windows.Shapes.Line line in getAllLines(polylines))
            {
                minX = Math.Min(Math.Min(minX, line.X1), line.X2);
                minY = Math.Min(Math.Min(minY, line.Y1), line.Y2);
            }
            return new System.Windows.Point(minX, minY);
        }

        private void scale(double scaleX, double scaleY)
        {
            foreach (System.Windows.Shapes.Line line in lines_)
            {
                line.X1 *= scaleX;
                line.Y1 *= scaleY;
                line.X2 *= scaleX;
                line.Y2 *= scaleY;
            }
        }
        private void translate(double dx, double dy)
        {
            foreach (System.Windows.Shapes.Line line in lines_)
            {
                line.X1 += dx;
                line.Y1 += dy;
                line.X2 += dx;
                line.Y2 += dy;
            }
        }
       
        private List<System.Windows.Shapes.Line> lines_;
    }
}
