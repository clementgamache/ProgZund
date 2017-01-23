using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProgZund
{
    class VirtualMachine
    {
        private bool isDown;
        private bool isRelative;
        private int currentPen;
        private string currentCommand;
        private System.Windows.Point currentPosition;
        public List<List<System.Windows.Shapes.Line>> visitedLines;

        public VirtualMachine()
        {
            initialize();
        }

        private string currentCommandMessage()
        {
            return "\nCommand: " + currentCommand;
        }

        private void initialize()
        {
            isDown = false;
            isRelative = false;
            currentPosition = new System.Windows.Point(0, 0);
            currentCommand = "";
            visitedLines = new List<List<System.Windows.Shapes.Line>>();
            int penCount = 256;
            for (int i = 0; i < penCount; i++)
            {
                visitedLines.Add(new List<System.Windows.Shapes.Line>());
            }
        }

        public void receiveFile(string filePath)
        {
            try
            {
                string allCode = File.ReadAllText(filePath);
                allCode = Regex.Replace(allCode, "\n", "");
                allCode = Regex.Replace(allCode, "\r", "");
                allCode = Regex.Replace(allCode, ",", " ");
                while (true)
                {
                    string current = allCode;
                    allCode = Regex.Replace(allCode, "  ", " ");
                    if (current == allCode) break;
                }
                string[] commands = allCode.Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string strCommand in commands)
                {
                    currentCommand = strCommand;
                    receiveCommand();
                }

            }
            catch (Exception e)
            {
                throw new Exception(e.Message + currentCommandMessage());
            }
        }

        private void receiveCommand()
        {
            HPGLCommand c = new HPGLCommand(currentCommand);
            if (c.args.Count> 0)
            {
                //System.Windows.MessageBox.Show(c.opCode + c.args[0]);
            }
            
            switch (c.opCode)
            {
                case "PA":
                    receivePA(c.args);
                    break;
                case "SP":
                    receiveSP(c.args);
                    break;
                case "PR":
                    receivePR(c.args);
                    break;
                case "PU":
                    receivePU(c.args);
                    break;
                case "PD":
                    receivePD(c.args);
                    break;
                case "IN":
                    initialize();
                    break;
                default:
                    break;
            }
        }

        private void receiveSP(List<string> args)
        {
            if (args.Count == 1)
            {
                if (!int.TryParse(args[0], out currentPen))
                {
                    throw new Exception("Not integer argument for a pen select command");
                }
            }
            else if (args.Count > 1)
                throw new Exception("Too many arguments for a pen select command");
        }

        private void receivePU(List<string> args)
        {
            isDown = false;
            move(args);
        }

        private void receivePD(List<string> args)
        {
            isDown = true;
            move(args);
        }

        private void receivePA(List<string> args)
        {
            isRelative = false;
            move(args);
        }

        private void receivePR(List<string> args)
        {
            isRelative = true;
            move(args);
        }
        
        private void move(List<string> args)
        {
            List<double> argd = new List<double>();
            foreach (string arg in args)
            {
                double a = 0;
                if (!double.TryParse(arg, out a))
                {
                    throw new Exception("Not numeric arguments for a moving command");
                }
                argd.Add(a);
            }
            if (args.Count % 2 != 0)
            {
                throw new Exception("Odd number of arguments for a move command");
            }

            for (int i = 0; i < argd.Count; i += 2)
            {
                System.Windows.Point startPosition = currentPosition;
                double dx = -argd[i];
                double dy = argd[i + 1];
                if (isRelative)
                    currentPosition = new System.Windows.Point(startPosition.X + dx, startPosition.Y + dy);
                else
                    currentPosition = new System.Windows.Point(dx, dy);
                if (isDown)
                {
                    System.Windows.Shapes.Line newLine = new System.Windows.Shapes.Line();
                    newLine.X1 = startPosition.X;
                    newLine.Y1 = startPosition.Y;
                    newLine.X2 = currentPosition.X;
                    newLine.Y2 = currentPosition.Y;
                    if (Math.Abs(newLine.X2 - newLine.X1) > 0 || Math.Abs(newLine.Y2 - newLine.Y1) > 0)
                        visitedLines[currentPen].Add(newLine);
                }
            }
        }
    }
}
