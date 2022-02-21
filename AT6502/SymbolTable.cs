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
using System.Text;

namespace AT6502
{
    public class SymbolTable
    {
        private Dictionary<string, Symbol> symbols { get; set; }
        
        public SymbolTable()
        {
            symbols = new Dictionary<string, Symbol>();
        }

        public void AddSymbol(Symbol symbol)
        {         
            symbols.Add(symbol.FullyQualifiedName, symbol);          
        }

        public void UpdateSymbolValue(string name, double? value)
        {
            symbols[name].NumericValue = value;
        }

        public bool SymbolExists(Symbol symbol)
        {
            return symbols.ContainsKey(symbol.FullyQualifiedName);
        }

        public Symbol GetSymbol(string name)
        {
            if (symbols.ContainsKey(name)) return symbols[name];
            return null;
        }

        public Symbol GetSymbol(string name, string scope)
        {
            string fqname = scope + ":" + name;
            if (symbols.ContainsKey(fqname)) return symbols[fqname];
            return null;
        }

        public string DumpTable()
        {
            var sb = new StringBuilder();
            foreach(KeyValuePair<string,Symbol> symbol in symbols )
            {
                sb.Append(symbol.Value.ToString());
                sb.Append(Environment.NewLine);                
            }

            return sb.ToString();
        }

        public string ExportTable()
        {
            var sb = new StringBuilder();
            foreach (KeyValuePair<string, Symbol> symbol in symbols)
            {
                sb.Append(symbol.Value.Export());
                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }
    }
}
