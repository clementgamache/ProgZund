using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;

namespace ProgZund
{
    public abstract class Cut
    {
        public Cut() {}

        public Cut(
            string outsideWidth, string outsideHeight,
            string qtyX, string qtyY, bool cutOutSide = true)
        {
            updateBaseCut(outsideWidth, outsideHeight, qtyX, qtyY, cutOutSide);
        }

        protected void updateBaseCut(
            string outsideWidth, string outsideHeight, 
            string qtyX, string qtyY, bool cutOutSide = true)
        {
            double w, h;
            int qX, qY;
            w = h = qX = qY = 0;
            if (!(double.TryParse(outsideWidth, out w) && double.TryParse(outsideHeight, out h) &&
                int.TryParse(qtyX, out qX) && int.TryParse(qtyY, out qY)))
                throw(Utility.NOTNUMERICENTRYEXCEPTION);
            
            if (!(w > 0 && h > 0 && qX > 0 && qY > 0 &&
                    w*qX <= Utility.TOTALWIDTH && h*qY <= Utility.TOTALHEIGHT))
                throw(Utility.OUTSIDEBOUNDSEXCEPTION);
            outside_ = new System.Windows.Size(w, h);
            qtyX_ = qX;
            qtyY_ = qY;
            cutOutside_ = cutOutSide;
            
        }

        public List<List<System.Windows.Shapes.Line>> getAllLines()
        {
            List<List<System.Windows.Shapes.Line>> r = getAllInsideLines();
            r.AddRange(getOutsideLines());
            return r;
        }

        protected abstract List<List<System.Windows.Shapes.Line>> getInsideLines();

        private List<List<System.Windows.Shapes.Line>> getAllInsideLines()
        {
            List<List<System.Windows.Shapes.Line>> insideLines = getInsideLines();
            List<List<System.Windows.Shapes.Line>> allLines = new List<List<System.Windows.Shapes.Line>>();
            bool top = false;
            bool left = qtyY_ % 2 == 1;
            for (int i = left ? 0 : qtyX_-1; left ? i < qtyX_ : i >=0 ; i+= left ? 1 : -1)
            {
                for (int j = top ? 0 : qtyY_-1; top ? j < qtyY_ : j >=0; j+= top ? 1 : -1)
                {
                    foreach(List<System.Windows.Shapes.Line> polyline in insideLines)
                    {
                        List<System.Windows.Shapes.Line> newPolyline = new List<System.Windows.Shapes.Line>();
                        foreach (System.Windows.Shapes.Line line in polyline)
                        {
                            System.Windows.Shapes.Line newLine = new System.Windows.Shapes.Line();
                            newLine.X1 = line.X1 + i * outside_.Width;
                            newLine.X2 = line.X2 + i * outside_.Width;
                            newLine.Y1 = line.Y1 + j * outside_.Height;
                            newLine.Y2 = line.Y2 + j * outside_.Height;
                            newPolyline.Add(newLine);
                        }
                        allLines.Add(newPolyline);
                    }
                }
                top = !top;
            }
            return allLines;
        }

        public List<List<System.Windows.Shapes.Line>> getOutsideLines()
        {
            List<List<System.Windows.Shapes.Line>> lines = new List<List<System.Windows.Shapes.Line>>();

            if (!cutOutside_)
                return lines;

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
                
                lines.Add(Utility.convertLineToPolyLine(newLine));
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
                lines.Add(Utility.convertLineToPolyLine( newLine));
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
            lines.AddRange(Utility.convertLinesToPolylines(new List<System.Windows.Shapes.Line> { rightLine, bottomLine, leftLine, topLine, }));
            return lines;
        }

        public void writeFile(string path)
        {
            System.IO.File.WriteAllText(path, getPLT());
        }

        public void cut()
        {
            string path = System.IO.Directory.GetParent(Directory.GetCurrentDirectory().ToString()).ToString() + @"\PLT.plt";
            writeFile(path);
            SerialPort port = new SerialPort(Utility.GET_PORT(), 19200, Parity.None, 8, StopBits.One);

            Byte[] data = File.ReadAllBytes(path);

            port.Write(data, 0, data.Count());

            System.IO.File.Delete(path);
        }
               
        private string getPLT()
        {
            return
                //Utility.GET_FILE_BEGINNING() + 
                getInsidePLT() +
                getOutsidePLT();//+ 
                //Utility.GET_FILE_ENDING(); 
        } 

        private string getOutsidePLT()
        {
            string s = "";
            foreach(List<System.Windows.Shapes.Line> polyline in getOutsideLines())
            {
                s += Utility.POLYLINE_TO_PLT(polyline);
            }
            return s;
        }

        protected virtual string getInsidePLT()
        {
            string s = "";
            foreach (List<System.Windows.Shapes.Line> polyline in getAllInsideLines())
            {
                s += Utility.POLYLINE_TO_PLT(polyline);
            }

            return s;
        }

        
        protected int qtyX_, qtyY_;
        protected System.Windows.Size outside_;
        protected bool cutOutside_;
    }
}
