using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgZund
{
    public class SimpleCut : Cut
    {
        public SimpleCut() { }

        public void updateCut(
            double insideWidth, double insideHeight,
            double outsideWidth, double outsideHeight,
            int qtyX, int qtyY)
        {
            updateBaseCut(outsideWidth, outsideHeight, qtyX, qtyY);
            updateSingleCut(insideWidth, insideHeight);
        }
        private void updateSingleCut(double insideWidth, double insideHeight)
        {
            inside_ = new System.Windows.Size(insideWidth, insideHeight);
        }
        
        protected override List<PolyLine> getInsideLines()
        {
            if (inside_.Width == 0 || inside_.Height == 0)
                return new List<PolyLine>(); 
            System.Windows.Point pos = new System.Windows.Point(
                (outside_.Width - inside_.Width) / 2,
                (outside_.Height - inside_.Height) / 2);
            List<PolyLine> l = PolyLine.convertRectangleToPolyLines(new System.Windows.Rect(pos, inside_), true);
            return l;
        }
       
        private System.Windows.Size inside_;
    }
}
