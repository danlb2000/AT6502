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
    public static class StringExtensions
    {
        public static string GetBetween(this String s, char start, char end)
        {
            int startPos = s.IndexOf(start);
            if (startPos == -1) return "";
            int endPos = s.IndexOf(end, startPos);
            if (endPos == -1)
                return s.Substring(startPos + 1);
            else
                return s.Substring(startPos + 1, endPos - startPos -1);
        }

        public static string RemoveString(this String s, string remove)
        {
            return s.Substring(remove.Length);
        }

        public static string GetUntil(this String s,char endChar)
        {
            if (string.IsNullOrEmpty(s)) return "";
            var i = s.IndexOf(endChar);
            if (i == -1) return "";
            return s.Substring(0, i );
        }


        public static string GetUntilLast(this String s, char endChar)
        {
            if (string.IsNullOrEmpty(s)) return "";
            var i = s.LastIndexOf(endChar);
            if (i == -1) return "";
            return s.Substring(0, i);
        }

        public static Match Match(this string source, IStringMatch matcher, int? start = 0)
        {
            return matcher.MatchString(source, false,start);
        }

        public static Match Match(this string source, params IStringMatch[] matchers)
        {
            var set = new TerminalSet(matchers);
            return set.MatchString(source, false, 0);
        }

        public static Match MatchIgnoreWhitespace(this string source, params IStringMatch[] matchers)
        {
            var set = new TerminalSet(matchers);
            return set.MatchString(source, true, 0);
        }

    }
}
