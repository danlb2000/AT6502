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


namespace AT6502Test
{

    [TestClass]
    public class TestOpcodes6502
    {
        [TestMethod]
        public void DetermineAddressingMode_Explicit()
        {
            var opcodes = new Opcodes6502();            
            Assert.AreEqual("RE", opcodes.DetermineAddressingMode("BCC", "30",0),"Relative");
            Assert.AreEqual("IP", opcodes.DetermineAddressingMode("NOP", "",0), "Implied");
            Assert.AreEqual("IM", opcodes.DetermineAddressingMode("ADC", "#10",0), "Immediate");
            Assert.AreEqual("ZP", opcodes.DetermineAddressingMode("ADC", "Z,10",10), "Zero Page");
            Assert.AreEqual("ZX", opcodes.DetermineAddressingMode("ADC", "ZX,10",10), "Zero Page X");
            Assert.AreEqual("AX", opcodes.DetermineAddressingMode("ADC", "X,^H1000",0x1000), "Absolute X");
            Assert.AreEqual("AY", opcodes.DetermineAddressingMode("ADC", "Y,^H1000", 0x1000), "Absolute Y");
            Assert.AreEqual("ZX", opcodes.DetermineAddressingMode("ADC", "X,^H10", 0x10), "Zero Page X");
            Assert.AreEqual("ZY", opcodes.DetermineAddressingMode("ADC", "Y,^H10", 0x10), "Zero Page Y");
            Assert.AreEqual("AX", opcodes.DetermineAddressingMode("ADC", "10(X)",0x10), "Absolute X");
            Assert.AreEqual("AY", opcodes.DetermineAddressingMode("ADC", "10(Y)",0x10), "Absolute y");
            Assert.AreEqual("AB", opcodes.DetermineAddressingMode("JMP", "0x10", 0x10), "Absolute");

            Assert.AreEqual("IX", opcodes.DetermineAddressingMode("LDA", "@TEMP(X)", 0x10), "Indirect X");
            Assert.AreEqual("IY", opcodes.DetermineAddressingMode("LDA", "@TEMP(Y)", 0x10), "Indirect Y");
        }

        [TestMethod]
        public void GetOpcode()
        {
            var opcodes = new Opcodes6502();
            Assert.AreEqual(0x90, opcodes.GetOpcode("BCC", "RE"));
        }

        [TestMethod]
        public void RemoveAddressModeIdentifier()
        {
            var opcodes = new Opcodes6502();
            Assert.AreEqual("20", opcodes.RemoveAddressModeIdentifier("#20"));
            Assert.AreEqual("TEST", opcodes.RemoveAddressModeIdentifier("X,TEST"));
            Assert.AreEqual("TEST", opcodes.RemoveAddressModeIdentifier("TEST(X)"));
        }
    }
}
