using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using FSUtility;
using Nakov.TurtleGraphics;

namespace ProgZund
{
    public abstract class Cut
    {
        public Cut() { }
        public Cut(
            double outsideWidth, double outsideHeight,
            int qtyX, int qtyY, bool cutOutSide = true)
        {
            updateBaseCut(outsideWidth, outsideHeight, qtyX, qtyY, cutOutSide);
        }

        protected void updateBaseCut(
            double outsideWidth, double outsideHeight, 
            int qtyX, int qtyY, bool cutOutSide = true)
        {
            outside_ = new System.Windows.Size(outsideWidth, outsideHeight);
            qtyX_ = qtyX;
            qtyY_ = qtyY;
            cutOutside_ = cutOutSide;
        }

        protected abstract List<PolyLine> getInsideLines();
        public List<PolyLine> getOutsideLines()
        {
            List<System.Windows.Shapes.Line> lines = new List<System.Windows.Shapes.Line>();

            if (!cutOutside_)
                return PolyLine.convertLinesToPolyLines(lines);

            bool top = qtyX_ % 2 == 1;
            bool left = qtyY_ % 2 == 0;
            for (int i = left ? 1 : qtyX_-1; left ? i <qtyX_ : i > 0 ; i += left ? 1 : -1)
            {
                System.Windows.Shapes.Line newLine = new System.Windows.Shapes.Line();
                newLine.X1 = newLine.X2 = i*outside_.Width;
                if (top)
                {
                    newLine.Y1 = 0;
                    newLine.Y2 = outside_.Height*qtyY_;
                }
                else
                {
                    newLine.Y2 = 0;
                    newLine.Y1 = outside_.Height * qtyY_;
                }
                
                lines.Add(newLine);
                top = !top;
            }

            
            for (int j = 1; j < qtyY_; j++)
            {
                System.Windows.Shapes.Line newLine = new System.Windows.Shapes.Line();
                newLine.Y1 = newLine.Y2 = j*outside_.Height;
                if (left)
                {
                    newLine.X1 = outside_.Width*qtyX_;
                    newLine.X2 = 0;
                }
                else
                {
                    newLine.X1 = 0;
                    newLine.X2 = outside_.Width * qtyX_;
                }
                lines.Add(newLine);
                left = !left;
            }
            System.Windows.Shapes.Line topLine = new System.Windows.Shapes.Line();
            System.Windows.Shapes.Line rightLine = new System.Windows.Shapes.Line();
            System.Windows.Shapes.Line bottomLine = new System.Windows.Shapes.Line();
            System.Windows.Shapes.Line leftLine = new System.Windows.Shapes.Line();
            topLine.X2 = rightLine.X1 = rightLine.X2 = bottomLine.X1 = 0;
            
            topLine.X1 = bottomLine.X2 = leftLine.X1 = leftLine.X2 = outside_.Width * qtyX_;
            topLine.Y1 = topLine.Y2 = rightLine.Y1 = leftLine.Y2 = outside_.Height * qtyY_;
            rightLine.Y2 = bottomLine.Y1 = bottomLine.Y2 = leftLine.Y1;
            lines.AddRange(
                new List<System.Windows.Shapes.Line> { rightLine, bottomLine, leftLine, topLine, });
            return PolyLine.convertLinesToPolyLines(lines);
        }

        private List<PolyLine> getAllInsideLines()
        {
            List<System.Windows.Shapes.Line> insideLines = PolyLine.getAllLines(getInsideLines());
            List<System.Windows.Shapes.Line> allLines = new List<System.Windows.Shapes.Line>();
            bool top = false;
            bool left = qtyY_ % 2 == 1;
            for (int i = left ? 0 : qtyX_-1; left ? i < qtyX_ : i >=0 ; i+= left ? 1 : -1)
            {
                for (int j = top ? 0 : qtyY_-1; top ? j < qtyY_ : j >=0; j+= top ? 1 : -1)
                {
                    foreach(System.Windows.Shapes.Line line in insideLines)
                    {
                        System.Windows.Shapes.Line newLine = new System.Windows.Shapes.Line();
                        newLine.X1 = line.X1 + i * outside_.Width;
                        newLine.X2 = line.X2 + i * outside_.Width;
                        newLine.Y1 = line.Y1 + j * outside_.Height;
                        newLine.Y2 = line.Y2 + j * outside_.Height;
                        //insideLines.Add(newLine);
                        allLines.Add(newLine);
                    }
                }
                top = !top;
            }
            return PolyLine.convertLinesToPolyLines(allLines);
            //return allLines;
        }
        public List<PolyLine> getAllLines()
        {
            List<PolyLine> r = getAllInsideLines();
            r.AddRange(getOutsideLines());
            return r;
        }

        //writes all plt to the file that will be printed
        public void writeFile(string path)
        {
            System.IO.File.WriteAllText(path, getPLT());
        }

        //sends the file to the buffer
        public void cut()
        {
            string path = System.IO.Directory.GetParent(Directory.GetCurrentDirectory().ToString()).ToString() + @"\PLT.plt";
            writeFile(path);
            using (SerialPort port = new SerialPort("COM4", 19200, Parity.None, 8, StopBits.One)) 
{
                port.Open();
                byte[] data = File.ReadAllBytes(path);

                port.Write(data, 0, data.Count());
            }

            //SerialPort port = new SerialPort("COM3", 19200, Parity.None, 8, StopBits.One);
            //port.Open();
            //byte[] data = File.ReadAllBytes(path);

            //port.Write(data, 0, data.Count());
            //System.IO.File.Delete(path);
        }

        public void simulate()
        {
            List<PolyLine> polylines = getAllLines();
            Turtle.Forward(200);
        }
               
        //computes the plt code associated with the cut
        private string getPLT()
        {
            return
                Utility.getFileBegin() + 
                getInsidePLT() +
                getOutsidePLT() + 
                Utility.getFileEnding(); 
        } 
        private string getOutsidePLT()
        {
            string s = "";
            foreach (PolyLine polyline in getOutsideLines())
            {
                s += polyline.getPlt();
            }
            return s;
        }
        protected virtual string getInsidePLT()
        {
            string s = "";
            foreach (PolyLine polyline in getAllInsideLines())
            {
                s += polyline.getPlt();
            }
            return s;
        }

        
        protected int qtyX_, qtyY_;
        protected System.Windows.Size outside_;
        protected bool cutOutside_;
    }
}
