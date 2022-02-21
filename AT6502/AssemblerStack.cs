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

namespace AT6502
{
    public class AssemblerStack
    {
        int pointer;
        string name;
        int size;

        public string Name { get { return name; } }
        public int Pointer { get {return pointer;} }

        private Stack<double> stack = null; 

        public AssemblerStack(string name, int size)
        {
            stack = new Stack<double>();
            this.size = size;
            this.pointer = size;
            this.name = name;
        }

        public int Count
        {
            get
            {
                return stack.Count;
            }
        }

        public double Pop()
        {
            pointer++;
            return stack.Pop();
        }

        public void Push(double d)
        {
            pointer--;
            stack.Push(d);
        }
    }
}
