using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgZund
{
    class FromFileCut : Cut
    {
        public FromFileCut() { }

        public void updateCut(
            string filePath,
            string leftPad, string rightPad, string topPad, string bottomPad,
            bool addBorders, bool keepOriginalSize, bool keepOriginalRatio,
            string insideWidth, string insideHeight,
            string qtyX, string qtyY)
        {
            double outsideWidth, outsideHeight;
            updateBaseCut(outsideWidth.ToString(), outsideHeight.ToString(), qtyX, qtyY);
            updateFromFileCut();
        }
        

        private void updateFromFileCut() { }

        protected override List<List<System.Windows.Shapes.Line>> getInsideLines()
        {
            return new List<List<System.Windows.Shapes.Line>>();
        }
    }
}
