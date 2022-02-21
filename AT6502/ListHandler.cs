/*   This file is part of AT6502.
     Copyright (C) 2022  Dan Boris (danlb_2000@yahoo.com)
    AT6502 is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    ACS Viewer is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;
using System.Text;

namespace AT6502
{
    public class ListHandler
    {
        public List<string> Lines { get; set; }
        public bool EnableList { get; set; }   
        public bool ConsoleOutput { get; set; }

        public ListHandler()
        {
            Lines = new List<string>();
        }

        private void AddLine(string line)
        {
            if (EnableList) Lines.Add(line);
            if (ConsoleOutput) System.Console.WriteLine(line);
        }

        public void AddError(AssembleException exception)
        {         
            var msg = $"ERROR [{exception.File}:{exception.LineNumber}]: {exception.ToString()}";
            Lines.Add(exception.Line);
            Lines.Add(msg);
            System.Console.WriteLine(exception.Line);
            System.Console.WriteLine(msg);            
        }

        public void AddComment(string comment)
        {
            AddLine(";" + comment);
        }

        public void FileSwitch(string fileName)
        {
            AddLine("------- FILE " + fileName);
        }

        public void AddMacroCall(int lineNumber, int pc, string line)
        {
            AddLine(line);
        }

        public void AddLabel(string label)
        {
            AddLine(label + ":");
        }

        public void AddPsuedoOp(string line)
        {
            AddLine(line);
        }

        public void AddMessage(string message)
        {
            AddLine(message);
        }

        public void AddLine(string file, int lineNumber, int pc, List<byte> data, string line)
        {
            var listLine = new StringBuilder();

            string s = lineNumber.ToString().PadLeft(6, ' ');
            listLine.Append(s);
            listLine.Append("  ");

            s = string.Format("{0:X4}", pc);
            listLine.Append(s);
            listLine.Append("    ");

            if (data != null)
            {
                foreach (byte b in data)
                {
                    listLine.Append(string.Format("{0:X2}", b));
                    listLine.Append(" ");
                }
            }
            listLine.AppendAt(line, 43);
            if (data != null && data.Count > 0)
            {
                AddLine(listLine.ToString());
            }

        }
    }
}
