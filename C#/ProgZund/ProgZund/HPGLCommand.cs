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
            cmd = cmd.Trim(' ');
            cmd = cmd.Trim('\r');

            args = new List<string>();
            if (cmd.Length >= 2)
            {
                opCode = cmd.Substring(0, 2);
                cmd = cmd.Remove(0, 2);
                string[] tmp = cmd.Split(' ');
                foreach (string s in tmp)
                    if (s.Length > 0) args.Add(s);
            }
            else
                opCode = "00";
            int a = 1;

        }

        
        public string opCode;
        public List<string> args;
    }
}
