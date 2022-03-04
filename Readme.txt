AT6502
Copyright 2022 Dan Boris (danlb_2000@yahoo.com)

License
--------------------
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

System Requirements
-------------------

   Windows XP or greater 
   Microsoft .NET Framework 4.6.1


Features
--------

AT6502 is a 6502 assembler designed to assemble the Atari arcade game source code. It was initially designed
to re-assemble the source code for Crystal Castles which can be found here: 

https://github.com/historicalsource/crystal-castles

The assembler syntax used in this source has a lot of differences from what is supported in other assemblers,
so I decided the best way to re-assemble the original code was to create an assembler designed for this syntax. 
Currently the assembler can accurately reproduce the ROMS from the base version of the code in that repository 
with minimal changes to the source. The ROMS that are created are a match for ccastles1 in MAME. 

Limitations:
-	The initial goal was to re-assemble the Crystal Castles source so the assembler may not work
with other source code.
-	The pseudo-ops  .title, .asect, .nocross, .list, .sbttl, .nlist, .page, .enable, 
.globl, .warn and .end are accepted by the assembler but do not currently do anything since 
they did not impact the binary out of the assembler. 
-	The original code used a linker to assemble the final binary, but this assembler gets the same 
result by assembling multiple source files at one time. 
-	The assembler currently changes everthing to uppercase before assembling it.

Usage:
AT6502 SourceFile(s) -list:filename -output:filename -symbols:filename -console

SourceFile(s) – Comma delimited list of files to assembler. The files will be assembled as if appended 
one to the next in the order specified.
-list:filename – Filename to output the list file to
-output:filename -Filename to output the binary data to. Currently the assembler outputs a since 64K 
file with the full contents of memory.
-symbols:filename – Filename to output the symbol table to.
-console – If specified the list output will also be sent to the console
