AT6502 CRF.MAC,CRP.MAC,CLS.MAC -List:CCASTLES.ASM -Output:ccastles.bin -Symbols:ccastles.sym
ROMTOOL "ccastles.bin" "136022-103.1k" -romnum:1 -start:A000 -Length:2000 
ROMTOOL "ccastles.bin" "136022-104.1l" -romnum:2 -start:C000 -Length:2000  
ROMTOOL "ccastles.bin" "136022-105.1n" -romnum:3 -start:E000 -Length:2000  

AT6502 C99.MAC -List:C99.LST -Output:C99.bin
ROMTOOL "C99.bin" "136022-102.1h" -start:A000 -Length:2000  -romnum:4
ROMTOOL "C99.bin" "136022-101.1f" -start:C000 -Length:2000  -romnum:5

