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

namespace AT6502
{
    public class Symbol
    {
        public enum SymbolType
        {
            Variable,
            GlobalLabel,
            LocalLabel,
            PC
        }

        public string Name { get; set; }
        public string Scope { get; set; }
        public double? NumericValue { get; set; }
        public SymbolType Type { get; set; }

        public override string ToString()
        {
            var name = "";
            if (!string.IsNullOrEmpty(Scope))
            {
               name = Scope + ":" + Name;
            }
            else
            {
                 name= Name;
            }

            string type = "(" + Type.ToString() + ")";
            string value = string.Format("{0:X4}", (int)NumericValue.Value);

            return name.PadRight(20) + type.PadRight(15) + value;
        }

        public string Export()
        {
            var name = "";
            if (!string.IsNullOrEmpty(Scope))
            {
                name = Scope + ":" + Name;
            }
            else
            {
                name = Name;
            }

            return name + "," + string.Format("{0:X4}", (int)NumericValue.Value);

        }

        public string FullyQualifiedName
        {
            get
            {
                if (Type == Symbol.SymbolType.LocalLabel)
                {
                    return Scope + ":" + Name;
                }
                else
                {
                    return Name;
                }
            }
        }


        public static Symbol NewVariable(string name, double? value)
        {
            var sym = new Symbol();
            sym.Name = name;
            sym.Scope = "";
            sym.NumericValue = value;
            sym.Type = SymbolType.Variable;
            return sym;
        }

        public static Symbol NewGlobalLabel(string name, int address)
        {
            var sym = new Symbol();
            sym.Name = name;
            sym.Scope = "";
            sym.NumericValue = (double?)address;
            sym.Type = SymbolType.GlobalLabel;
            return sym;
        }

        public static Symbol NewLocalLabel(string name, string scope, int address)
        {
            var sym = new Symbol();
            sym.Name = name;
            sym.Scope = scope;
            sym.NumericValue = (double)address;
            sym.Type = SymbolType.LocalLabel;
            return sym;
        }


        public static Symbol NewPcSymbol()
        {
            var sym = new Symbol();
            sym.Name = ".";
            sym.Scope = "";
            sym.NumericValue = 0;
            sym.Type = SymbolType.PC;
            return sym;
        }
    }
}
