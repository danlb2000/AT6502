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
using System.Text;

namespace AT6502
{
    public class Assembler
    {
        public ListHandler listHandler = new ListHandler();
        public SymbolTable symbolTable = new SymbolTable();
        public Memory memory = new Memory();
        private Opcodes6502 opcodes6502 = new Opcodes6502();

        static WhiteSpaceTerminal whiteSpaceTerminal = new WhiteSpaceTerminal();
        static CharacterSet IdentifierStart = new CharacterSet("{Letter}", "[.$?]");
        static CharacterSet IdentifierCharacters = new CharacterSet("{AlphaNumeric}", "[.$']");
        static CharacterSet HexCharacters = new CharacterSet("[0123456789ABCDEF]");
        static CharacterSet BinaryCharacters = new CharacterSet("[01]");

        static CharacterSetTerminal Parameter = new CharacterSetTerminal(CharacterSetTerminal.MatchType.OneOrMore, new CharacterSet("{AlphaNumeric}"));
        static TerminalSet GlobalLabel = new TerminalSet(new CharacterSetTerminal(CharacterSetTerminal.MatchType.OneOrMore, IdentifierStart),
                                                              new CharacterSetTerminal(CharacterSetTerminal.MatchType.ZeroOrMore, IdentifierCharacters));

        static TerminalSet MacroName = new TerminalSet(new CharacterSetTerminal(CharacterSetTerminal.MatchType.OneOrMore, IdentifierStart),
                                                           new CharacterSetTerminal(CharacterSetTerminal.MatchType.ZeroOrMore, IdentifierCharacters));

        static TerminalSet LocalLabel = new TerminalSet(new CharacterSetTerminal(CharacterSetTerminal.MatchType.OneOrMore, new CharacterSet("{Number}")),
                                                      new StringTerminal("$"));

        static TerminalSet PsuedoOp = new TerminalSet(new StringTerminal("."), new CharacterSetTerminal(CharacterSetTerminal.MatchType.OneOrMore, new CharacterSet("{Letter}")));
        static TerminalSet NonTypedNumber = new TerminalSet(new CharacterSetTerminal(CharacterSetTerminal.MatchType.ExactlyOne, new CharacterSet("{number}")),
                                                            new CharacterSetTerminal(CharacterSetTerminal.MatchType.ZeroOrMore, new CharacterSet("[0123456789ABCDEF.")));
        static TerminalSet HexValue = new TerminalSet(new StringTerminal("^H"), new CharacterSetTerminal(CharacterSetTerminal.MatchType.OneOrMore, HexCharacters));
        static TerminalSet BinaryValue = new TerminalSet(new StringTerminal("^B"), new CharacterSetTerminal(CharacterSetTerminal.MatchType.OneOrMore, BinaryCharacters));

        static CharacterSetTerminal Comment = new CharacterSetTerminal(CharacterSetTerminal.MatchType.ExactlyOne, new CharacterSet("[;]"));
        static CharacterSetTerminal EqualsCharacter = new CharacterSetTerminal(CharacterSetTerminal.MatchType.ExactlyOne, new CharacterSet("[=]"));

        static StringTerminal PcIdentifier = new StringTerminal(".");
        static StringTerminal LabelDeliminter = new StringTerminal(":");

        static StringTerminal IrpTerminal = new StringTerminal(".IRP");
        static StringTerminal ReptTerminal = new StringTerminal(".REPT");
        static StringTerminal EndrTerminal = new StringTerminal(".ENDR");
        static TerminalSet IfTerminal = new TerminalSet(new StringTerminal(".IF"), new WhiteSpaceTerminal());
        static StringTerminal IIfTerminal = new StringTerminal(".IIF");
        static StringTerminal EndcTerminal = new StringTerminal(".ENDC");
        static StringTerminal IffTerminal = new StringTerminal(".IFF");
        static StringTerminal IftTerminal = new StringTerminal(".IFT");
        static StringTerminal IftfTerminal = new StringTerminal(".IFTF");
        static StringTerminal MacroTerminal = new StringTerminal(".MACRO");
        static StringTerminal EndMacroTerminal = new StringTerminal(".ENDM");

        char[] listSeparators = new char[] { ',', ' ', '\t' };

        int radix = 10;
        int pc = 0;
        int macroLocal = 0;
        bool hasError = false;

