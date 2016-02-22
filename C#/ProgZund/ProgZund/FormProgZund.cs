using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProgZund
{
    public partial class FormProgZund : Form
    {
        private Cut cut;

        public FormProgZund()
        {
            cut = new SimpleCut();
            InitializeComponent();
            MessageBox.Show(Utility.GET_FILE_BEGINNING());
        }


        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            updatePicture(e);

        }

        private void updatePicture(PaintEventArgs e)
        {
            try
            {
                updateCut();
                pictureBox1.Height = (int)(pictureBox1.Width * Utility.TOTALHEIGHT / Utility.TOTALWIDTH);
                e.Graphics.Clear(Color.White);
                Pen p = new Pen(Color.Black);
                foreach(List<System.Windows.Shapes.Line> polyline in cut.getAllLines())
                {
                    foreach(System.Windows.Shapes.Line line in polyline)
                    {
                        Point p1 = Utility.CONVERT_XY_REAL_TO_POINT(line.X1, line.Y1, pictureBox1.Size);
                        Point p2 = Utility.CONVERT_XY_REAL_TO_POINT(line.X2, line.Y2, pictureBox1.Size);
                        e.Graphics.DrawLine(p, p1, p2);
                    }
                }
            }
            catch(Exception ex)
            {
                e.Graphics.Clear(Color.White);
                //MessageBox.Show(ex.Message);
            }
        }

        private void updateCut()
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    ((SimpleCut)cut).updateCut(
                        textBoxInsideWidthSimple.Text, textBoxInsideHeightSimple.Text,
                        textBoxOutsideWidthSimple.Text, textBoxOutsideHeightSimple.Text,
                        textBoxQtyX.Text, textBoxQtyY.Text);
                    break;
                case 1:
                    break;
                case 2:
                    break;
            }
            cut.writeFile(@"C:\Users\ClémentGamache\Desktop\plt.plt");
            
        }


        private void textBoxQtyX_TextChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            Refresh();
        }
        

        private void FormProgZund_Resize(object sender, EventArgs e)
        {
            Refresh();
        }

        private void buttonCut_Click(object sender, EventArgs e)
        {
            try
            {
                cut.cut();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                cut.writeFile(@"C:\Users\ClémentGamache\Desktop\plt.plt");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

}


