using Nakov.TurtleGraphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Shapes;

namespace ProgZund
{
    public partial class TurtleSimulationForm : Form
    {
        private List<PolyLine> polylines;

        public TurtleSimulationForm(List<PolyLine> polylines)
        {
            InitializeComponent();
            this.polylines = polylines;
        }

        private void TurtleSimulationForm_Load(object sender, EventArgs e)
        {
            
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Turtle.Dispose();
            this.Close();
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            float minX, minY, maxX, maxY;
            minX = -400;
            minY = -250;
            maxX = 500;
            maxY = 250;
            System.Windows.Size size = PolyLine.getSize(polylines);
            System.Windows.Point position = PolyLine.getPosition(polylines);
            float scale;
            Turtle.Delay = 1;
            if (size.Width/size.Height > (maxX - minX)/(maxY-minY))
            {
                scale = (float)size.Width / (maxX - minX);
            }
            else
            {
                scale = (float)size.Height / (maxY - minY);
            }
            Turtle.PenSize = 2;
            Turtle.PenColor = Color.Green;
            Turtle.PenUp();
            Turtle.ShowTurtle = true;
            try
            {

                foreach (PolyLine polyline in polylines)
                {
                    List<Line> lines = polyline.getLines();
                    Turtle.MoveTo((float)lines[0].X1 / scale + (float)position.X * scale + minX, (float)lines[0].Y1 / scale + (float)position.Y * scale + minY);
                    Turtle.PenDown();
                    foreach (Line l in lines)
                    {
                        //Turtle.MoveTo((float)l.X1 / scale + (float)position.X * scale + minX, (float)l.Y1 / scale + (float)position.Y * scale +minY);
                        Turtle.MoveTo((float)l.X2 / scale + (float)position.X * scale + minX, (float)l.Y2 / scale + (float)position.Y * scale + minY);
                    }
                    Turtle.PenUp();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
