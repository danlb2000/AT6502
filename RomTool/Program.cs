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
using System.IO;

namespace RomTool
{
    class Program
    {
        static void Main(string[] args)
        {


            var commandLine = new CommandLineParser();
            commandLine.ParseCommandLine(args);

            if (!File.Exists(commandLine.SourceFile)) {
                Console.WriteLine("Cannot find source file");
                return;
            }

            var fullRom = File.ReadAllBytes(commandLine.SourceFile);
            var rom = GetSegment(fullRom, commandLine.Start, commandLine.Length);
            HandleChecksum(commandLine, rom);
            File.WriteAllBytes(commandLine.DestFile, rom);
        }

        static void HandleChecksum(CommandLineParser commandLine, byte[] rom)
        {
            if (!commandLine.RomNumber.HasValue) return;

            if (commandLine.CheckOffset.HasValue)
            {
                rom[commandLine.CheckOffset.Value] = 0;
                var checksum = CalcualteChecksum(rom, commandLine.RomNumber.Value);
                rom[commandLine.CheckOffset.Value] = checksum;
            }
            else if (!string.IsNullOrEmpty(commandLine.CheckSymbol))
            {
                var checksum = CalcualteChecksum(rom, commandLine.RomNumber.Value);

                var addr = GetSymbol(commandLine.SymbolFile, commandLine.CheckSymbol);
                if (!addr.HasValue)
                {
                    Console.WriteLine($"Symbol {commandLine.SymbolFile} not found");
                    return;
                }
                rom[addr.Value - commandLine.Start] = checksum;
            }
            else 
            {
                var checksum = CalcualteChecksum(rom, commandLine.RomNumber.Value);
                Console.WriteLine("Checksum: " + string.Format("{0:X2}", checksum));
            }
        }

        static int? GetSymbol(string symbolFile,string symbolName)
        {
            if (string.IsNullOrEmpty(symbolFile)) return null;

            int? addr = null;
            using (var sr = new StreamReader(symbolFile))
            {
                var line = "";
                while(line != null)
                {
                    line = sr.ReadLine();
                    var parts = line.Split(',');
                    if (parts[0] == symbolName) addr = Convert.ToInt16(parts[1], 16);
                }
                sr.Close();
            }

            return addr;                        
        }

        static byte[] GetSegment(byte[] fullRom, int start, int length)
        {
            byte[] segment = new byte[length];
            for (int i = 0; i < length; i++)
            {
                segment[i] = fullRom[start + i];
            }
            return segment;

        }



        static byte CalcualteChecksum(byte[] rom, int romNum)
        {
            byte checksum = 0xFF;

            foreach (byte d in rom)
            {
                checksum = (byte)(checksum ^ d);
            }

            Console.WriteLine(string.Format("{0:X2}", checksum));
            checksum = (byte)(checksum ^ romNum);
            return checksum;
        }
    }
}
