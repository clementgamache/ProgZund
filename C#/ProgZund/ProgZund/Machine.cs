using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgZund
{
    class Machine
    {
        public static double TOTALWIDTH = 47.6;
        public static double TOTALHEIGHT = 32.2;
        public static double POINTSPERINCH = 2540.0;
        public static double INSIDEKNIFEPOINTSREMOVAL = 15;

        //Gets the header of the plt file
        public static string GET_FILE_BEGINNING()
        {
            return System.IO.File.ReadAllText(System.IO.Directory.GetParent(Directory.GetCurrentDirectory().ToString()).ToString() + @"\BEGIN.txt");
        }

        //Gets the footer of the plt file
        public static string GET_FILE_ENDING()
        {
            return System.IO.File.ReadAllText(System.IO.Directory.GetParent(Directory.GetCurrentDirectory().ToString()).ToString() + @"\END.txt");
        }

        //Gets the saved port
        public static string GET_PORT()
        {
            return System.IO.File.ReadAllText(System.IO.Directory.GetParent(Directory.GetCurrentDirectory().ToString()).ToString() + @"\PORT.txt");
        }
    }
}
