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


using System.Linq;

namespace AT6502
{
    public class CharacterSet
    {
     
        string characters; 
              
        public CharacterSet(params string[] sets) 
        {
            foreach(string set in sets)
            {
                if (set.StartsWith("{"))
                {
                    AddPredefined(set);
                }

                if (set.StartsWith("["))
                {
                    AddSet(set);
                }
            }    
        }

        public bool ContainsCharacter(char c)
        {
            if (characters.Contains(c)) return true;
            return false;
        }

        public void AddSet(string set)
        {
            set = set.TrimStart('[').TrimEnd(']');
            characters += set;
        }

        public void AddPredefined(string setName)
        {
            switch(setName.Substring(1, setName.Length-2).ToLower())
            {
                case "letter":
                    AddSet("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
                    break;
                case "number":
                    AddSet("0123456789");
                    break;
                case "alphanumeric":
                    AddSet("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
                    break;

            }
        }
    }
}