        public int PC
        {
            get { return pc; }
            set
            {
                pc = value;
                symbolTable.UpdateSymbolValue(".", pc);
            }
        }

        ProgramSegment programSegment;
        string currentScope = "";
        bool secondPass = false;

        Stack<ProgramSegment> segmentStack = new Stack<ProgramSegment>();
        Stack<ConditionState> conditionStack = new Stack<ConditionState>();
        Stack<Repeat> repeatStack = new Stack<Repeat>();
        bool repeatZero = false;

        Dictionary<string, Macro> macros = new Dictionary<string, Macro>();
        Dictionary<string, AssemblerStack> stacks = new Dictionary<string, AssemblerStack>();

        public AssemblerStack GetStack(string name)
        {
            return stacks[name];
        }

        public Assembler()
        {
            symbolTable.AddSymbol(Symbol.NewPcSymbol());
        }

        public List<string> LoadFile(string filename)
        {
            var lines = new List<string>();

            if (!File.Exists(filename))
            {
                RaiseAssemblerException($"File {filename} not found", true);
            }

            try
            {
                using (var sr = new StreamReader(filename))
                {
                    while (true)
                    {
                        var line = sr.ReadLine();
                        if (line == null) break;
                        var line2 = line.Trim();
                        if (line2.Length > 0 && line2.Trim()[0] == 0) continue;
                        lines.Add(line);
                    }
                    sr.Close();
                }
            }
            catch (Exception ex)
            {
                RaiseAssemblerException($"Could not read {filename}. " + ex.Message, true);
            }

            return lines;
        }

        public void StartAssemble(List<string> fileNames)
        {
            var segments = new List<ProgramSegment>();
            foreach (string filename in fileNames)
            {
                var lines = LoadFile(filename);
                var segment = new ProgramSegment(lines);
                segment.Filename = filename;
                segments.Add(segment);
            }

            StartAssemble(segments);
        }

        public void AssembleLines(List<string> lines)
        {
            var segments = new List<ProgramSegment>();
            var segment = new ProgramSegment(lines);
            segment.Filename = "";
            segments.Add(segment);
            StartAssemble(segments);
        }

        public void StartAssemble(List<ProgramSegment> segments)
        {
            listHandler.EnableList = false;
            for (int pass = 0; pass < 2; pass++)
            {
                macroLocal = 30000;
                foreach (ProgramSegment segment in segments) { 
                    Assemble(segment);
                }
                if (hasError) return;
                listHandler.EnableList = true;
                listHandler.AddMessage("PASS 2");
                secondPass = true;
            }
        }

        private void Assemble(ProgramSegment segment)
        {
            int lastPC = 0;

            programSegment = segment;
            programSegment.Reset();
            currentScope = "";
            PC = 0;
            radix = 10;
            stacks = new Dictionary<string, AssemblerStack>();
            repeatZero = false;
    
            while (true)
            {
                lastPC = PC;

                string line = "";
                try
                {
                    line = AssembleLine();
                }
                catch (AssembleException ex)
                {
                    listHandler.AddMessage(programSegment.CurrentLine);
                    listHandler.AddError(ex);
                    hasError = true;
                    if (ex.Fatal) break;
                }

                if (line == null) break;

                var bytes = memory.PopBytesWritten();
                listHandler.AddLine(programSegment.Filename, programSegment.FileLineNumber, lastPC, bytes, line);
            }
        }

