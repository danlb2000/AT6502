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
using System.Linq;

namespace AT6502
{
    public class Opcodes6502
    {
        List<OpcodeDef> opcodes = new List<OpcodeDef>()
        {
            new OpcodeDef { OpName = "ADC", AddressMode = "IM", OpCode = 0x69 },
            new OpcodeDef { OpName = "ADC", AddressMode = "ZP", OpCode = 0x65 },
            new OpcodeDef { OpName = "ADC", AddressMode = "ZX", OpCode = 0x75 },
            new OpcodeDef { OpName = "ADC", AddressMode = "AB", OpCode = 0x6D },
            new OpcodeDef { OpName = "ADC", AddressMode = "AX", OpCode = 0x7D },
            new OpcodeDef { OpName = "ADC", AddressMode = "AY", OpCode = 0x79 },
            new OpcodeDef { OpName = "ADC", AddressMode = "IX", OpCode = 0x61 },
            new OpcodeDef { OpName = "ADC", AddressMode = "IY", OpCode = 0x71 },

            new OpcodeDef { OpName = "AND", AddressMode = "IM", OpCode = 0x29 },
            new OpcodeDef { OpName = "AND", AddressMode = "ZP", OpCode = 0x25 },
            new OpcodeDef { OpName = "AND", AddressMode = "ZX", OpCode = 0x35 },
            new OpcodeDef { OpName = "AND", AddressMode = "AB", OpCode = 0x2D },
            new OpcodeDef { OpName = "AND", AddressMode = "AX", OpCode = 0x3D },
            new OpcodeDef { OpName = "AND", AddressMode = "AY", OpCode = 0x39 },
            new OpcodeDef { OpName = "AND", AddressMode = "IX", OpCode = 0x21 },
            new OpcodeDef { OpName = "AND", AddressMode = "IY", OpCode = 0x31 },

            new OpcodeDef { OpName = "ASL", AddressMode = "AC", OpCode = 0x0A },
            new OpcodeDef { OpName = "ASL", AddressMode = "ZP", OpCode = 0x06 },
            new OpcodeDef { OpName = "ASL", AddressMode = "ZX", OpCode = 0x16 },
            new OpcodeDef { OpName = "ASL", AddressMode = "AB", OpCode = 0x0E },
            new OpcodeDef { OpName = "ASL", AddressMode = "AX", OpCode = 0x1E },

            new OpcodeDef { OpName = "BCC", AddressMode = "RE", OpCode = 0x90 },
            new OpcodeDef { OpName = "BCS", AddressMode = "RE", OpCode = 0xB0 },
            new OpcodeDef { OpName = "BEQ", AddressMode = "RE", OpCode = 0xF0 },
            new OpcodeDef { OpName = "BIT", AddressMode = "ZP", OpCode = 0x24 },
            new OpcodeDef { OpName = "BIT", AddressMode = "AB", OpCode = 0x2C },
            new OpcodeDef { OpName = "BMI", AddressMode = "RE", OpCode = 0x30 },
            new OpcodeDef { OpName = "BNE", AddressMode = "RE", OpCode = 0xD0 },
            new OpcodeDef { OpName = "BPL", AddressMode = "RE", OpCode = 0x10 },
            new OpcodeDef { OpName = "BRK", AddressMode = "IP", OpCode = 0x00 },
            new OpcodeDef { OpName = "BVC", AddressMode = "RE", OpCode = 0x50 },
            new OpcodeDef { OpName = "BVS", AddressMode = "RE", OpCode = 0x70 },

            new OpcodeDef { OpName = "CLC", AddressMode = "IP", OpCode = 0x18 },
            new OpcodeDef { OpName = "CLD", AddressMode = "IP", OpCode = 0xD8 },
            new OpcodeDef { OpName = "CLI", AddressMode = "IP", OpCode = 0x58 },
            new OpcodeDef { OpName = "CLV", AddressMode = "IP", OpCode = 0xB8 },

            new OpcodeDef { OpName = "CMP", AddressMode = "IM", OpCode = 0xC9 },
            new OpcodeDef { OpName = "CMP", AddressMode = "ZP", OpCode = 0xC5 },
            new OpcodeDef { OpName = "CMP", AddressMode = "ZX", OpCode = 0xD5 },
            new OpcodeDef { OpName = "CMP", AddressMode = "AB", OpCode = 0xCD },
            new OpcodeDef { OpName = "CMP", AddressMode = "AX", OpCode = 0xDD },
            new OpcodeDef { OpName = "CMP", AddressMode = "AY", OpCode = 0xD9 },
            new OpcodeDef { OpName = "CMP", AddressMode = "IX", OpCode = 0xC1 },
            new OpcodeDef { OpName = "CMP", AddressMode = "IY", OpCode = 0xD1 },

            new OpcodeDef { OpName = "CPX", AddressMode = "IM", OpCode = 0xE0 },
            new OpcodeDef { OpName = "CPX", AddressMode = "ZP", OpCode = 0xE4 },
            new OpcodeDef { OpName = "CPX", AddressMode = "AB", OpCode = 0xEC },

            new OpcodeDef { OpName = "CPY", AddressMode = "IM", OpCode = 0xC0 },
            new OpcodeDef { OpName = "CPY", AddressMode = "ZP", OpCode = 0xC4 },
            new OpcodeDef { OpName = "CPY", AddressMode = "AB", OpCode = 0xCC },

            new OpcodeDef { OpName = "DEC", AddressMode = "ZP", OpCode = 0xC6 },
            new OpcodeDef { OpName = "DEC", AddressMode = "ZX", OpCode = 0xD6 },
            new OpcodeDef { OpName = "DEC", AddressMode = "AB", OpCode = 0xCE },
            new OpcodeDef { OpName = "DEC", AddressMode = "AX", OpCode = 0xDE },

            new OpcodeDef { OpName = "DEX", AddressMode = "IP", OpCode = 0xCA },
            new OpcodeDef { OpName = "DEY", AddressMode = "IP", OpCode = 0x88 },

            new OpcodeDef { OpName = "EOR", AddressMode = "IM", OpCode = 0x49 },
            new OpcodeDef { OpName = "EOR", AddressMode = "ZP", OpCode = 0x45 },
            new OpcodeDef { OpName = "EOR", AddressMode = "ZX", OpCode = 0x55 },
            new OpcodeDef { OpName = "EOR", AddressMode = "AB", OpCode = 0x4D },
            new OpcodeDef { OpName = "EOR", AddressMode = "AX", OpCode = 0x5D },
            new OpcodeDef { OpName = "EOR", AddressMode = "AY", OpCode = 0x59 },
            new OpcodeDef { OpName = "EOR", AddressMode = "IX", OpCode = 0x41 },
            new OpcodeDef { OpName = "EOR", AddressMode = "IY", OpCode = 0x51 },

            new OpcodeDef { OpName = "INC", AddressMode = "ZP", OpCode = 0xE6 },
            new OpcodeDef { OpName = "INC", AddressMode = "ZX", OpCode = 0xF6 },
            new OpcodeDef { OpName = "INC", AddressMode = "AB", OpCode = 0xEE },
            new OpcodeDef { OpName = "INC", AddressMode = "AX", OpCode = 0xFE },

            new OpcodeDef { OpName = "INX", AddressMode = "IP", OpCode = 0xE8 },
            new OpcodeDef { OpName = "INY", AddressMode = "IP", OpCode = 0xC8 },

            new OpcodeDef { OpName = "JMP", AddressMode = "AB", OpCode = 0x4C },
            new OpcodeDef { OpName = "JMP", AddressMode = "ID", OpCode = 0x6C },
            new OpcodeDef { OpName = "JSR", AddressMode = "AB", OpCode = 0x20 },

            new OpcodeDef { OpName = "LDA", AddressMode = "IM", OpCode = 0xA9 },
            new OpcodeDef { OpName = "LDA", AddressMode = "ZP", OpCode = 0xA5 },
            new OpcodeDef { OpName = "LDA", AddressMode = "ZX", OpCode = 0xB5 },
            new OpcodeDef { OpName = "LDA", AddressMode = "AB", OpCode = 0xAD },
            new OpcodeDef { OpName = "LDA", AddressMode = "AX", OpCode = 0xBD },
            new OpcodeDef { OpName = "LDA", AddressMode = "AY", OpCode = 0xB9 },
            new OpcodeDef { OpName = "LDA", AddressMode = "IX", OpCode = 0xA1 },
            new OpcodeDef { OpName = "LDA", AddressMode = "IY", OpCode = 0xB1 },

            new OpcodeDef { OpName = "LDX", AddressMode = "IM", OpCode = 0xA2 },
            new OpcodeDef { OpName = "LDX", AddressMode = "ZP", OpCode = 0xA6 },
            new OpcodeDef { OpName = "LDX", AddressMode = "ZY", OpCode = 0xB6 },
            new OpcodeDef { OpName = "LDX", AddressMode = "AB", OpCode = 0xAE },
            new OpcodeDef { OpName = "LDX", AddressMode = "AY", OpCode = 0xBE },

            new OpcodeDef { OpName = "LDY", AddressMode = "IM", OpCode = 0xA0 },
            new OpcodeDef { OpName = "LDY", AddressMode = "ZP", OpCode = 0xA4 },
            new OpcodeDef { OpName = "LDY", AddressMode = "ZX", OpCode = 0xB4 },
            new OpcodeDef { OpName = "LDY", AddressMode = "AB", OpCode = 0xAC },
            new OpcodeDef { OpName = "LDY", AddressMode = "AX", OpCode = 0xBC },

            new OpcodeDef { OpName = "LSR", AddressMode = "AC", OpCode = 0x4A },
            new OpcodeDef { OpName = "LSR", AddressMode = "ZP", OpCode = 0x46 },
            new OpcodeDef { OpName = "LSR", AddressMode = "ZX", OpCode = 0x56 },
            new OpcodeDef { OpName = "LSR", AddressMode = "AB", OpCode = 0x4E },
            new OpcodeDef { OpName = "LSR", AddressMode = "AX", OpCode = 0x5E },

            new OpcodeDef { OpName = "NOP", AddressMode = "IP", OpCode = 0xEA },

            new OpcodeDef { OpName = "ORA", AddressMode = "IM", OpCode = 0x09 },
            new OpcodeDef { OpName = "ORA", AddressMode = "ZP", OpCode = 0x05 },
            new OpcodeDef { OpName = "ORA", AddressMode = "ZX", OpCode = 0x15 },
            new OpcodeDef { OpName = "ORA", AddressMode = "AB", OpCode = 0x0D },
            new OpcodeDef { OpName = "ORA", AddressMode = "AX", OpCode = 0x1D },
            new OpcodeDef { OpName = "ORA", AddressMode = "AY", OpCode = 0x19 },
            new OpcodeDef { OpName = "ORA", AddressMode = "IX", OpCode = 0x01 },
            new OpcodeDef { OpName = "ORA", AddressMode = "IY", OpCode = 0x11 },

            new OpcodeDef { OpName = "PHA", AddressMode = "IP", OpCode = 0x48 },
            new OpcodeDef { OpName = "PHP", AddressMode = "IP", OpCode = 0x08 },
            new OpcodeDef { OpName = "PLA", AddressMode = "IP", OpCode = 0x68 },
            new OpcodeDef { OpName = "PLP", AddressMode = "IP", OpCode = 0x28 },

            new OpcodeDef { OpName = "ROL", AddressMode = "AC", OpCode = 0x2A },
            new OpcodeDef { OpName = "ROL", AddressMode = "ZP", OpCode = 0x26 },
            new OpcodeDef { OpName = "ROL", AddressMode = "ZX", OpCode = 0x36 },
            new OpcodeDef { OpName = "ROL", AddressMode = "AB", OpCode = 0x2E },
            new OpcodeDef { OpName = "ROL", AddressMode = "AX", OpCode = 0x3E },

            new OpcodeDef { OpName = "ROR", AddressMode = "AC", OpCode = 0x6A },
            new OpcodeDef { OpName = "ROR", AddressMode = "ZP", OpCode = 0x66 },
            new OpcodeDef { OpName = "ROR", AddressMode = "ZX", OpCode = 0x76 },
            new OpcodeDef { OpName = "ROR", AddressMode = "AB", OpCode = 0x6E },
            new OpcodeDef { OpName = "ROR", AddressMode = "AX", OpCode = 0x7E },

            new OpcodeDef { OpName = "RTI", AddressMode = "IP", OpCode = 0x40 },
            new OpcodeDef { OpName = "RTS", AddressMode = "IP", OpCode = 0x60 },

            new OpcodeDef { OpName = "SBC", AddressMode = "IM", OpCode = 0xE9 },
            new OpcodeDef { OpName = "SBC", AddressMode = "ZP", OpCode = 0xE5 },
            new OpcodeDef { OpName = "SBC", AddressMode = "ZX", OpCode = 0xF5 },
            new OpcodeDef { OpName = "SBC", AddressMode = "AB", OpCode = 0xED },
            new OpcodeDef { OpName = "SBC", AddressMode = "AX", OpCode = 0xFD },
            new OpcodeDef { OpName = "SBC", AddressMode = "AY", OpCode = 0xF9 },
            new OpcodeDef { OpName = "SBC", AddressMode = "IX", OpCode = 0xE1 },
            new OpcodeDef { OpName = "SBC", AddressMode = "IY", OpCode = 0xF1 },

            new OpcodeDef { OpName = "SEC", AddressMode = "IP", OpCode = 0x38 },
            new OpcodeDef { OpName = "SED", AddressMode = "IP", OpCode = 0xF8 },
            new OpcodeDef { OpName = "SEI", AddressMode = "IP", OpCode = 0x78 },

            new OpcodeDef { OpName = "STA", AddressMode = "ZP", OpCode = 0x85 },
            new OpcodeDef { OpName = "STA", AddressMode = "ZX", OpCode = 0x95 },
            new OpcodeDef { OpName = "STA", AddressMode = "AB", OpCode = 0x8D },
            new OpcodeDef { OpName = "STA", AddressMode = "AX", OpCode = 0x9D },
            new OpcodeDef { OpName = "STA", AddressMode = "AY", OpCode = 0x99 },
            new OpcodeDef { OpName = "STA", AddressMode = "IX", OpCode = 0x81 },
            new OpcodeDef { OpName = "STA", AddressMode = "IY", OpCode = 0x91 },

            new OpcodeDef { OpName = "STX", AddressMode = "ZP", OpCode = 0x86 },
            new OpcodeDef { OpName = "STX", AddressMode = "ZY", OpCode = 0x96 },
            new OpcodeDef { OpName = "STX", AddressMode = "AB", OpCode = 0x8E },

            new OpcodeDef { OpName = "STY", AddressMode = "ZP", OpCode = 0x84 },
            new OpcodeDef { OpName = "STY", AddressMode = "ZX", OpCode = 0x94 },
            new OpcodeDef { OpName = "STY", AddressMode = "AB", OpCode = 0x8C },

            new OpcodeDef { OpName = "TAX", AddressMode = "IP", OpCode = 0xAA },
            new OpcodeDef { OpName = "TAY", AddressMode = "IP", OpCode = 0xA8 },
            new OpcodeDef { OpName = "TSX", AddressMode = "IP", OpCode = 0xBA },
            new OpcodeDef { OpName = "TXA", AddressMode = "IP", OpCode = 0x8A },
            new OpcodeDef { OpName = "TXS", AddressMode = "IP", OpCode = 0x9A },
            new OpcodeDef { OpName = "TYA", AddressMode = "IP", OpCode = 0x98 },

        };

