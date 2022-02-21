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
using System.Collections.Generic;
using System.IO;

namespace AT6502
{
    class Program
    {
        static void Main(string[] args)
        {
            var commandLine = new CommandLineParser();
            commandLine.ParseCommandLine(args);

            var assembler = new Assembler();
            assembler.listHandler.ConsoleOutput = commandLine.ConsoleOutput;
            try
            {
                assembler.StartAssemble(commandLine.SourceFiles);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (!string.IsNullOrEmpty(commandLine.ListFile))
            {
                SaveList(commandLine.ListFile, assembler.listHandler.Lines);
            }


            if (!string.IsNullOrEmpty(commandLine.SymbolTable))
            {
                WriteSymbolTable(assembler.symbolTable, commandLine.SymbolTable);
            }

            if (!string.IsNullOrEmpty(commandLine.OutFile))
            {
                WriteRom(assembler.memory.MemoryArray, commandLine.OutFile);
            }

        }

        static void WriteRom(byte[] memory,string fileName)
        {
            File.WriteAllBytes(fileName, memory);
        }

        static void WriteSymbolTable(SymbolTable table, string symbolTableFile)
        {
            var sw = new StreamWriter(symbolTableFile, false);
            sw.Write(table.ExportTable());
            sw.Close();
        }

        static void SaveList(string fileName, List<string> lines)
        {
            var sw = new StreamWriter(fileName, false);
            foreach (string l in lines)
            {
                sw.WriteLine(l);
            }
            sw.Close();
        }
    }
}
