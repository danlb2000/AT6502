using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        static StringTerminal IfTerminal = new StringTerminal(".IF ");
        static StringTerminal IIfTerminal = new StringTerminal(".IIF");
        static StringTerminal EndcTerminal = new StringTerminal(".ENDC");
        static StringTerminal IffTerminal = new StringTerminal(".IFF");
        static StringTerminal IftTerminal = new StringTerminal(".IFT");
        static StringTerminal IftfTerminal = new StringTerminal(".IFTF");
        static StringTerminal MacroTerminal = new StringTerminal(".MACRO");
        static StringTerminal EndMacroTerminal = new StringTerminal(".ENDM");

        int radix = 10;
        int pc = 0;

        public int PC
        {
            get { return pc; }
            set
            {
                pc = value;
                symbolTable.UpdateSymbolValue(".", pc);
                System.Diagnostics.Debug.WriteLine("PC: " + pc.ToString());
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
            var sr = new StreamReader(filename);
            while (true)
            {
                var line = sr.ReadLine();
                if (line == null) break;
                if (line.Length > 0 && line[0] == 0) continue;
                lines.Add(line);
            }

            return lines;
        }

        public void StartAssemble(string fileName)
        {
            var program = LoadFile(fileName);
            StartAssemble(program, fileName);
        }

        public void StartAssemble(List<string> program, string fileName = "")
        {
            programSegment = new ProgramSegment(program);
            programSegment.Filename = fileName;
            Assemble();
        }

        public string NextLine()
        {
            string line = "";

            while (true)
            {
                line = programSegment.NextLine();
                if (line == null)
                {
                    if (segmentStack.Count == 0) return null;
                    programSegment = segmentStack.Pop();
                    continue;
                }

                if (line.Trim().Match(EndrTerminal).Matched)
                {
                    AssembleEndRepeat();
                    continue;
                }

                if (repeatZero) continue;

                if (line.Trim().Match(ReptTerminal).Matched)
                {
                    AssembleRepeat(line);
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

                if (line.Trim().ToUpper().Match(IfTerminal).Matched)
                {
                    PreprocessIf(line);
                    continue;
                }

                if (line.Trim().ToUpper().Match(EndcTerminal).Matched)
                {
                    PreprocessEndc(line);
                    continue;
                }

                if (line.Trim().Match(IftTerminal).Matched ||
                    line.Trim().Match(IffTerminal).Matched ||
                    line.Trim().Match(IftfTerminal).Matched)
                {
                    ProcessSubCondition(line);
                    continue;
                }

                if (line.Trim().Match(IIfTerminal).Matched)
                {
                    var result = PreProcessIif(line);
                    if (result != "")
                        line = result;
                    else
                        line = "";

                }

                if (conditionStack.Count == 0 || conditionStack.Peek().EvalState)
                {
                    break;
                }
            }

            return line;
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
            for(int i=0; i<tokens.Count; i++) 
            {
                var token = tokens[i];
                if (token == placeholder)
                {
                    result.Append(replaceWith);
                    if (i < tokens.Count - 1 && tokens[i + 1] == "'") i++;
                }
                else
                {
                    result.Append(token);
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

        public void AssembleMacro(string line)
        {
            line = line.Trim();
            var match = line.Match(MacroName);
            if (!match.Matched) throw new Exception("Sytnax error parsing macro definiton");

            var name = match.MatchString;
            line = line.RemoveString(name).Trim();

            string[] parameters = new string[0];

            if (line != "") { parameters = line.Split(','); }

            if (!macros.ContainsKey(name))
            {
                throw new AssembleException($"Unknow macro {name}", 1005, programSegment.FileLineNumber, programSegment.Filename);
            }
            var macro = macros[name];
            var expandedLines = new List<string>();
            foreach (string macroLine in macro.Lines)
            {
                string expandedLine = macroLine;

                expandedLine = "";
                var tokens = Tokenize(macroLine);
                foreach (string s in tokens)
                {
                    string s2 = s;
                    for (int index = 0; index < macro.Parameters.Count; index++)
                    {
                        if (s == macro.Parameters[index])
                        {
                            if (index <= parameters.GetUpperBound(0))
                            {
                                s2 = parameters[index];                               
                            }
                            else
                            {
                                s2 = "";
                            }
                        }
                    }
                    expandedLine += s2;
                }

                expandedLines.Add(expandedLine);
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

            var repeat = new Repeat(programSegment.CurrentLine, placeholder, replaceList);
            repeatStack.Push(repeat);
        }

        public void AssembleRepeat(string line)
        {
            line = line.Trim().RemoveString(".REPT").Trim();

            double? count = EvaluateExpression(line);
            int start = programSegment.CurrentLine;
            if (count == 0) repeatZero = true;

            var repeat = new Repeat(start, count);
            repeatStack.Push(repeat);
        }

        public void AssembleEndRepeat()
        {
            var repeat = repeatStack.Peek();

            if (repeat.Type == Repeat.RepeatType.count)
            {
                repeat.Count--;

                if (repeat.Count <= 0)
                {
                    repeatStack.Pop();
                    repeatZero = false;
                }
                else
                {
                    programSegment.CurrentLine = repeat.StartLine;
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
                    programSegment.CurrentLine = repeat.StartLine;
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
                var parameters = line.Split(',');
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
            if (conditionStack.Peek().IsSubCondition) conditionStack.Pop();

            var condition = new ConditionState();
            condition.Line = line;
            condition.LineNumber = programSegment.CurrentLine - 1;
            condition.IsSubCondition = true;

            var previousState = conditionStack.Peek().EvalState;
            conditionStack.Push(condition);

            line = line.Trim();
            condition.EvalState = false;
            if ((line == ".IFT" || line == ".IFTF") && previousState == true) condition.EvalState = true;
            if ((line == ".IFF" || line == ".IFTF") && previousState == false) condition.EvalState = true;
        }

        public void PreprocessEndc(string line)
        {
            while (conditionStack.Peek().IsSubCondition)
            {
                conditionStack.Pop();
            }
            conditionStack.Pop();
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
            condition.LineNumber = programSegment.CurrentLine - 1;
            condition.EvalState = EvaluateCondition(parts[0], parts[1]);
            conditionStack.Push(condition);
        }

        private bool EvaluateCondition(string condition, string expression)
        {
            double? value;

            value = EvaluateExpression(expression);

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
                    throw new AssembleException("Unknown comparison type " + condition, 1000, programSegment.FileLineNumber, programSegment.Filename);
            }
            return false;
        }

        private void Assemble()
        {
            int lastPC = 0;
            listHandler.EnableList = false;

            for (int pass = 0; pass < 2; pass++)
            {
                programSegment.Reset();
                currentScope = "";
                PC = 0;
                radix = 10;
                stacks = new Dictionary<string, AssemblerStack>();
                repeatZero = false;

                if (pass == 1)
                {
                    secondPass = true;
                    listHandler.EnableList = true;
                    System.Diagnostics.Debug.WriteLine("PASS 2");
                }

                while (true)
                {
                    lastPC = PC;
                    var line = AssembleLine();
                    if (line == null) break;

                    var bytes = memory.PopBytesWritten();
                    listHandler.AddLine(programSegment.FileLineNumber, lastPC, bytes, line);
                }
            }
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
                line = line.Substring(0, commentPosition - 1).Trim();
            }

            // Handle line label
            if ((match = line.Match(GlobalLabel, LabelDeliminter)).Matched)
            {
                AssembleGlobalLabel(match.MatchString);
                line = line.RemoveString(match.MatchString).Trim();
                line = line.TrimStart(':');  
                if (line == "") return lineStart;
            }

            if ((match = line.Match(LocalLabel, LabelDeliminter)).Matched)
            {
                AssembleLocalLabel(line);
                line = line.RemoveString(match.MatchString).Trim();
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
            var addrMode = opcodes6502.DetermineAddressingMode(opName, line);
            line = opcodes6502.RemoveAddressModeIdentifier(line).Trim();

            double? value = 0;
            if (line != "")
            {
                value = EvaluateExpression(line);
                addrMode = opcodes6502.CorrectAddressingMode(addrMode, value, opName);
            };

            var opcode = opcodes6502.GetOpcode(opName, addrMode);
            memory.WriteMem(PC++, opcode);

            if (line != "")
            {
                var operand = opcodes6502.GetOperand(value, addrMode, PC);
                foreach (byte b in operand)
                {
                    memory.WriteMem(PC++, b);
                }
            }
        }

        public void AssemblePseudoOp(string line)
        {
            var matchOp = line.Match(PsuedoOp);
            line = line.RemoveString(matchOp.MatchString).Trim();

            switch (matchOp.MatchString.ToLower())
            {
                case ".byte":
                    AssembleByte(line);
                    break;
                case ".word":
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
                case ".print":
                    break;
                case ".list":
                    break;
                case ".nlist":
                    break;
                case ".page":
                    break;
                case ".enable":
                    break;
                case ".globl":
                    break;
                case ".error":
                    throw new AssembleException($"ERROR Pseudoop {line}", 1003, programSegment.FileLineNumber, programSegment.Filename);
                default:
                    throw new AssembleException($"Unknown psuedo op {matchOp.MatchString}", 1003, programSegment.FileLineNumber, programSegment.Filename);
            }
        }

        private void AssembleAscii(string line)
        {
            int start = line.IndexOf("/");
            int end = line.IndexOf("/", start + 1);
            for(int i = start+1; i<end; i++)
            {
                memory.WriteMem(PC++, (byte)line[i]);
            }
        }

        private void AssembleRadix(string line)
        {
            double? value = EvaluateExpression(line);
            if (value != 2 && value != 10 && value != 16) throw new AssembleException($"Invalid radix {value}", 1010, programSegment.FileLineNumber, programSegment.Filename);
            radix = (int)value.Value;
        }

        private void AssembleBlkb(string line)
        {
            double? value = EvaluateExpression(line);
            PC = PC + (int)value.Value;
        }


        private void AssemblePush(string line)
        {
            var parts = line.Split(',');
            if (!stacks.ContainsKey(parts[0])) throw new AssembleException($"Stack {parts[0]} not found", 1006, programSegment.FileLineNumber, programSegment.Filename);
            var stack = stacks[parts[0]];
            for (int i = 1; i <= parts.GetUpperBound(0); i++)
            {
                double? value = EvaluateExpression(parts[i]);
                if (!value.HasValue) value = 0;
                stack.Push(value.Value);
            }
        }


        private void AssemblePop(string line)
        {
            var parts = line.Split(',');
            if (!stacks.ContainsKey(parts[0])) throw new AssembleException($"Stack {parts[0]} not found", 1006, programSegment.FileLineNumber, programSegment.Filename);
            var stack = stacks[parts[0]];
            for (int i = 1; i <= parts.GetUpperBound(0); i++)
            {
                var symbol = symbolTable.GetSymbol(parts[i]);
                if (symbol == null)
                {
                    symbol = Symbol.NewConstant(parts[i], 0);
                    symbolTable.AddSymbol(symbol);
                }
                symbol.NumericValue = stack.Pop();
            }
        }

        private void AssembleDefineStack(string line)
        {
            var parts = line.Split(',');
            double? value = EvaluateExpression(parts[1]);
            var stack = new AssemblerStack();
            stack.Name = parts[0];
            stack.Size = value.Value;
            stacks.Add(stack.Name, stack);
        }

        private void AssembleInclude(string line)
        {
            var include = LoadFile(line);
            var segment = new ProgramSegment(include);
            segment.Filename = line;
            segmentStack.Push(programSegment);
            programSegment = segment;
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
                if (!secondPass)
                {
                    throw new AssembleException($"Symbol {symbol.Name} already definied", 1001, programSegment.FileLineNumber, programSegment.Filename);
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
                    AddSymbol(Symbol.NewConstant(identifier, value));
                }
            }
        }

        public double? EvaluateExpression(string expression)
        {
            double? value = 0;
            double? result = 0;

            string op = "";

            if (expression.Trim() == "") return null;

            while (expression != "")
            {
                expression = expression.Trim();

                Match match;

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
                    var symbol = symbolTable.GetSymbol(match.MatchString,currentScope);
                    if (symbol == null)
                    {
                        if (secondPass)
                        {
                            throw new AssembleException($"Undefined symbol {match.MatchString}", 1000, programSegment.FileLineNumber, programSegment.Filename);
                        }
                        return null;
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
                            throw new AssembleException($"Undefined symbol {match.MatchString}", 1000, programSegment.FileLineNumber, programSegment.Filename);
                        }
                        return null;
                    }
                    value = symbol.NumericValue;
                    expression = expression.RemoveString(match.MatchString);
                }

                // Sub expression
                else if (expression.StartsWith("<"))
                {
                    var sub = expression.GetUntilLast('>');
                    value = EvaluateExpression(sub.Substring(1));
                    expression = expression.RemoveString(sub);
                }


                expression = expression.Trim();

                if (op == "")
                {
                    result = value;
                }
                else
                {
                    result = PerformOp(result, value, op);
                }


                if (expression != "")
                {
                    op = expression.Substring(0, 1);
                    expression = expression.RemoveString(op).Trim();
                }

            }
            return result;
        }

        public double PerformOp(double? operand1, double? operand2, string op)
        {
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
            }
            return 0;
        }

        public double ConvertHex(string hex)
        {
            hex = hex.RemoveString("^H");
            return (double)Convert.ToInt16(hex, 16);
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
