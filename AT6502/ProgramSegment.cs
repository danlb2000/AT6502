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
using System.Linq;

namespace AT6502
{
    public class ProgramSegment
    {
        List<string> lines;

        public int CurrentLineNumber {get; set;}
        public string Filename { get; set; }        
        public string CurrentLine { get; set; }

        public ProgramSegment()
        {
            lines = new List<string>();
            CurrentLineNumber = 0;
        }

        public int FileLineNumber
        {
            get { return CurrentLineNumber; }
        }

        public void Reset()
        {
            CurrentLineNumber = 0;
        }

        public ProgramSegment(List<string> lines)
        {
            this.lines = lines;
            CurrentLineNumber = 0;
        }

        public string NextLine()
        {
            if (CurrentLineNumber >= lines.Count()) return null;
            CurrentLine = lines[CurrentLineNumber++].ToUpper();        
            return CurrentLine;
        }
    }
}