        public string NextLine()
        {
            string line = "";
            string comment = "";
            string originalLine = "";

            while (true)
            {
                line = programSegment.NextLine();
                if (line == null)
                {
                    if (segmentStack.Count == 0) return null;
                    programSegment = segmentStack.Pop();
                    continue;
                }

                originalLine = line;

                // Full line comment
                if (line.Match(Comment).Matched ||
                    line.Match(whiteSpaceTerminal, Comment).Matched)
                {
                    break;
                }

                // Remove comment from the end of the line
                int commentPosition = line.IndexOf(";");
                if (commentPosition != -1)
                {
                    comment = line.Substring(commentPosition);
                    line = line.Substring(0, commentPosition - 1).Trim();
                }

                if (line.Trim().Match(EndrTerminal).Matched ||
                line.Trim().Match(EndMacroTerminal).Matched)
                {
                    AssembleEndRepeat();
                    continue;
                }

                if (line.Trim().Match(ReptTerminal).Matched)
                {
                    AssembleRepeat(line);
                    continue;
                }

                if (repeatZero) continue;

                // End of condition block
                if (line.Trim().ToUpper().Match(EndcTerminal).Matched)
                {
                    PreprocessEndc(line);
                    continue;
                }

                if (line.Trim().ToUpper().Match(IfTerminal).Matched)
                {
                    PreprocessIf(line);
                    continue;
                }

                if (conditionStack.Count != 0 && conditionStack.Peek().DoNotAssembler)
                {
                    continue;
                }

                // Sub conditionals
                if (line.Trim().Match(IftTerminal).Matched ||
                  line.Trim().Match(IffTerminal).Matched ||
                  line.Trim().Match(IftfTerminal).Matched)
                {
                    ProcessSubCondition(line);
                    continue;
                }

                if (conditionStack.Count != 0 && !conditionStack.Peek().EvalState)
                {
                    continue;
                }



                line = HandleLineReplace(line);

                if (line.Trim().Match(IrpTerminal).Matched)
                {
                    AssembleIrp(line);
                    continue;
                }

                if (line.Trim().Match(MacroTerminal).Matched)
                {
                    AssembleMacroDefinition(line);
                    continue;
                }

                if (line.Trim().Match(IIfTerminal).Matched)
                {
                    var result = PreProcessIif(line);
                    if (result != "")
                        originalLine = result;
                    else
                        originalLine = "";

                }

                break;
            }

            return originalLine;
        }

        public string HandleLineReplace(string line)
        {
            if (repeatStack.Count > 0 && repeatStack.Peek().Type == Repeat.RepeatType.replace)
            {
                var repeat = repeatStack.Peek();
                line = ReplaceToken(line, repeat.Placeholder, repeat.ReplaceList[(int)repeat.Count.Value]);
            }

            return line;
        }

        public string ReplaceToken(string line, string placeholder, string replaceWith)
        {
            var tokens = Tokenize(line);
            var result = new StringBuilder();
            for (int i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];
                if (token == placeholder)
                {
                    result.Append(replaceWith);
                    if (i < tokens.Count - 1 && tokens[i + 1] == "'") i++;
                }
                else
                {
                    if (token != "'" || i >= tokens.Count - 1 || !tokens[i + 1].Match(Parameter).Matched)
                    {
                        result.Append(token);
                    }
                }
            }

            return result.ToString();
        }

        public static List<string> Tokenize(string s)
        {
            var identifierCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLKMNOPQRSTUVWXYZ0123456789.";
            var tokens = new List<string>();
            var currentToken = new StringBuilder();

            if (s.Trim() == "") return tokens;

            bool isIdentifier = false;
            if (identifierCharacters.Contains(s[0].ToString()))
                isIdentifier = true;

            foreach (char c in s)
            {
                if (identifierCharacters.Contains(c.ToString()) && !isIdentifier ||
                    !identifierCharacters.Contains(c.ToString()) && isIdentifier)
                {
                    isIdentifier = !isIdentifier;
                    tokens.Add(currentToken.ToString());
                    currentToken = new StringBuilder();
                }

                currentToken.Append(c);
            }

            if (currentToken.ToString() != "") tokens.Add(currentToken.ToString());
            return tokens;
        }

        public Macro GetMacro(string name)
        {
            if (!macros.ContainsKey(name)) return null;
            return macros[name];
        }

        public void RaiseAssemblerException(string message, bool fatal = false)
        {
            throw new AssembleException(message, programSegment.FileLineNumber, programSegment.Filename, programSegment.CurrentLine, fatal); ;
        }

