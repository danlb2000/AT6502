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
    public class Memory
    {
        byte[] mem;

        List<byte> bytesWritten;

        public Memory()
        {
            mem = new byte[65536];
            bytesWritten = new List<byte>();
        }

        public byte[] MemoryArray
        {
            get { return mem; }
        }

        public void WriteMem(int addr, byte data)
        {
            mem[addr] = data;
            bytesWritten.Add(data);
        }

        public byte ReadMem(int addr)
        {
            return mem[addr];
        }

        public List<byte> PopBytesWritten()
        {
            var b = bytesWritten;
            bytesWritten = new List<byte>();
            return b;
        }
    }
}
