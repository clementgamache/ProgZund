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

        public SimpleCut(
            string insideWidth, string insideHeight,
            string outsideWidth, string outsideHeight,
            string qtyX, string qtyY)
            :base(outsideWidth, outsideHeight, qtyX, qtyY)
        {
            updateSingleCut(insideWidth, insideHeight);
        }

        public void updateCut(
            string insideWidth, string insideHeight,
            string outsideWidth, string outsideHeight,
            string qtyX, string qtyY)
        {
            updateBaseCut(outsideWidth, outsideHeight, qtyX, qtyY);
            updateSingleCut(insideWidth, insideHeight);
        }

        private void updateSingleCut(string insideWidth, string insideHeight)
        {
            double w, h;
            w = h = 0;
            if (!(string.IsNullOrWhiteSpace(insideWidth) && string.IsNullOrWhiteSpace(insideHeight)))
            {
               if (!(double.TryParse(insideWidth, out w) && double.TryParse(insideHeight, out h)))
                    throw(Utility.NOTNUMERICENTRYEXCEPTION);
               if (!(w >= 0 && h >= 0 && w < outside_.Width && h < outside_.Height))
                    throw (Utility.INSIDEISBIGGERTHANOUTSIDEEXCEPTION);
                if ((w == 0 && h != 0) || (w != 0 && h == 0))
                    throw (Utility.NOTNUMERICENTRYEXCEPTION);
            }
            
            inside_ = new System.Windows.Size(w, h);

        }
        
        protected override List<List<System.Windows.Shapes.Line>> getInsideLines()
        {
            List<List<System.Windows.Shapes.Line>> lines = new List<List<System.Windows.Shapes.Line>>();

            if (inside_.Width == 0 || inside_.Height == 0)
                return lines;
            System.Windows.Point pos = new System.Windows.Point(
                (outside_.Width - inside_.Width) / 2,
                (outside_.Height - inside_.Height) / 2);



            return Utility.getPolylinesFromRectangle(pos, inside_);
        }
       
        private System.Windows.Size inside_;
    }
}