        public void AssembleMacro(string line)
        {
            listHandler.AddMacroCall(programSegment.FileLineNumber, PC, line);

            line = line.Trim();
            var match = line.Match(MacroName);
            if (!match.Matched) throw new Exception("Sytnax error parsing macro definiton");

            // Get macro definition
            var name = match.MatchString;
            line = line.RemoveString(name).Trim();

            if (!macros.ContainsKey(name))
            {
                RaiseAssemblerException($"Unknow macro {name}");
            }
            var macro = macros[name];

            // Get parameters
            string[] parameters = new string[macro.Parameters.Count];
            string[] passedParameters;

            if (line != "")
            {
                passedParameters = line.Split(listSeparators, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < macro.Parameters.Count; i++)
                {
                    if (i <= passedParameters.GetUpperBound(0))
                    {
                        parameters[i] = passedParameters[i];
                    }
                    else
                    {
                        if (macro.Parameters[i].StartsWith("?"))
                        {
                            parameters[i] = macroLocal.ToString() + "$";
                            macroLocal++;
                        }
                        else
                        {
                            parameters[i] = "";
                        }
                    }
                }
            }

            // Expand macro
            var expandedLines = new List<string>();
            foreach (string macroLine in macro.Lines)
            {
                string expandLine = macroLine;

                // Replace parameters
                for (int index = 0; index < macro.Parameters.Count; index++)
                {
                    var placeholder = macro.Parameters[index].TrimStart('?');
                    expandLine = ReplaceToken(expandLine, placeholder, parameters[index]);
                }

                expandedLines.Add(expandLine);
            }

            segmentStack.Push(programSegment);
            programSegment = new ProgramSegment(expandedLines);
        }

        public void AssembleIrp(string line)
        {
            line = line.Trim().RemoveString(".IRP").Trim();
            var placeholder = line.GetUntil(',');
            line = line.RemoveString(placeholder).TrimStart(',').Trim();
            line = line.TrimStart('<').TrimEnd('>');
            var replaceList = line.Split(',');

            var repeat = new Repeat(programSegment.CurrentLineNumber, placeholder, replaceList);
            repeatStack.Push(repeat);
        }

        public void AssembleRepeat(string line)
        {
            line = line.Trim().RemoveString(".REPT").Trim();

            double? count = EvaluateExpression(line);
            int start = programSegment.CurrentLineNumber;
            if (repeatZero) count = 0;
            if (count == 0) repeatZero = true;

            var repeat = new Repeat(start, count);
            repeatStack.Push(repeat);
        }

        public void AssembleEndRepeat()
        {
            if (repeatStack.Count == 0) return;

            var repeat = repeatStack.Peek();

            if (repeat.Type == Repeat.RepeatType.count)
            {
                repeat.Count--;

                if (repeat.Count <= 0)
                {
                    repeatStack.Pop();
                    repeatZero = false;
                    if (repeatStack.Count > 0 && repeatStack.Peek().Type == Repeat.RepeatType.count && repeatStack.Peek().Count == 0) repeatZero = true;
                }
                else
                {
                    programSegment.CurrentLineNumber = repeat.StartLine;
                }
            }
            else
            {
                repeat.Count++;
                if (repeat.Count > repeat.ReplaceList.GetUpperBound(0))
                {
                    repeatStack.Pop();
                }
                else
                {
                    programSegment.CurrentLineNumber = repeat.StartLine;
                }
            }

        }

        public void AssembleMacroDefinition(string line)
        {
            int nestingCount = 0;

            line = line.Trim().RemoveString(".MACRO").Trim();
            var match = line.Match(MacroName);
            var name = match.MatchString;

            line = line.RemoveString(name).Trim();

            var macro = new Macro();
            macro.Name = name;
            if (line != "")
            {
                var parameters = line.Split(listSeparators);
                foreach (string s in parameters)
                {
                    if (s != "") macro.Parameters.Add(s);
                }
            }


            if (!macros.ContainsKey(macro.Name))
            {
                macros.Add(macro.Name, macro);
            }

            while (true)
            {
                var macroLine = programSegment.NextLine();
                if (macroLine == null) break;

                macroLine = HandleLineReplace(macroLine);

                if (macroLine.Trim().Match(MacroTerminal).Matched)
                {
                    nestingCount++;
                }

                if (macroLine.Trim().Match(EndMacroTerminal).Matched)
                {
                    nestingCount--;
                    if (nestingCount == -1) break;
                }
                macro.Lines.Add(macroLine);
            }
        }

        public void ProcessSubCondition(string line)
        {
            string originalLine = line;

            if (conditionStack.Peek().IsSubCondition) conditionStack.Pop();

            var condition = new ConditionState();
            condition.Line = line;
            condition.LineNumber = programSegment.CurrentLineNumber - 1;
            condition.IsSubCondition = true;

            var previousState = conditionStack.Peek().EvalState;
            conditionStack.Push(condition);

            line = line.Trim();
            condition.EvalState = false;
            if ((line == ".IFT" || line == ".IFTF") && previousState == true) condition.EvalState = true;
            if ((line == ".IFF" || line == ".IFTF") && previousState == false) condition.EvalState = true;

            listHandler.AddPsuedoOp(originalLine + $" [{condition.EvalState.ToString()}]");
        }

