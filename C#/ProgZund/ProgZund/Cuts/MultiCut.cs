using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProgZund
{
    public class MultiCut : Cut
    {
        public MultiCut() { }

        //update the cut parameters depending on textboxes
        public void updateCut(
            List<System.Windows.Rect> insideCuts,
            double outsideWidth, double outsideHeight,
            int qtyX, int qtyY)
        {
            updateBaseCut(outsideWidth, outsideHeight, qtyX, qtyY);
            updateMultiCut(insideCuts);
        }
        private void updateMultiCut(List<System.Windows.Rect> insideCuts)
        {
            insideCuts_ = insideCuts;
        }

        protected override List<PolyLine> getInsideLines()
        {
            List<PolyLine> polyLines = new List<PolyLine>();

            foreach (System.Windows.Rect r in insideCuts_)
            {
                polyLines.AddRange(PolyLine.convertRectangleToPolyLines(new System.Windows.Rect( r.Location, r.Size), true));
            }

            return polyLines;
        }

        private List<System.Windows.Rect> insideCuts_;
    }
}
