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
    public class TestStringTerminal
    {
        [TestMethod]
        public void MatchString_Pass()
        {
            var term = new StringTerminal("test");
            var match = term.MatchString("test: line");
            Assert.IsTrue(match.Matched, "Matched");
            Assert.AreEqual("test", match.MatchString, "MatchString");
            Assert.AreEqual(3, match.MatchEndPosition, "MathEndPosition");
        }

        [TestMethod]
        public void MatchString_PassWithStart()
        {
            var term = new StringTerminal("test");
            var match = term.MatchString("line: test", false, 6);
            Assert.IsTrue(match.Matched, "Matched");
            Assert.AreEqual("test", match.MatchString, "MatchString");
            Assert.AreEqual(9, match.MatchEndPosition, "MathEndPosition");
        }

        [TestMethod]
        public void MatchString_Fail()
        {
            var term = new StringTerminal("test");
            var match = term.MatchString("tes: line");
            Assert.IsFalse(match.Matched, "Matched");
            Assert.AreEqual("", match.MatchString, "MatchString");
            Assert.AreEqual(0, match.MatchEndPosition, "MathEndPosition");
        }
    }
}