        public void PreprocessEndc(string line)
        {
            while (conditionStack.Peek().IsSubCondition)
            {
                conditionStack.Pop();
            }
            conditionStack.Pop();

            listHandler.AddPsuedoOp(line);
        }

        public string PreProcessIif(string line)
        {
            line = line.Trim().RemoveString(".IIF").Trim();
            string[] parts = line.Split(',');

            var result = EvaluateCondition(parts[0], parts[1]);
            if (result) return parts[2];
            return "";
        }

        public void PreprocessIf(string line)
        {
            string originalLine = line;

            line = line.Trim().RemoveString(".IF").Trim();
            string[] parts = line.Split(',');

            var condition = new ConditionState();
            condition.Line = originalLine;
            condition.LineNumber = programSegment.CurrentLineNumber - 1;
            condition.EvalState = EvaluateCondition(parts[0], parts[1]);

            if (conditionStack.Count > 0 && !conditionStack.Peek().EvalState)
            {
                condition.EvalState = false;
                condition.DoNotAssembler = true;
            }
            conditionStack.Push(condition);

            listHandler.AddPsuedoOp(originalLine + $" [{condition.EvalState.ToString()}]");
        }

        private bool EvaluateCondition(string condition, string expression)
        {
            double? value;

            value = EvaluateExpression(expression, true);

            switch (condition.ToUpper())
            {
                case "NE":  // Not equal to zero 
                    if (value != 0) return true;
                    break;
                case "EQ":  // Equal to zero                  
                    if (value == 0) return true;
                    break;
                case "GE":  // Greater than or equal to zero
                    if (value >= 0) return true;
                    break;
                case "GT":  // Greater than
                    if (value > 0) return true;
                    break;
                case "LT":  // Less than
                    if (value < 0) return true;
                    break;
                case "LE":  // Less than or equal to zero
                    if (value <= 0) return true;
                    break;
                case "NDF": // Not defined
                    if (value == null) return true;
                    return false;
                case "IDN": // Identical
                    break;
                case "NB":  // Not blank
                    if (value.HasValue) return true;
                    break;
                case "B":   // Blank
                    if (value == null) return true;
                    break;
                default:
                    RaiseAssemblerException("Unknown comparison type " + condition);
                    break;
            }
            return false;
        }



        public string AssembleLine()
        {
            var comment = "";
            Match match = null;
            string lineStart;

            var line = NextLine();
            if (line == null) return null;

            if (line.Trim() == "") return "";

            lineStart = line;

            // Full line comment
            if (line.Match(Comment).Matched ||
                line.Match(whiteSpaceTerminal, Comment).Matched)
            {
                ProcessComment(line);
                return lineStart;
            }

            // Remove comment from the end of the line
            int commentPosition = line.IndexOf(";");
            if (commentPosition != -1)
            {
                comment = line.Substring(commentPosition);
                line = line.Substring(0, commentPosition).Trim();
            }

            // Handle line label
            if (line.StartsWith(" ")) line = line.Substring(1);
            if ((match = line.Match(GlobalLabel, LabelDeliminter)).Matched)
            {
                AssembleGlobalLabel(match.MatchString);
                line = line.RemoveString(match.MatchString).Trim();
                line = line.TrimStart(':');
                if (line == "") return lineStart;
            }

            if ((match = line.Match(LocalLabel, LabelDeliminter)).Matched)
            {
                AssembleLocalLabel(match.MatchString);
                line = line.RemoveString(match.MatchString).Trim();
                line = line.TrimStart(':');
                if (line == "") return lineStart;
            }

            // Assignment
            if (line.MatchIgnoreWhitespace(GlobalLabel, EqualsCharacter).Matched ||
                line.MatchIgnoreWhitespace(PcIdentifier, EqualsCharacter).Matched)
            {
                AssembleAssignment(line);
                return lineStart;
            }

            line = line.Trim();
            // PseudoOps
            if (line.Match(PsuedoOp).Matched)
            {
                AssemblePseudoOp(line);
                return lineStart;
            }

            // Opcodes or Macros
            AssembleOpCodesOrMacros(line);
            return lineStart;
        }

