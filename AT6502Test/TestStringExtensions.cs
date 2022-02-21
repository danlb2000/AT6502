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
    public class TestStringExtensions
    {
        [TestMethod]
        public void RemoveString()
        {
            Assert.AreEqual("cd", "abcd".RemoveString("ab"));
        }

        [TestMethod]
        public void GetUntil()
        {
            Assert.AreEqual("a", "a!".GetUntil('!'));
            Assert.AreEqual("", "ab".GetUntil('!'));
            Assert.AreEqual("", "".GetUntil('!'));
        }

        [TestMethod]
        public void GetUntilLast()
        {
            Assert.AreEqual("<ab<cd>ef", "<ab<cd>ef>g".GetUntilLast('>'));
            Assert.AreEqual("", "".GetUntil('!'));
        }

        [TestMethod]
        public void Match_Single()
        {
            var TestTerminal = new StringTerminal("test");
            var match = "test".Match(TestTerminal);
            Assert.IsTrue(match.Matched, "Matched");
            Assert.AreEqual("test", match.MatchString, "MatchString");
        }    
        
   
        [TestMethod]
        public void Match_Multiple()
        {
            var testTerminal = new StringTerminal("test");
            var whiteSpaceTerminal = new WhiteSpaceTerminal();
            var matchSet = new TerminalSet(whiteSpaceTerminal, testTerminal);
            var match = " test".Match(matchSet);
            Assert.IsTrue(match.Matched, "Matched");
            Assert.AreEqual(" test", match.MatchString, "MatchString");

            match = "test".Match(matchSet);
            Assert.IsFalse(match.Matched, "Matched");

            match = "  ".Match(matchSet);
            Assert.IsFalse(match.Matched, "Matched");
        }
    }
}
