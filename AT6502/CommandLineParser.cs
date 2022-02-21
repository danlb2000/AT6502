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
    public class CommandLineParser
    {
        public List<string> SourceFiles { get; set; }
        public string ListFile { get; set; }
        public string OutFile { get; set; }
        public string SymbolTable { get; set; }
        public bool ConsoleOutput { get; set; }

        public CommandLineParser()
        {
        }

        public void ParseCommandLine(string[] args)
        {
            SourceFiles = args[0].Split(',').ToList<string>() ;
            ConsoleOutput = false;

            if (args.Count() == 1) return;

            for (int i = 1; i < args.Count(); i++)
            {
                var parts = args[i].Split(':');

                switch(parts[0].ToLower())
                {
                    case "-console":
                        ConsoleOutput = true;
                        break;
                    case "-list":
                        ListFile = parts[1];
                        break;
                    case "-output":
                        OutFile = parts[1];
                        break;
                    case "-symbols":
                        SymbolTable = parts[1];
                        break;
                    default:
                        System.Console.WriteLine("Unknow command line parameter " + parts[0]);
                        break;
                }
            }
        }
    }
}