        public void AssembleOpCodesOrMacros(string line)
        {
            line = line.Trim();
            if (line.Length >= 3)
            {
                var opcode = line.Substring(0, 3);
                if (opcodes6502.IsValidOpcode(opcode) && (line.Length == 3 || string.IsNullOrWhiteSpace(line.Substring(3, 1))))
                {
                    AssembleOpCode(line);
                    return;
                }
            }

            AssembleMacro(line);
        }

        private void AssembleOpCode(string line)
        {
            var opName = line.Substring(0, 3);
            line = line.RemoveString(opName).Trim();

            var operand = opcodes6502.RemoveAddressModeIdentifier(line).Trim();

            double? value = 0;
            if (operand != "")
            {
                value = EvaluateExpression(operand, true);
            };

            var addrMode = opcodes6502.DetermineAddressingMode(opName, line, value);
            if (addrMode == "") RaiseAssemblerException("Unknown address mode");

            var opcode = opcodes6502.GetOpcode(opName, addrMode);
            memory.WriteMem(PC++, opcode);

            if (line != "")
            {
                var operandBytes = opcodes6502.GetOperandData(value, addrMode, PC);
                foreach (byte b in operandBytes)
                {
                    memory.WriteMem(PC++, b);
                }
            }
        }

        public void AssemblePseudoOp(string line)
        {
            listHandler.AddPsuedoOp(line);


            var matchOp = line.Match(PsuedoOp);
            line = line.RemoveString(matchOp.MatchString).Trim();

            switch (matchOp.MatchString.ToLower())
            {
                case ".byte":
                    AssembleByte(line);
                    break;
                case ".word":
                case ".vctrs":
                    AssembleWord(line);
                    break;
                case ".title":
                    break;
                case ".asect":
                    break;
                case ".include":
                    AssembleInclude(line);
                    break;
                case ".nocross":
                    break;
                case ".defstack":
                    AssembleDefineStack(line);
                    break;
                case ".push":
                    AssemblePush(line);
                    break;
                case ".pop":
                    AssemblePop(line);
                    break;
                case ".radix":
                    AssembleRadix(line);
                    break;
                case ".blkb":
                    AssembleBlkb(line);
                    break;
                case ".ascii":
                    AssembleAscii(line);
                    break;
                case ".endm":           // ENDM also behaves as an ENDR
                    AssembleEndRepeat();
                    break;
                case ".getpointer":
                    AssembleGetPointer(line);
                    break;
                case ".print":
                    break;
                case ".list":
                    break;
                case ".sbttl":
                    break;
                case ".nlist":
                    break;
                case ".page":
                    break;
                case ".enable":
                case ".enabl":
                    break;
                case ".globl":
                case ".globb":
                    break;
                case ".warn":
                    break;
                case ".end":
                    break;
                case ".error":
                    RaiseAssemblerException($"ERROR Pseudoop {line}");
                    break;
                default:
                    RaiseAssemblerException($"Unknown psuedo op {matchOp.MatchString}");
                    break;
            }
        }

        private void AssembleAscii(string line)
        {
            int start = line.IndexOf("/");
            int end = line.IndexOf("/", start + 1);
            for (int i = start + 1; i < end; i++)
            {
                memory.WriteMem(PC++, (byte)line[i]);
            }
        }

        private void AssembleRadix(string line)
        {
            int value;
            if (!int.TryParse(line, out value))
            {
                double? dvalue = EvaluateExpression(line).Value;
                value = (int)dvalue;
            }
            if (value != 2 && value != 10 && value != 16) RaiseAssemblerException($"Invalid radix {value}");
            radix = value;
        }

        private void AssembleBlkb(string line)
        {
            double? value = EvaluateExpression(line);
            PC = PC + (int)value.Value;
        }


        private void AssemblePush(string line)
        {
            var parts = line.Split(',');
            if (!stacks.ContainsKey(parts[0])) RaiseAssemblerException($"Stack {parts[0]} not found");
            var stack = stacks[parts[0]];
            for (int i = 1; i <= parts.GetUpperBound(0); i++)
            {
                double? value = EvaluateExpression(parts[i]);
                stack.Push(value.Value);
            }
        }


