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



using AT6502;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AT6502Test
{
    [TestClass]
    public class TestAssembler
    {


        [TestMethod] 
        public void Tokenize()
        {
            var list = Assembler.Tokenize("	LDA NY,A");
            Assert.AreEqual(6, list.Count, "Count");
            Assert.AreEqual("\t", list[0]);
            Assert.AreEqual("LDA", list[1]);
            Assert.AreEqual(" ", list[2]);
            Assert.AreEqual("NY", list[3]);
            Assert.AreEqual(",", list[4]);
            Assert.AreEqual("A", list[5]);

            list = Assembler.Tokenize("ADC I,POSVAL&^H0FF00/^H100");
            Assert.AreEqual(9, list.Count, "Count 2");

            list = Assembler.Tokenize("  .MACRO .1.");
            Assert.AreEqual(4, list.Count, "Count 3");
        }


        [TestMethod]
        public void Macro_Locals()
        {
            var lines = new List<string>()
            {
                ".=^H1000",
                " .MACRO DEC16A ADDR,?B,?A",
                "     LDA ADDR",
                "     BNE A",
                "     NOP",
                "A:",
                "    INX",
                "    JMP B",
                ".ENDM",
                "TEST:",
                "DEC16A ^h1000,TEST",
                "DEC16A ^h2000,TEST"
            };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0xAD, assembler.memory.ReadMem(0x1000),"LDA");
            Assert.AreEqual(0x00, assembler.memory.ReadMem(0x1001), "00");
            Assert.AreEqual(0x10, assembler.memory.ReadMem(0x1002), "01");
            Assert.AreEqual(0xD0, assembler.memory.ReadMem(0x1003), "BNE");
            Assert.AreEqual(0x1, assembler.memory.ReadMem(0x1004), "1");
            Assert.AreEqual(0xEA, assembler.memory.ReadMem(0x1005), "NOP");
            Assert.AreEqual(0xE8, assembler.memory.ReadMem(0x1006), "INX");
            Assert.AreEqual(0x4C, assembler.memory.ReadMem(0x1007), "JMP");
            Assert.AreEqual(0x00, assembler.memory.ReadMem(0x1008), "00");
            Assert.AreEqual(0x10, assembler.memory.ReadMem(0x1009), "10");

            Assert.AreEqual(0xAD, assembler.memory.ReadMem(0x100A), "LDA");
            Assert.AreEqual(0x00, assembler.memory.ReadMem(0x100B), "00");
            Assert.AreEqual(0x20, assembler.memory.ReadMem(0x100C), "20");
            Assert.AreEqual(0xD0, assembler.memory.ReadMem(0x100D), "BNE");
            Assert.AreEqual(0x1, assembler.memory.ReadMem(0x100E), "1");
            Assert.AreEqual(0xEA, assembler.memory.ReadMem(0x100F), "NOP");
            Assert.AreEqual(0xE8, assembler.memory.ReadMem(0x1010), "INX");

        }

        [TestMethod]
        public void Macro_LOC()
        {
            var lines = new List<string>()
            {
                ".DEFSTACK PC,4",
	            ".DEFSTACK REGSAV,4",
                ".=^H1000",
                ".MACRO	LOC type",
                "    .PUSH REGSAV,...P0,...S0,...P1,...S1",
                "    ...P0 = .",
                "    ...S0 = type",
                "    .PUSH PC,...P0,...S0",
                "    .POP REGSAV,...S1,...P1,...S0,...P0",
                ".ENDM",
                "...P0 = 1",
                "...S0 = 2",
                "...P1 = 3",
                "...S1 = 4",
                "LOC 1",
            };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(1, assembler.symbolTable.GetSymbol("...P0").NumericValue, "...P0 value");
            Assert.AreEqual(2, assembler.symbolTable.GetSymbol("...S0").NumericValue, "...S0 value");
            Assert.AreEqual(3, assembler.symbolTable.GetSymbol("...P1").NumericValue, "...P1 value");
            Assert.AreEqual(4, assembler.symbolTable.GetSymbol("...S1").NumericValue, "...S1 value");

            Assert.AreEqual(1, assembler.GetStack("PC").Pop(), "PC stack 1");
            Assert.AreEqual(0x1000, assembler.GetStack("PC").Pop(), "PC stack 0");

        }

        [TestMethod]
        public void REPT()
        {
            var lines = new List<string>()
            {
                ".REPT 4",
                "   NOP",
                ".ENDR"
            };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0xEA, assembler.memory.ReadMem(0));
            Assert.AreEqual(0xEA, assembler.memory.ReadMem(1));
            Assert.AreEqual(0xEA, assembler.memory.ReadMem(2));
            Assert.AreEqual(0xEA, assembler.memory.ReadMem(3));
            Assert.AreEqual(0x00, assembler.memory.ReadMem(4));
        }

        [TestMethod]
        public void REPT_0()
        {
            var lines = new List<string>()
            {
                ".REPT 0",
                "   NOP",
                ".ENDR",
                "NOP"
            };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0xEA, assembler.memory.ReadMem(0));
            Assert.AreEqual(0x00, assembler.memory.ReadMem(1));
        }

        [TestMethod]
        public void REPT_0_Nested()
        {
            var lines = new List<string>()
            {
                ".REPT 0",
                "   NOP",
                "   .REPT 5",
                "    DEX",
                "   .ENDR",
                "    DEY",
                ".ENDR",
                "NOP"
            };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0xEA, assembler.memory.ReadMem(0));
            Assert.AreEqual(0x00, assembler.memory.ReadMem(1));
        }

        [TestMethod]
        public void IRP()
        {
            var lines = new List<string>()
            {
                ".IRP OPC,<ASL,ROL>",
                ".MACRO OPC'S COUNT,OPERAND",
                ".REPT COUNT",
                "OPC OPERAND",
                ".ENDR",
                ".ENDM",
                ".ENDR",
                "ASLS 2",
                "ROLS 2"
     
            };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0x0A, assembler.memory.ReadMem(0), "mem 0");
            Assert.AreEqual(0x0A, assembler.memory.ReadMem(1), "mem 1");
            Assert.AreEqual(0x2A, assembler.memory.ReadMem(2), "mem 2");
            Assert.AreEqual(0x2A, assembler.memory.ReadMem(3), "mem 3");
        }

        [TestMethod]
        public void Stack()
        {
            var lines = new List<string>() { ".DEFSTACK REGSAV, 4.","..S1=0",".PUSH REGSAV,1",".POP REGSAV,..S1","  LDA ..S1" };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0xA5, assembler.memory.ReadMem(0));
            Assert.AreEqual(0x01, assembler.memory.ReadMem(1));
        }

        [TestMethod]
        public void GetPointer()
        {
            var lines = new List<string>() { ".DEFSTACK REGSAV, 4.", ".GETPOINTER REGSAV,..P1", ".PUSH REGSAV,1",".GETPOINTER REGSAV,..P2", ".POP REGSAV,..S1", ".GETPOINTER REGSAV,..P3" };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            var symbol = assembler.symbolTable.GetSymbol("..P1");
            Assert.AreEqual(4, symbol.NumericValue,"..P1");
            symbol = assembler.symbolTable.GetSymbol("..P2");
            Assert.AreEqual(3, symbol.NumericValue,"..P2");
            symbol = assembler.symbolTable.GetSymbol("..P3");
            Assert.AreEqual(4, symbol.NumericValue,"..P3");
        }

       [TestMethod]
        public void MACRO_Expand_DoubleDotName()
        {
            var lines = new List<string>() { ".MACRO   ..END", "  PLA", "", " .ENDM", "..END" };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0x68, assembler.memory.ReadMem(0));
        }

        [TestMethod]
        public void MACRO_Expand_NoParameters()
        {
            var lines = new List<string>() { ".MACRO PLYA", "  PLA", "  TAY", " .ENDM","  PLYA" };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0x68, assembler.memory.ReadMem(0));
            Assert.AreEqual(0xA8, assembler.memory.ReadMem(1));
        }


        [TestMethod]
        public void MACRO_Expand_ParamConcatenation()
        {
            var lines = new List<string>() { ".MACRO PL REG", "  PL'REG", "  TAY", " .ENDM", "  PL A" };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0x68, assembler.memory.ReadMem(0));
            Assert.AreEqual(0xA8, assembler.memory.ReadMem(1));
        }

        [TestMethod]
        public void MACRO_Expand_parameters()
        {
            var lines = new List<string>() { ".MACRO CMPIN A,B", " LDY #0", "  LDA NY,A", "  CMP NY,B", " .ENDM", "	CMPIN ^H20,^H30" };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0xA0, assembler.memory.ReadMem(0));
            Assert.AreEqual(0x00, assembler.memory.ReadMem(1));
            Assert.AreEqual(0xB1, assembler.memory.ReadMem(2));
            Assert.AreEqual(0x20, assembler.memory.ReadMem(3));
            Assert.AreEqual(0xD1, assembler.memory.ReadMem(4));
            Assert.AreEqual(0x30, assembler.memory.ReadMem(5));
        }

        [TestMethod]
        public void MACRO_Expand_parameters2()
        {
            var lines = new List<string>() {
                " .MACRO TRAM FROM,TO",
                " LDA FROM",
                "  STA TO",
                " .ENDM",
                "	TRAM ^H1000(X) ^H2000(Y)" 
            };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0xBD, assembler.memory.ReadMem(0));
            Assert.AreEqual(0x00, assembler.memory.ReadMem(1));
            Assert.AreEqual(0x10, assembler.memory.ReadMem(2));
            Assert.AreEqual(0x99, assembler.memory.ReadMem(3));
            Assert.AreEqual(0x00, assembler.memory.ReadMem(4));
            Assert.AreEqual(0x20, assembler.memory.ReadMem(5));
        }

        [TestMethod]
        public void MACRO_Expand_parametersSpaceDelim()
        {
            var lines = new List<string>() { ".MACRO CMPIN A   B", " LDY #0", "  LDA NY,A", "  CMP NY,B", " .ENDM", "	CMPIN ^H20\t^H30" };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0xA0, assembler.memory.ReadMem(0));
            Assert.AreEqual(0x00, assembler.memory.ReadMem(1));
            Assert.AreEqual(0xB1, assembler.memory.ReadMem(2));
            Assert.AreEqual(0x20, assembler.memory.ReadMem(3));
            Assert.AreEqual(0xD1, assembler.memory.ReadMem(4));
            Assert.AreEqual(0x30, assembler.memory.ReadMem(5));
        }

        [TestMethod]
        public void MACRO_Expand_parametersSpaceDelimit()            
        {
            var lines = new List<string>() { ".MACRO CMPIN A,B", " LDY #0", "  LDA NY,A", "  CMP NY,B", " .ENDM", "	CMPIN ^H20 ^H30" };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0xA0, assembler.memory.ReadMem(0));
            Assert.AreEqual(0x00, assembler.memory.ReadMem(1));
            Assert.AreEqual(0xB1, assembler.memory.ReadMem(2));
            Assert.AreEqual(0x20, assembler.memory.ReadMem(3));
            Assert.AreEqual(0xD1, assembler.memory.ReadMem(4));
            Assert.AreEqual(0x30, assembler.memory.ReadMem(5));
        }

        [TestMethod]
        public void MACRO_nesteddef()
        {
            var lines = new List<string>() { ".MACRO TEST", " INY", ".MACRO TESTN", "  NOP", " .ENDM", ".ENDM", "  TEST", "  TESTN", "  TESTN" };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);


            Assert.AreEqual(0xC8, assembler.memory.ReadMem(0));
            Assert.AreEqual(0xEA, assembler.memory.ReadMem(1));
            Assert.AreEqual(0xEA, assembler.memory.ReadMem(2));
        }



        [TestMethod]
        public void MACRO_parameters()
        {
            var lines = new List<string>() { ".MACRO CMPIN A,B", " LDY #0", "  LDA NY,A","  CMP NY,B", " .ENDM" };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            var macro = assembler.GetMacro("CMPIN");
            Assert.IsNotNull(macro, "macro");

            Assert.AreEqual("CMPIN", macro.Name, "Name");
            Assert.AreEqual(3, macro.Lines.Count, "Lines.Count");
            Assert.AreEqual(2, macro.Parameters.Count, "Parameter.Count");
        }

        [TestMethod]
        public void MACRO_noparameters()
        {
            var lines = new List<string>() { ".MACRO PLYA", "  PLA", "  TAY", " .ENDM, " };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            var macro = assembler.GetMacro("PLYA");
            Assert.IsNotNull(macro, "macro");

            Assert.AreEqual("PLYA", macro.Name, "Name");
            Assert.AreEqual(2, macro.Lines.Count, "Lines.Count");
            Assert.AreEqual(0, macro.Parameters.Count, "Parameter.Count");
        }


        [TestMethod]
        public void IF_sub()
        {
            var lines = new List<string>() { ".IF EQ,0", "  NOP", ".IFF", "  DEX", ".IFT","   DEY",".IFTF"," INX",".ENDC" };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0xEA, assembler.memory.ReadMem(0x0));
            Assert.AreEqual(0x88, assembler.memory.ReadMem(0x1));
            Assert.AreEqual(0xE8, assembler.memory.ReadMem(0x2));
        }

        [TestMethod]
        public void IFF_true()
        {
            var lines = new List<string>() { ".IF EQ,0", "  NOP", ".IFF", "  DEX", ".ENDC" };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0xEA, assembler.memory.ReadMem(0x0));
            Assert.AreEqual(0x00, assembler.memory.ReadMem(0x1));
        }

        [TestMethod]
        public void IFF_false()
        {
            var lines = new List<string>() { ".IF EQ,1", "  NOP", ".IFF", "  DEX", ".ENDC" };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0xCA, assembler.memory.ReadMem(0x0));
            Assert.AreEqual(0x00, assembler.memory.ReadMem(0x1));
        }

        [TestMethod]
        public void IFT_true()
        {
            var lines = new List<string>() { ".IF EQ,0", "  NOP", ".IFT", "  NOP", ".ENDC" };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0xEA, assembler.memory.ReadMem(0x0));
            Assert.AreEqual(0xEA, assembler.memory.ReadMem(0x1));
        }

        [TestMethod]
        public void IFT_nested()
        {
            var lines = new List<string>() {
                ".IF EQ,0", 
                "  NOP", 
                ".IFF", 
                "    TAX",
                "    .IF EQ,1",
                "        TAY",
                "        .IFF",
                "           CLC",
                "        .IFTF",
                "           CLD",
                "     .ENDC",
                ".ENDC" };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0xEA, assembler.memory.ReadMem(0x0),"0");
            Assert.AreEqual(0x00, assembler.memory.ReadMem(0x1),"1");
            Assert.AreEqual(0x00, assembler.memory.ReadMem(0x2), "2");
            Assert.AreEqual(0x00, assembler.memory.ReadMem(0x3), "3");
        }

        [TestMethod]
        public void IFT_false()
        {
            var lines = new List<string>() { ".IF EQ,1", "  NOP", ".IFT", "  NOP", ".ENDC" };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0x00, assembler.memory.ReadMem(0x0));
            Assert.AreEqual(0x00, assembler.memory.ReadMem(0x1));
        }

        [TestMethod]
        public void IF_NE_Nested()
        {
            var lines = new List<string>() { ".IF NE,1", "  NOP", ".IF NE,0", "  NOP", ".ENDC", ".ENDC" };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0xEA, assembler.memory.ReadMem(0x0));
            Assert.AreEqual(0x00, assembler.memory.ReadMem(0x1));
        }


        [TestMethod] 
        public void IF_NE_True()
        {
            var lines = new List<string>() { ".IF NE,1", "  NOP", ".ENDC" };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0xEA, assembler.memory.ReadMem(0x0));            
        }

        [TestMethod]
        public void IF_NE_TrueTabs()
        {
            var lines = new List<string>() { "\t.IF\tNE,1", "  NOP", ".ENDC" };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0xEA, assembler.memory.ReadMem(0x0));
        }


        [TestMethod]
        public void IF_NE_False()
        {
            var lines = new List<string>() { ".IF NE,0", "  NOP", ".ENDC" };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0x00, assembler.memory.ReadMem(0x0));
        }

        [TestMethod]
        public void IF_EQ_True()
        {
            var lines = new List<string>() { ".IF EQ,0", "  NOP", ".ENDC" };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0xEA, assembler.memory.ReadMem(0x0));
        }

        [TestMethod]
        public void IIF_True()
        {
            var lines = new List<string> {".IIF NDF,..SRC$,..SRC$ = 41."};
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            var symbol = assembler.symbolTable.GetSymbol("..SRC$");
            Assert.IsNotNull(symbol);
            Assert.AreEqual((double)41, symbol.NumericValue);
        }


        [TestMethod]
        public void IF_B_True()
        {
            var lines = new List<string>() { ".MACRO	ELSE COND", ".IF B,<COND>", "  NOP", ".ENDC", " .ENDM"," ELSE" };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0xEA, assembler.memory.ReadMem(0x0));
        }

        [TestMethod]
        public void IF_EQ_False()
        {
            var lines = new List<string>() { ".IF EQ,1", "  NOP", ".ENDC" };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0x00, assembler.memory.ReadMem(0x0));
        }


        [TestMethod]
        public void IF_Nested()
        {
            var lines = new List<string>() { ".IF NE,0", "  NOP", ".IF NE,1","  INX",".ENDC",".ENDC" };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0x00, assembler.memory.ReadMem(0x00));
            Assert.AreEqual(0x00, assembler.memory.ReadMem(0x01));
        }

        [TestMethod]
        public void Opcode_BCC_Backwards()
        {
            var lines = new List<string>() { ".=^H2000", "  NOP"," ADC #23", " BCC ^H2001" };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0xEA, assembler.memory.ReadMem(0x2000), "b0");
            Assert.AreEqual(0x69, assembler.memory.ReadMem(0x2001),"b1");
            Assert.AreEqual(0x17, assembler.memory.ReadMem(0x2002),"b2");
            Assert.AreEqual(0x90, assembler.memory.ReadMem(0x2003),"b3");
            Assert.AreEqual(0xFC, assembler.memory.ReadMem(0x2004),"b4");
        }

        [TestMethod]
        public void Opcode_BCC_BackwardsToLocal()
        {
            var lines = new List<string>() { ".=^H2000", "SCOPE1:", "10$:", "  NOP" ,"SCOPE2:","  NOP", "10$:"," ADC #23", " BCC 10$" };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0xEA, assembler.memory.ReadMem(0x2000), "b0");
            Assert.AreEqual(0xEA, assembler.memory.ReadMem(0x2001), "b1");
            Assert.AreEqual(0x69, assembler.memory.ReadMem(0x2002), "b2");
            Assert.AreEqual(0x17, assembler.memory.ReadMem(0x2003), "b3");
            Assert.AreEqual(0x90, assembler.memory.ReadMem(0x2004), "b4");
            Assert.AreEqual(0xFC, assembler.memory.ReadMem(0x2005), "b5");
        }

        [TestMethod]
        public void Opcode_BCC_ForwardsToLocal()
        {
            var lines = new List<string>() { ".=^H2000", "SCOPE1:", "10$:", "  NOP", "SCOPE2:", " BCC 10$", "  NOP", "10$:", " ADC #23" };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0xEA, assembler.memory.ReadMem(0x2000), "b0");
            Assert.AreEqual(0x90, assembler.memory.ReadMem(0x2001), "b1");
            Assert.AreEqual(0x01, assembler.memory.ReadMem(0x2002), "b2");
            Assert.AreEqual(0xEA, assembler.memory.ReadMem(0x2003), "b3");
            Assert.AreEqual(0x69, assembler.memory.ReadMem(0x2004), "b4");
            Assert.AreEqual(23, assembler.memory.ReadMem(0x2005), "b5");
        }


        [TestMethod]
        public void Opcode_JMP_Forward()
        {
            var lines = new List<string>() { ".=^H2000", "  JMP LABEL", " NOP", "LABEL:", "  NOP" };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0x4C, assembler.memory.ReadMem(0x2000), "b0");
            Assert.AreEqual(0x04, assembler.memory.ReadMem(0x2001), "b1");
            Assert.AreEqual(0x20, assembler.memory.ReadMem(0x2002), "b2");
            Assert.AreEqual(0xEA, assembler.memory.ReadMem(0x2003), "b3");
            Assert.AreEqual(0xEA, assembler.memory.ReadMem(0x2004), "b4");
        }

        [TestMethod]
        public void Assemble_MultiSegment()
        {
            var lines1 = new List<string>() { ".=^H2000", "  JMP LABEL", " NOP", };
            var lines2 = new List<string>() { ".=^H3000","LABEL:", "  NOP" };

            var segments = new List<ProgramSegment>();
            segments.Add(new ProgramSegment(lines1));
            segments.Add(new ProgramSegment(lines2));
            var assembler = new Assembler();
            assembler.StartAssemble(segments);

            Assert.AreEqual(0x4C, assembler.memory.ReadMem(0x2000), "b0");
            Assert.AreEqual(0x00, assembler.memory.ReadMem(0x2001), "b1");
            Assert.AreEqual(0x30, assembler.memory.ReadMem(0x2002), "b2");
            Assert.AreEqual(0xEA, assembler.memory.ReadMem(0x2003), "b3");
            Assert.AreEqual(0xEA, assembler.memory.ReadMem(0x3000), "b4");
        }

        [TestMethod]
        public void Opcode_BCC_Forwards()
        {
            var lines = new List<string>() { ".=^H2000", "  NOP", " BCC ^H2004"," ADC #23", "  NOP" };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0xEA, assembler.memory.ReadMem(0x2000), "b0");
            Assert.AreEqual(0x90, assembler.memory.ReadMem(0x2001), "b1");
            Assert.AreEqual(0x01, assembler.memory.ReadMem(0x2002), "b2");
            Assert.AreEqual(0x69, assembler.memory.ReadMem(0x2003), "b3");
            Assert.AreEqual(0x17, assembler.memory.ReadMem(0x2004), "b4");
        }

        //[TestMethod]
        public void Opcode_BCC_ForwardToLocal()
        {
            var lines = new List<string>() { ".=^H2000", "  NOP", " BCC 10$", " ADC #23","10$:"," NOP" };

            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0xEA, assembler.memory.ReadMem(0x2000), "b0");
            Assert.AreEqual(0x90, assembler.memory.ReadMem(0x2001), "b1");
            Assert.AreEqual(0x01, assembler.memory.ReadMem(0x2002), "b2");
            Assert.AreEqual(0x69, assembler.memory.ReadMem(0x2003), "b3");
            Assert.AreEqual(0x17, assembler.memory.ReadMem(0x2004), "b4");
        }

        [TestMethod]
        public void Opcode_NOP()
        {
            var lines = new List<string>() { "  NOP" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0xEA, assembler.memory.ReadMem(0));
        }

        [TestMethod]
        public void Opcode_ADC_Immediate()
        {
            var lines = new List<string>() { "  ADC #20" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0x69, assembler.memory.ReadMem(0));
            Assert.AreEqual(20, assembler.memory.ReadMem(1));
        }

        [TestMethod]
        public void Opcode_ADC_ZeroPage()
        {
            var lines = new List<string>() { "  ADC Z,20" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0x65, assembler.memory.ReadMem(0));
            Assert.AreEqual(20, assembler.memory.ReadMem(1));
        }

        [TestMethod]
        public void Opcode_ADC_IndirectY()
        {
            var lines = new List<string>() { "  LDA @10(Y)" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0xB1, assembler.memory.ReadMem(0));
            Assert.AreEqual(10, assembler.memory.ReadMem(1));
        }

        [TestMethod]
        public void Opcode_ADC_ZeroPageX()
        {
            var lines = new List<string>() { "  ADC ZX,20" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0x75, assembler.memory.ReadMem(0));
            Assert.AreEqual(20, assembler.memory.ReadMem(1));
        }


        [TestMethod]
        public void Opcode_STA_ZeroPage()
        {
            var lines = new List<string>() { "TEMP1::	.BLKB 2", "   STA X,TEMP1" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0x95, assembler.memory.ReadMem(2));
            Assert.AreEqual(0x00, assembler.memory.ReadMem(3));
        }



        [TestMethod]
        public void Opcode_STA_AbsoluteForward()
        {
            var lines = new List<string>() { "   STA RS.KEY+1" ,".=^HC000","RS.KEY:","NOP"};
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0x8D, assembler.memory.ReadMem(0x0));
            Assert.AreEqual(0x01, assembler.memory.ReadMem(0x1));
            Assert.AreEqual(0xC0, assembler.memory.ReadMem(0x2));
        }


        [TestMethod]
        public void Opcode_LSR_ZeroPage()
        {
            var lines = new List<string>() { "TEMP1::	.BLKB 2", "   LSR TEMP1" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0x46, assembler.memory.ReadMem(2));
            Assert.AreEqual(0x00, assembler.memory.ReadMem(3));
        }

        [TestMethod]
        public void GlobalLabel()
        {
            var lines = new List<string>() { "  NOP","test:" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            var symbol = assembler.symbolTable.GetSymbol("TEST");
            Assert.AreEqual(1, symbol.NumericValue, "NumericValue");
        }

        [TestMethod]
        public void GlobalLabel_withOpcode()
        {
            var lines = new List<string>() { "  NOP", "test:  NOP" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            var symbol = assembler.symbolTable.GetSymbol("TEST");
            Assert.AreEqual(1, symbol.NumericValue, "NumericValue");
        }

        [TestMethod]
        public void GlobalLabel_withOpcodeDoubleColon()
        {
            var lines = new List<string>() { "  NOP", "test::  NOP" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            var symbol = assembler.symbolTable.GetSymbol("TEST");
            Assert.AreEqual(1, symbol.NumericValue, "NumericValue");
        }

        [TestMethod]
        public void GlobalLabel_backwardsRefernce()
        {
            var lines = new List<string>() { "  NOP","TEST: ","NOP","  JMP TEST" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0xEA, assembler.memory.ReadMem(0), "Mem 0");
            Assert.AreEqual(0xEA, assembler.memory.ReadMem(1), "Mem 1");
            Assert.AreEqual(0x4C, assembler.memory.ReadMem(2), "Mem 2");
            Assert.AreEqual(0x01, assembler.memory.ReadMem(3), "Mem 3");
            Assert.AreEqual(0x00, assembler.memory.ReadMem(4), "Mem 4");
        }

     //   [TestMethod]
        public void GlobalLabel_forwardRefernce()
        {
            var lines = new List<string>() { "  NOP", "JMP TEST", "NOP", "  TEST:" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0xEA, assembler.memory.ReadMem(0), "Mem 0");
            Assert.AreEqual(0x4C, assembler.memory.ReadMem(1), "Mem 1");
            Assert.AreEqual(0x05, assembler.memory.ReadMem(2), "Mem 2");
            Assert.AreEqual(0x00, assembler.memory.ReadMem(3), "Mem 3");
            Assert.AreEqual(0xEA, assembler.memory.ReadMem(4), "Mem 4");
        }


        [TestMethod]
        public void LocalLabel_DefWithTab()
        {
            var lines = new List<string>() { "5$:	LDY #40" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            var symbol = assembler.symbolTable.GetSymbol(":5$");
            Assert.AreEqual(0, symbol.NumericValue, "NumericValue");

            Assert.AreEqual(0xA0, assembler.memory.ReadMem(0));
            Assert.AreEqual(40, assembler.memory.ReadMem(1));
        }

        [TestMethod]
        public void LocalLabel()
        {
            var lines = new List<string>() { "  NOP","10$:" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            var symbol = assembler.symbolTable.GetSymbol(":10$");
            Assert.AreEqual(1, symbol.NumericValue, "NumericValue");
        }

        [TestMethod]
        public void LocalLabel_secondColumn()
        {
            var lines = new List<string>() { "  NOP", " 10$:" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            var symbol = assembler.symbolTable.GetSymbol(":10$");
            Assert.AreEqual(1, symbol.NumericValue, "NumericValue");
        }



        [TestMethod] 
        public void Byte()
        {
            var lines = new List<string>() { ".byte 10,20,30" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(10, assembler.memory.ReadMem(0), "Mem 0");
            Assert.AreEqual(20, assembler.memory.ReadMem(1), "Mem 1");
            Assert.AreEqual(30, assembler.memory.ReadMem(2), "Mem 2");
        }

        [TestMethod]
        public void Ascii()
        {
            var lines = new List<string>() { ".ascii /ABC/" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(65, assembler.memory.ReadMem(0), "Mem 0");
            Assert.AreEqual(66, assembler.memory.ReadMem(1), "Mem 1");
            Assert.AreEqual(67, assembler.memory.ReadMem(2), "Mem 2");
        }

        [TestMethod]
        public void Word()
        {
            var lines = new List<string>() { ".word ^H1122,^H3344" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0x22, assembler.memory.ReadMem(0), "Mem 0");
            Assert.AreEqual(0x11, assembler.memory.ReadMem(1), "Mem 1");        
            Assert.AreEqual(0x44, assembler.memory.ReadMem(2), "Mem 2");
            Assert.AreEqual(0x33, assembler.memory.ReadMem(3), "Mem 3");
        }

        [TestMethod]
        public void Word_ForwardRef()
        {
            var lines = new List<string>() { ".=^h1000",".word label1,label2","label1:", "nop","label2:" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(0x04, assembler.memory.ReadMem(0x1000), "Mem 0");
            Assert.AreEqual(0x10, assembler.memory.ReadMem(0x1001), "Mem 1");
            Assert.AreEqual(0x05, assembler.memory.ReadMem(0x1002), "Mem 2");
            Assert.AreEqual(0x10, assembler.memory.ReadMem(0x1003), "Mem 3");
        }


        [TestMethod]
        public void Byte_Indented()
        {
            var lines = new List<string>() { "\t.byte 10,20,30" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(10, assembler.memory.ReadMem(0), "Mem 0");
            Assert.AreEqual(20, assembler.memory.ReadMem(1), "Mem 1");
            Assert.AreEqual(30, assembler.memory.ReadMem(2), "Mem 2");
        }


        [TestMethod]
        public void Assignment_PC()
        {
            var lines = new List<string>() { ".=100" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            var symbol = assembler.symbolTable.GetSymbol(".");
            Assert.AreEqual(100, symbol.NumericValue, "NumericValue Hex");
        }

      //  [TestMethod] 
        public void CommentLine()
        {
            var lines = new List<string>() { ";	FILENAME  CRF.MAC" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            Assert.AreEqual(lines[0], assembler.listHandler.Lines[0], "Lines[0]");
        }

        [TestMethod]
        public void Comment_LocalLabel()
        {
            var lines = new List<string>() { "5$:;[AND I,31.] " };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            var label = assembler.symbolTable.GetSymbol("5$","");
            Assert.AreEqual(0, label.NumericValue);
        }

        [TestMethod]
        public void Assignment_WithComment()
        {
            var lines = new List<string>() { "CG.PC=23   ;test" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            var symbol = assembler.symbolTable.GetSymbol("CG.PC");
            Assert.AreEqual(23, symbol.NumericValue, "NumericValue Hex");
        }

        [TestMethod]
        public void Assignment_ExplicitHex()
        {
            var lines = new List<string>() { "CG.PC=^H1" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            var symbol = assembler.symbolTable.GetSymbol("CG.PC");
            Assert.AreEqual(1, symbol.NumericValue, "NumericValue Hex");
        }

        [TestMethod]
        public void Assignment_ExplicitBinary()
        {
            var lines = new List<string>() { "CG.PC=^B01010101" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            var symbol = assembler.symbolTable.GetSymbol("CG.PC");
            Assert.AreEqual(0x55, symbol.NumericValue, "NumericValue Binary");
        }

        [TestMethod]
        public void Assignment_Radix10()
        {
            var lines = new List<string>() { "CG.PC=23" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            var symbol = assembler.symbolTable.GetSymbol("CG.PC");
            Assert.AreEqual(23, symbol.NumericValue, "NumericValue Hex");
        }

        [TestMethod]
        public void Assignment_Radix10Decimal()
        {
            var lines = new List<string>() { ".radix 16","CG.PC=23." };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            var symbol = assembler.symbolTable.GetSymbol("CG.PC");
            Assert.AreEqual(23, symbol.NumericValue, "NumericValue Hex");
        }

        [TestMethod]
        public void Assignment_ExpressionAdd()
        {
            var lines = new List<string>() { "CG.PC=10+20" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            var symbol = assembler.symbolTable.GetSymbol("CG.PC");
            Assert.AreEqual(30, symbol.NumericValue, "NumericValue Hex");
        }

        [TestMethod]
        public void Assignment_ExpressionAddWithSpaces()
        {
            var lines = new List<string>() { "CG.PC= 10 + 20" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            var symbol = assembler.symbolTable.GetSymbol("CG.PC");
            Assert.AreEqual(30, symbol.NumericValue, "NumericValue Hex");
        }

        [TestMethod]
        public void Assignment_ExpressionSubtract()
        {
            var lines = new List<string>() { "CG.PC=20-10" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            var symbol = assembler.symbolTable.GetSymbol("CG.PC");
            Assert.AreEqual(10, symbol.NumericValue, "NumericValue Hex");
        }

        [TestMethod]
        public void Assignment_ExpressionNegate()
        {
            var lines = new List<string>() { "CG.PC=^H4321&-256/256" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            var symbol = assembler.symbolTable.GetSymbol("CG.PC");
            Assert.AreEqual(0x43, symbol.NumericValue, "NumericValue Hex");
        }

        [TestMethod]
        public void Assignment_ExpressionMultiply()
        {
            var lines = new List<string>() { "CG.PC=20*10" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            var symbol = assembler.symbolTable.GetSymbol("CG.PC");
            Assert.AreEqual(200, symbol.NumericValue, "NumericValue Hex");
        }

        [TestMethod]
        public void Assignment_ExpressionDivide()
        {
            var lines = new List<string>() { "CG.PC=30/10" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            var symbol = assembler.symbolTable.GetSymbol("CG.PC");
            Assert.AreEqual(3, symbol.NumericValue, "NumericValue Hex");
        }

        [TestMethod]
        public void Assignment_ExpressionAnd()
        {
            var lines = new List<string>() { "CG.PC=^HFF&^H05" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            var symbol = assembler.symbolTable.GetSymbol("CG.PC");
            Assert.AreEqual(5, symbol.NumericValue, "NumericValue Hex");
        }

        [TestMethod]
        public void Assignment_ExpressionOr()
        {
            var lines = new List<string>() { "CG.PC=^H50!^H05" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            var symbol = assembler.symbolTable.GetSymbol("CG.PC");
            Assert.AreEqual(0x55, symbol.NumericValue, "NumericValue Hex");
        }

        [TestMethod]
        public void Assignment_ExpressionXor()
        {
            var lines = new List<string>() { @"CG.PC=^H11\^H33" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            var symbol = assembler.symbolTable.GetSymbol("CG.PC");
            Assert.AreEqual(0x22, symbol.NumericValue, "NumericValue Hex");
        }

        [TestMethod]
        public void Assignment_SubExpression()
        {
            var lines = new List<string>() { "CG.PC=5*<3+1>" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            var symbol = assembler.symbolTable.GetSymbol("CG.PC");
            Assert.AreEqual(20, symbol.NumericValue, "NumericValue Hex");
        }

        [TestMethod]
        public void Assignment_NestedSubExpression()
        {
            var lines = new List<string>() { "CG.PC=5*<3+<2*2>>" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            var symbol = assembler.symbolTable.GetSymbol("CG.PC");
            Assert.AreEqual(35, symbol.NumericValue, "NumericValue Hex");
        }

        [TestMethod]
        public void Assignment_MultiSub()
        {
          
           var lines = new List<string>() {"EMCTRS=5", "test=<EMCTRS>*<EMCTRS-1>*<EMCTRS-3>*<EMCTRS-4>" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            var symbol = assembler.symbolTable.GetSymbol("TEST");
            Assert.AreEqual(40, symbol.NumericValue, "NumericValue Hex");
        }

        [TestMethod]
        public void Assignment_Symbol()
        {
            var lines = new List<string>() { "SYM1=5","CG.PC=SYM1" };
            var assembler = new Assembler();
            assembler.AssembleLines(lines);

            var symbol = assembler.symbolTable.GetSymbol("CG.PC");
            Assert.AreEqual(5, symbol.NumericValue, "NumericValue Hex");
        }

    }
}
