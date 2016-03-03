using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace ProgZund
{
    class HPGLCommand
    {
        public HPGLCommand(string cmd)
        {
            cmd = cmd.Trim();
            cmd = cmd.Trim(';');
            cmd = cmd.Trim('\n');
            cmd = Regex.Replace(cmd, ", ", " ");
            opCode = cmd.Substring(0, 2);
            cmd = cmd.Remove(0, 2);
            args = new List<double>();
            if (cmd.Length > 2)
            {
                string[] argStr = cmd.Split(new Char[] { ' ', ',' });
                foreach (string arg in argStr)
                {
                    double o = 0;
                    if (!double.TryParse(arg, out o))
                    {
                        throw BADFILEEXCEPTION;
                    }
                    args.Add(o);
                }
            }
        }

        public static System.Windows.Shapes.Line getPLTLine(HPGLCommand c1, HPGLCommand c2)
        {
            System.Windows.Shapes.Line ret = new System.Windows.Shapes.Line();
            ret.X1 = ret.X2 = ret.Y1 = ret.Y2 = 0;
            HPGLCommand[] commands = { c1, c2 };
            foreach (HPGLCommand c in commands)
            {
                if (!lineCommands.Contains(c.opCode)) return ret;
                if (c.args.Count != 2) return ret; 
            }
            if (c2.opCode.ToUpper() == "PU") return ret;
            ret = new System.Windows.Shapes.Line();
            ret.X1 = c1.args[0];
            ret.Y1 = c1.args[1];
            ret.X2 = c2.args[0];
            ret.Y2 = c2.args[1];
            return ret;
        }

        private static string[] lineCommands = { "PU", "PD" };
        private static char[] allowed = { ' ', ',', '-', '.' };
        private string opCode;
        private List<double> args;
        private static char[] separators = { ' ', ',' };
        private static Exception BADFILEEXCEPTION = new Exception("File does not respect the syntax");
    }
}
