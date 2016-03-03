using Hpgl.Language;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgZund
{
    class FromFileCut : Cut
    {
        public FromFileCut() { currentPath_ = ""; }

        public void updateCut(
            string filePath,
            double leftPad, double rightPad, double topPad, double bottomPad,
            bool addBorders, double outsideWidth, double outsideHeight,
            int qtyX, int qtyY)
        {
            try
            {
                updateBaseCut(outsideWidth, outsideHeight, qtyX, qtyY, addBorders);
                updateFromFileCut(filePath, leftPad, rightPad, topPad, bottomPad);
            }
            catch (Exception ex)
            {
                currentPath_ = "";
                throw ex;
            }
        }
        private void updateFromFileCut(
            string fileName, double leftPad, double rightPad, double topPad, double bottomPad)
        {
            leftPad_ = leftPad;
            rightPad_ = rightPad;
            topPad_ = topPad;
            bottomPad_ = bottomPad;
            if (fileName != currentPath_)
            {
                currentPath_ = fileName;
                basePolylines_ = PolyLine.getPolylinesFromFile(currentPath_);
            }
        }

        protected override List<PolyLine> getInsideLines()
        {
            if (string.IsNullOrWhiteSpace(currentPath_) || string.IsNullOrWhiteSpace(currentPath_)) return new List<PolyLine>();

            System.Windows.Size inside = new System.Windows.Size(outside_.Width - leftPad_ - rightPad_, outside_.Height - topPad_ - bottomPad_);

            PolyLine.setSize(basePolylines_, inside);
            PolyLine.moveTo(basePolylines_, new System.Windows.Point(rightPad_, bottomPad_));
            
            return basePolylines_;
        }

        private double leftPad_, rightPad_, topPad_, bottomPad_;
        private string currentPath_;
        private List<PolyLine> basePolylines_;
    }
}