        private void AssemblePop(string line)
        {
            var parts = line.Split(',');
            if (!stacks.ContainsKey(parts[0])) RaiseAssemblerException($"Stack {parts[0]} not found");
            var stack = stacks[parts[0]];
            for (int i = 1; i <= parts.GetUpperBound(0); i++)
            {
                var symbol = symbolTable.GetSymbol(parts[i]);
                if (symbol == null)
                {
                    symbol = Symbol.NewVariable(parts[i], 0);
                    symbolTable.AddSymbol(symbol);
                }
                symbol.NumericValue = stack.Pop();
            }
        }

        private void AssembleGetPointer(string line)
        {
            var parts = line.Split(',');
            var stack = stacks[parts[0]];
            var symbol = symbolTable.GetSymbol(parts[1]);
            if (symbol == null)
            {
                symbol = Symbol.NewVariable(parts[1], stack.Pointer);
                symbolTable.AddSymbol(symbol);
            }
            else
            {
                symbol.NumericValue = stack.Pointer;
            }
        }

        private void AssembleDefineStack(string line)
        {
            var parts = line.Split(',');
            double? value = EvaluateExpression(parts[1]);
            var stack = new AssemblerStack(parts[0], (int)value.Value);
            stacks.Add(stack.Name, stack);
        }

        private void AssembleInclude(string line)
        {
            var include = LoadFile(line);
            var segment = new ProgramSegment(include);
            segment.Filename = line;
            segmentStack.Push(programSegment);
            programSegment = segment;

            listHandler.FileSwitch(line);
        }

        private void AssembleWord(string line)
        {
            var values = line.Split(',');
            foreach (string value in values)
            {
                var numValue = EvaluateExpression(value);
                memory.WriteMem(PC++, (byte)((int)numValue & 0xFF));
                memory.WriteMem(PC++, (byte)(((int)numValue.Value & 0xFF00) >> 8));
            }
        }

        private void AssembleByte(string line)
        {
            var values = line.Split(',');
            foreach (string value in values)
            {
                var numValue = EvaluateExpression(value);
                memory.WriteMem(PC++, (byte)numValue);
            }
        }

        public void AssembleGlobalLabel(string label)
        {
            label = label.TrimEnd(':');
            var symbol = Symbol.NewGlobalLabel(label, PC);

            AddSymbol(symbol);
            currentScope = label;
            listHandler.AddLabel(label);
        }


        public void AssembleLocalLabel(string label)
        {
            label = label.TrimEnd(':');
            var symbol = Symbol.NewLocalLabel(label, currentScope, PC);

            AddSymbol(symbol);
        }

        private void AddSymbol(Symbol symbol)
        {

            if (symbolTable.SymbolExists(symbol))
            {
                if (secondPass)
                {
                    var existingSymbol = symbolTable.GetSymbol(symbol.FullyQualifiedName);
                    if (symbol.NumericValue != existingSymbol.NumericValue) RaiseAssemblerException("Symbol value changed between passes");
                }
                else
                {
                    RaiseAssemblerException($"Symbol {symbol.Name} already definied");
                }
            }
            else
            {
                symbolTable.AddSymbol(symbol);
            }
        }

        public void AssembleAssignment(string line)
        {
            line = line.Trim();
            var identifier = line.GetUntil('=').Trim();
            line = line.RemoveString(identifier).Trim();
            line = line.TrimStart('=').Trim();

            var value = EvaluateExpression(line);
            if (identifier == ".")
            {
                PC = (int)value;
            }
            else
            {
                if (symbolTable.GetSymbol(identifier) != null)
                {
                    symbolTable.UpdateSymbolValue(identifier, value);
                }
                else
                {
                    AddSymbol(Symbol.NewVariable(identifier, value));
                }
            }
        }

