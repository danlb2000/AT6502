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


using System.Text;

namespace AT6502
{
    public static class StringBuilderExtensions
    {

        public static void AppendAt(this StringBuilder sb, string appendString, int position)
        {
            if (position <= sb.Length)
            {
                sb.Append(appendString);
                return;
            }

            for (int i = 0; i < position - sb.Length; i++) sb.Append(" ");
            sb.Append(appendString);
        }
    }
}
