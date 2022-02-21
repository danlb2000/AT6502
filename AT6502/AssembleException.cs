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


using System;

namespace AT6502
{
    public class AssembleException : Exception
    {
        public int LineNumber { get; set; }
        public string File { get; set; }
        public bool Fatal { get; set; }
        public string Line { get; set; }

        public AssembleException()
        {
        }

        public AssembleException(string message, int lineNumber, string fileName ,string line, bool fatal)
            : base(message)
        {
            LineNumber = lineNumber;
            File = fileName;
            Fatal = fatal;
            Line = line;
        }

        public override string ToString()
        {
            return $"{File}({LineNumber}) {Message}";
        }
    }
}