        Dictionary<string, string> addressModePrefix = new Dictionary<string, string>()
        {
            {"#","IM" },
            {"I,","IM" },
            {"ZX,","ZX" },
            {"ZY,","ZY" },
            {"Z,","ZP" },
            {"X," ,"X"},
            {"Y,","Y" },
            {"NY,","IY" },
            {"NX,","IX" },
            {"AX,","AX" },
            {"AY,","AY" },
            {"A,","AB"}
        };

        Dictionary<string, string> addressModeSuffix = new Dictionary<string, string>()
        {
            {"(X)","AX" },
            {"(Y)","AY" },
            {"(X","AX" },
            {"(Y","AY" }
        };


        Dictionary<string, string> nameList = new Dictionary<string, string>();
        Dictionary<string, string> relativeModeOpcodes = new Dictionary<string, string>();
        Dictionary<string, string> impliedModeOpcodes = new Dictionary<string, string>();
        Dictionary<string, string> accumulatorModeOpcodes = new Dictionary<string, string>();

        public Opcodes6502()
        {
            foreach (OpcodeDef op in opcodes)
            {
                if (!nameList.ContainsKey(op.OpName)) nameList.Add(op.OpName, op.OpName);
                if (op.AddressMode == "RE") relativeModeOpcodes.Add(op.OpName, op.OpName);
                if (op.AddressMode == "IP") impliedModeOpcodes.Add(op.OpName, op.OpName);
                if (op.AddressMode == "AC") accumulatorModeOpcodes.Add(op.OpName, op.OpName);
            }
        }

