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


namespace AT6502
{
    public class WhiteSpaceTerminal : IStringMatch
    {
        public Match MatchString(string source, bool? ingoreWhitespace = false,int? start = 0)
        {
            int i = start.Value;
            while(i<source.Length) { 
                if (!char.IsWhiteSpace(source[i])) break;
                i++;
            }
            
            if(i==start.Value)
            {
                return new Match(false, "",start.Value);
            }
            else
            {
                return new Match(true, source.Substring(start.Value, i-start.Value),i-1);
            }
          
        }

    }
}
