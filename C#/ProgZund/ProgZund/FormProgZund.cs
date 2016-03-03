using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
                pictureBox1.Height = (int)(pictureBox1.Width * Machine.TOTALHEIGHT / Machine.TOTALWIDTH);
                e.Graphics.Clear(Color.White);
                Pen p = new Pen(Color.Black);
                foreach(System.Windows.Shapes.Line line in PolyLine.getAllLines(cut.getAllLines()))
                {
                    System.Drawing.Point p1 = Utility.CONVERT_XY_REAL_TO_POINT(line.X1, line.Y1, pictureBox1.Size);
                    System.Drawing.Point p2 = Utility.CONVERT_XY_REAL_TO_POINT(line.X2, line.Y2, pictureBox1.Size);
                    e.Graphics.DrawLine(p, p1, p2);
                }
            }
            catch(Exception ex)
            {
                clearCanvas();
                assignCutter();
                System.Windows.MessageBox.Show("Failed to update painting \n" + ex.Message);
            }
        }

        private void updateCut()
        {
            try
            {
                double oW, oH, iW, iH, lP, rP, tP, bP;
                oW = oH = iW = iH = lP = rP = tP = bP = 0;
                string[] strQty = { textBoxQtyX.Text, textBoxQtyY.Text };
                int qtyX = Utility.intEntry(strQty[0]);
                int qtyY = Utility.intEntry(strQty[1]);
                switch (tabControl1.SelectedIndex)
                {
                    case 0:
                        string[] stringValuesSimple = { textBoxInsideWidthSimple.Text, textBoxInsideHeightSimple.Text,
                            textBoxOutsideWidthSimple.Text, textBoxOutsideHeightSimple.Text};
                        iW = Utility.dblEntry(stringValuesSimple[0]);
                        iH = Utility.dblEntry(stringValuesSimple[1]);
                        oW = Utility.dblEntry(stringValuesSimple[2]);
                        oH = Utility.dblEntry(stringValuesSimple[3]);
                        Utility<double>.testMax(iW, oW);
                        Utility<double>.testMax(iH, oH);
                        Utility<double>.testMax(oW * qtyX, Machine.TOTALWIDTH);
                        Utility<double>.testMax(oH * qtyY, Machine.TOTALHEIGHT);


                        ((SimpleCut)cut).updateCut(iW, iH, oW, oH, qtyX, qtyY);
                        break;
                    case 1:
                        string[] stringValuesMulti = {
                            textBoxInsideWidth1.Text, textBoxInsideHeight1.Text, textBoxInsideX1.Text, textBoxInsideY1.Text,
                            textBoxInsideWidth2.Text, textBoxInsideHeight2.Text, textBoxInsideX2.Text, textBoxInsideY2.Text,
                            textBoxInsideWidth3.Text, textBoxInsideHeight3.Text, textBoxInsideX3.Text, textBoxInsideY3.Text,
                            textBoxInsideWidth4.Text, textBoxInsideHeight4.Text, textBoxInsideX4.Text, textBoxInsideY4.Text,
                            textBoxInsideWidth5.Text, textBoxInsideHeight5.Text, textBoxInsideX5.Text, textBoxInsideY5.Text,
                            textBoxInsideWidth6.Text, textBoxInsideHeight6.Text, textBoxInsideX6.Text, textBoxInsideY6.Text,
                            textBoxInsideWidth7.Text, textBoxInsideHeight7.Text, textBoxInsideX7.Text, textBoxInsideY7.Text,
                            textBoxInsideWidth8.Text, textBoxInsideHeight8.Text, textBoxInsideX8.Text, textBoxInsideY8.Text,
                            textBoxInsideWidth9.Text, textBoxInsideHeight9.Text, textBoxInsideX9.Text, textBoxInsideY9.Text,
                            textBoxInsideWidth10.Text, textBoxInsideHeight10.Text, textBoxInsideX10.Text, textBoxInsideY10.Text
                        };
                        oW = Utility.dblEntry(textBoxMultiOutsideWidth.Text);
                        oH = Utility.dblEntry(textBoxMultiOutsideHeight.Text);
                        List<System.Windows.Rect> r = new List<System.Windows.Rect>();
                        for (int i = 0; i < stringValuesMulti.Length; i += 4)
                        {
                            Rect rec = Utility.parseRectangle(
                                stringValuesMulti[i],
                                stringValuesMulti[i + 1],
                                stringValuesMulti[i + 2],
                                stringValuesMulti[i + 3]);
                            Utility<double>.testMax(rec.X + rec.Width, oW);
                            Utility<double>.testMax(rec.Y + rec.Height, oH);
                            Rect trueRect = new Rect();
                            trueRect.Size = rec.Size;
                            trueRect.X = oW - rec.Width - rec.X;
                            trueRect.Y = oH - rec.Height - rec.Y;

                            r.Add(trueRect);
                        }
                        ((MultiCut)cut).updateCut(r, oW, oH, qtyX, qtyY);
                        break;
                    case 2:
                        if (checkBoxKeepSize.Checked)
                        {
                            System.Windows.Size s = Utility.getPLTSize(openFileDialog.FileName);
                            textBox2.Text = Math.Round(s.Width,Utility.MAXDIGITS).ToString();
                            textBox3.Text = Math.Round(s.Height,Utility.MAXDIGITS).ToString();
                        } 
                        else if (checkBoxKeepRatio.Checked)
                        {
                            setHeightFromWidth();
                        }
                        

                        string[] stringValuesFromFile = {openFileDialog.FileName,
                            textBoxPaddingLeft.Text, textBoxPaddingRight.Text,
                            textBoxPaddingTop.Text, textBoxPaddingBottom.Text,
                            textBox2.Text, textBox3.Text, //size
                            textBoxQtyX.Text, textBoxQtyY.Text};
                        string fN = openFileDialog.FileName;
                        lP = Utility.dblEntry(stringValuesFromFile[1]);
                        rP = Utility.dblEntry(stringValuesFromFile[2]);
                        tP = Utility.dblEntry(stringValuesFromFile[3]);
                        bP = Utility.dblEntry(stringValuesFromFile[4]);
                        iW = Utility.dblEntry(stringValuesFromFile[5]);
                        iH = Utility.dblEntry(stringValuesFromFile[6]);
                        oW = lP + iW + rP;
                        oH = tP + iH + bP;
                        Utility<double>.testMax(iW, oW);
                        Utility<double>.testMax(iH, oH);

                        ((FromFileCut)cut).updateCut(openFileDialog.FileName,
                            lP, rP, tP, bP, 
                            checkBoxAddBorders.Checked, oW, oH,
                            qtyX, qtyY);
                        break;
                }
                Utility<double>.testMax(oW * qtyX, Machine.TOTALWIDTH);
                Utility<double>.testMax(oH * qtyY, Machine.TOTALHEIGHT);
                if (oH == 0 || oW == 0)
                    clearCanvas();
                cut.writeFile(Directory.GetCurrentDirectory() + "plt.plt");
            }
            catch(Exception ex)
            {
                assignCutter();
                clearCanvas();
                System.Windows.MessageBox.Show("Failed to update work \n" + ex.Message);

            }
            
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
                System.Windows.MessageBox.Show("Failed to cut \n" + ex.Message);
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                saveFileDialog.ShowDialog();
                cut.writeFile(saveFileDialog.FileName);
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show("Failed to save \n" + ex.Message);
            }
        }

        private void checkBoxKeepSize_CheckedChanged(object sender, EventArgs e)
        {
            tableLayoutPanelSize.Enabled = checkBoxKeepRatio.Enabled = !checkBoxKeepSize.Checked;
            Refresh();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            clearCanvas();
            assignCutter();
            updateCut();
            Refresh();
        }

        private void buttonFileDialog_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
            labelChosenFile.Text = openFileDialog.FileName;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            setHeightFromWidth();
            Refresh();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            setWidthFromHeight();
            Refresh();
        }

        private void checkBoxKeepRatio_CheckedChanged(object sender, EventArgs e)
        {
            setHeightFromWidth();
            Refresh();
        }

        private void assignCutter()
        {
            try
            {
                switch (tabControl1.SelectedIndex)
                {
                    case 0:
                        cut = new SimpleCut();
                        break;
                    case 1:
                        cut = new MultiCut();
                        break;
                    case 2:
                        cut = new FromFileCut();
                        break;
                }
                clearCanvas();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Failed to construct cutter \n" + ex.Message);
            }
        }
        private void setHeightFromWidth()
        {
            try
            {
                if (checkBoxKeepRatio.Checked)
                {
                    System.Windows.Size s = Utility.getPLTSize(openFileDialog.FileName);
                    double ratio = s.Height / s.Width;
                    double iW = Utility.dblEntry(textBox2.Text);
                    textBox3.Text = Math.Round((iW * ratio), Utility.MAXDIGITS).ToString();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void setWidthFromHeight()
        {
            try
            { 
                if (checkBoxKeepRatio.Checked)
                {
                    System.Windows.Size s = Utility.getPLTSize(openFileDialog.FileName);
                    double ratio = s.Height / s.Width;
                    double iH = Utility.dblEntry(textBox3.Text);
                    textBox2.Text = Math.Round((iH / ratio), Utility.MAXDIGITS).ToString();
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private void clearCanvas()
        {
            if (pictureBox1.Image == null)
            {
                Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.White);
                }
                pictureBox1.Image = bmp;
            }
            using (Graphics g = Graphics.FromImage(pictureBox1.Image))
            {
                g.Clear(System.Drawing.Color.White);
            }
        }
    }

}