        public string DetermineAddressingMode(string op, string line, double? address)
        {
            if (relativeModeOpcodes.ContainsKey(op)) return "RE";
            if (impliedModeOpcodes.ContainsKey(op)) return "IP";
            if (accumulatorModeOpcodes.ContainsKey(op) && line == "") return "AC";

            if (line.StartsWith("@"))
            {
                if (line.EndsWith("(Y)")) return "IY";
                if (line.EndsWith("(X)")) return "IX";
            }

            var modeid = AddressModeIdentifier(line);

            if (modeid == "X")
            {
                if (address.HasValue && address < 0x100)
                    modeid = "ZX";
                else
                    modeid = "AX";

            }

            if (modeid == "Y")
            {
                if (address.HasValue && address < 0x100)
                    modeid = "ZY";
                else
                    modeid = "AY";
            }

            if (modeid == "")
            {
                if (address.HasValue && address < 0x100 && op.Substring(0,1) != "J")
                    modeid = "ZP";
                else
                    modeid = "AB";
            }

            return modeid;
        }


        public byte GetOpcode(string opName, string addressMode)
        {
            var opcode = (from o in opcodes where o.OpName == opName && o.AddressMode == addressMode select o).FirstOrDefault<OpcodeDef>();
            if (opcode != null) return opcode.OpCode;
            return 0;
        }


