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
    public class TestCharacterSetTerminal
    {
        [TestMethod]
        public void MatchString_OneOrMore()
        {
            var set = new CharacterSet("{alphanumeric}");
            var segment = new CharacterSetTerminal(CharacterSetTerminal.MatchType.OneOrMore, set);
            var match = segment.MatchString("abcd!");
            Assert.IsTrue(match.Matched, "Matched");
            Assert.AreEqual("abcd", match.MatchString, "MatchString");
            Assert.AreEqual(3, match.MatchEndPosition, "MatchEndPosition");

            match = segment.MatchString("abcd!efg",false,5);
            Assert.IsTrue(match.Matched, "Matched");
            Assert.AreEqual("efg", match.MatchString, "MatchString");
            Assert.AreEqual(7, match.MatchEndPosition, "MatchEndPosition");

            match = segment.MatchString("!abcd");
            Assert.IsFalse(match.Matched, "Matched");
            Assert.AreEqual("", match.MatchString, "MatchString");
            Assert.AreEqual(0, match.MatchEndPosition);
        }

        [TestMethod] 
        public void MatchString_OneOrMoreIgnoreWhitespace()
        {
            var set = new CharacterSet("{alphanumeric}");
            var termSet = new CharacterSetTerminal(CharacterSetTerminal.MatchType.OneOrMore, set);
            var match = termSet.MatchString("abcd ef!",true);
            Assert.IsTrue(match.Matched, "Matched");
            Assert.AreEqual("abcd ef", match.MatchString, "MatchString");
            Assert.AreEqual(6, match.MatchEndPosition, "MatchEndPosition");
        }

        [TestMethod]
        public void MatchString_ExactlyOne()
        {
            var set = new CharacterSet("[!]");
            var segment = new CharacterSetTerminal(CharacterSetTerminal.MatchType.ExactlyOne, set);
            var match = segment.MatchString("!abcd");
            Assert.IsTrue(match.Matched, "Matched");
            Assert.AreEqual("!", match.MatchString, "MatchString");
            Assert.AreEqual(0, match.MatchEndPosition, "MatchEndPosition");

            match = segment.MatchString("!!abcd");
            Assert.IsTrue(match.Matched, "Matched");
            Assert.AreEqual("!", match.MatchString, "MatchString");
            Assert.AreEqual(0, match.MatchEndPosition, "MatchEndPosition");

            match = segment.MatchString("ab!cd", false, 2);
            Assert.IsTrue(match.Matched, "Matched");
            Assert.AreEqual("!", match.MatchString, "MatchString");
            Assert.AreEqual(2, match.MatchEndPosition, "MatchEndPosition");

            match = segment.MatchString("abcd!");
            Assert.IsFalse(match.Matched, "Matched");
            Assert.AreEqual("", match.MatchString, "MatchString");
        }

        [TestMethod]
        public void MatchString_ZeroOrMore()
        {
            var set = new CharacterSet("[abcde]");
            var segment = new CharacterSetTerminal(CharacterSetTerminal.MatchType.ZeroOrMore, set);
            var match = segment.MatchString("abcd");
            Assert.IsTrue(match.Matched, "Matched");
            Assert.AreEqual("abcd", match.MatchString, "MatchString");
            Assert.AreEqual(3, match.MatchEndPosition, "MatchEndPosition");

             match = segment.MatchString("abcd", false, 2);
            Assert.IsTrue(match.Matched, "Matched");
            Assert.AreEqual("cd", match.MatchString, "MatchString");
            Assert.AreEqual(3, match.MatchEndPosition, "MatchEndPosition");

            match = segment.MatchString("!abcd");
            Assert.IsTrue(match.Matched, "Matched");
            Assert.AreEqual("", match.MatchString, "MatchString");
        }

        [TestMethod]
        public void MatchString_ZeroOrOne()
        {
            var set = new CharacterSet("[abcde]");
            var segment = new CharacterSetTerminal(CharacterSetTerminal.MatchType.ZeroOrOne, set);
            var match = segment.MatchString("abcd");
            Assert.IsTrue(match.Matched, "Matched");
            Assert.AreEqual("a", match.MatchString, "MatchString");
            Assert.AreEqual(0, match.MatchEndPosition, "MatchEndPosition");

            match = segment.MatchString("abcd", false, 2);
            Assert.IsTrue(match.Matched, "Matched");
            Assert.AreEqual("c", match.MatchString, "MatchString");
            Assert.AreEqual(2, match.MatchEndPosition, "MatchEndPosition");


            match = segment.MatchString("!abcd");
            Assert.IsTrue(match.Matched, "Matched");
            Assert.AreEqual("", match.MatchString, "MatchString");
        }



    }
}