        public double? EvaluateExpression(string expression, bool nullForUndefined = false)
        {
            double? value = 0;
            double? result = 0;

            string op = "";
            string unary = "";

            if (expression.Trim() == "") return null;

            while (expression != "")
            {
                expression = expression.Trim();

                Match match;

                // Unary negate
                if (expression.StartsWith("-"))
                {
                    unary = "-";
                    expression = expression.TrimStart('-');
                }

                // Hex value
                if ((match = expression.Match(HexValue)).Matched)
                {
                    value = ConvertHex(match.MatchString);
                    expression = expression.RemoveString(match.MatchString);
                }

                // Binary value
                else if ((match = expression.Match(BinaryValue)).Matched)
                {
                    value = ConvertBinary(match.MatchString);
                    expression = expression.RemoveString(match.MatchString);
                }

                // Local Label
                else if ((match = expression.Match(LocalLabel)).Matched)
                {
                    var symbol = symbolTable.GetSymbol(match.MatchString, currentScope);
                    if (symbol == null)
                    {
                        if (secondPass)
                        {
                            RaiseAssemblerException($"Undefined symbol {match.MatchString}");
                        }
                        if (nullForUndefined)
                            return null;
                        else
                            return 0;
                    }
                    value = symbol.NumericValue;
                    expression = expression.RemoveString(match.MatchString);
                }

                // Non types value
                else if ((match = expression.Match(NonTypedNumber)).Matched)
                {
                    value = ConvertNonType(match.MatchString);
                    expression = expression.RemoveString(match.MatchString);
                }

                // Global Label
                else if ((match = expression.Match(GlobalLabel)).Matched)
                {
                    var symbol = symbolTable.GetSymbol(match.MatchString);
                    if (symbol == null)
                    {
                        if (secondPass)
                        {
                            RaiseAssemblerException($"Undefined symbol {match.MatchString}");
                        }
                        if (nullForUndefined)
                            return null;
                        else
                            return 0;
                    }
                    value = symbol.NumericValue;
                    expression = expression.RemoveString(match.MatchString);
                }

                // Sub expression
                else if (expression.StartsWith("<"))
                {
                    int close = FindClosingAngleBracket(expression);
                    var sub = expression.Substring(1, close - 1);
                    value = EvaluateExpression(sub, nullForUndefined);
                    expression = expression.RemoveString("<" + sub + ">");
                }


                expression = expression.Trim();

                if (unary != "")
                {
                    switch (unary)
                    {
                        case "-":
                            int v = (int)value;
                            v = v ^ 0xFFFF;
                            v = v + 1;
                            value = (double?)v;
                            break;
                    }
                    unary = "";
                }

                if (op == "")
                {
                    result = value;
                }
                else
                {
                    result = PerformOp(result, value, op);
                    if (nullForUndefined && !result.HasValue) result = 0;
                }


                if (expression != "")
                {
                    op = expression.Substring(0, 1);
                    expression = expression.RemoveString(op).Trim();
                }

            }
            return result;
        }

        public int FindClosingAngleBracket(string expression)
        {
            int openCount = 0;
            for (int i = 0; i < expression.Length; i++)
            {
                if (expression[i] == '<') openCount++;
                if (expression[i] == '>')
                {
                    openCount--;
                    if (openCount == 0) return i;
                }
            }

            return -1;
        }

        public double? PerformOp(double? operand1, double? operand2, string op)
        {
            if (!operand1.HasValue || !operand2.HasValue) return null;

            switch (op)
            {
                case "+":
                    return operand1.Value + operand2.Value;
                case "-":
                    return operand1.Value - operand2.Value;
                case "*":
                    return operand1.Value * operand2.Value;
                case "/":
                    return operand1.Value / operand2.Value;
                case "&":
                    return (int)operand1.Value & (int)operand2.Value;
                case "!":
                    return (int)operand1.Value | (int)operand2.Value;
                case @"\":
                    return (int)operand1.Value ^ (int)operand2.Value;
                default:
                    RaiseAssemblerException("Unknow operator " + op);
                    break;

            }
            return 0;

        }

        public double ConvertNonType(string num)
        {
            if (num.Contains("."))
            {
                return double.Parse(num);
            }

            switch (radix)
            {
                case 16:
                    return (double)Convert.ToUInt16(num, 16);
                case 10:
                    return double.Parse(num);
                case 2:
                    var i = Convert.ToInt32(num, 2);
                    return (double)i;
            }
            return 0;
        }

        public double ConvertHex(string hex)
        {
            hex = hex.RemoveString("^H");
            return (double)Convert.ToInt32(hex, 16);
        }

        public double ConvertBinary(string bin)
        {
            bin = bin.RemoveString("^B");
            return (double)Convert.ToInt16(bin, 2);
        }

        public void ProcessComment(string line)
        {
            line = line.Trim().TrimStart(';');
            listHandler.AddComment(line);
        }


    }
}