        public byte[] GetOperandData(double? operand, string addressMode, int pc)
        {
            byte[] b = new byte[0];

            int word;

            if (operand.HasValue)
            {
                word = (int)operand;
            }
            else
            {
                word = 0;
            }

            switch (addressMode)
            {
                case "IM":
                case "ZP":
                case "ZX":
                case "ZY":
                case "IX":
                case "IY":
                    b = new Byte[1];
                    b[0] = (byte)(word & 0xFF);
                    break;
                case "AB":
                case "AX":
                case "AY":
                    b = new Byte[2];
                    b[0] = (byte)(word & 0xFF);
                    b[1] = (byte)((word & 0xFF00) >> 8);
                    break;
                case "RE":
                    b = new Byte[1];
                    if (word >= pc)
                    {
                        b[0] = (byte)(word - pc - 1);
                    }
                    else
                    {
                        b[0] = (byte)(((pc - word) ^ 0xFF));
                    }
                    break;
            }
            return b;
        }

        public string RemoveAddressModeIdentifier(string line)
        {
            if (line.StartsWith("@")) line = line.Substring(1);

            foreach (KeyValuePair<string, string> kvp in addressModePrefix)
            {
                if (line.StartsWith(kvp.Key))
                {
                    return line.RemoveString(kvp.Key);
                }
            }

            foreach (KeyValuePair<string, string> kvp in addressModeSuffix)
            {
                if (line.EndsWith(kvp.Key))
                {
                    return line.Substring(0, line.Length - kvp.Key.Length);
                }
            }

            return line;
        }


        public string AddressModeIdentifier(string line)
        {
            foreach (KeyValuePair<string, string> kvp in addressModePrefix)
            {
                if (line.StartsWith(kvp.Key)) return kvp.Value;
            }

            foreach (KeyValuePair<string, string> kvp in addressModeSuffix)
            {
                if (line.EndsWith(kvp.Key)) return kvp.Value;
            }

            return "";
        }

        public bool IsValidOpcode(string opcode)
        {
            return nameList.ContainsKey(opcode);
        }
    }
}
