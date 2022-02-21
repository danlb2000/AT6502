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
using System.Linq;

namespace RomTool
{
    public class CommandLineParser
    {
        public string SourceFile { get; set; }
        public string DestFile { get; set; }
        public int? CheckOffset { get; set; }
        public int? RomNumber { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public string SymbolFile { get; set; }
        public string CheckSymbol { get; set; }

        public CommandLineParser()
        {
        }

        public void Help()
        {
            Console.WriteLine("ROMTOOL [FullRomFile] [DestRomFile] -start:xxxx - length:xxxx {-romnum:n} {-chkoff:xxxx}");
            Console.WriteLine("   FullRomFile = File contianing full binary data");
            Console.WriteLine("   DestRomFile = Output file for the specifed ROM segment");
            Console.WriteLine("   Start = Hex starting ROM location in FullRomFile");
            Console.WriteLine("   Length = Length of ROM to create");
            Console.WriteLine("   -romnum = ROM number used to calcualte checksum. If not included no checksum is done");
            Console.WriteLine("   -chkoff = Offset into DestRomFile to write checksum. If not included checksum is only displayed");
        }

        public void ParseCommandLine(string[] args)
        {
            SourceFile = args[0];
            DestFile = args[1];
            if (args.Count() == 1) return;
            CheckOffset = null;
            RomNumber = null;
            for (int i = 2; i < args.Count(); i++)
            {
                var parts = args[i].Split(':');
                switch(parts[0].ToLower())
                {
                    case "-chkoff":
                        CheckOffset = Convert.ToUInt16(parts[1],16);
                        break;
                    case "-start":
                        Start = Convert.ToUInt16(parts[1], 16);
                        break;
                    case "-length":
                        Length = Convert.ToUInt16(parts[1], 16);
                        break;
                    case "-romnum":
                        RomNumber = Convert.ToInt16(parts[1]);
                        break;
                    case "-symbols":
                        SymbolFile = parts[1];
                        break;
                    case "-chksymbol":
                        CheckSymbol = parts[1];
                        break;
                }
            }
        }


    }
}
