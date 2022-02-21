;  filename CMAC.MAC
.INCLUDE M6502.MAC
;------------------------------------------
;  macro to compare indirect
.MACRO CMPIN A,B
~LDY #0
~LDA NY,A
~CMP NY,B
.ENDM
;  macro to subtract indirect
.MACRO SUBIN A,B
~LDA A
~LDY #0
~SEC
~SBC NY,B
.ENDM
;  macro to branch to B when A inside interval around 0
.MACRO BINT INTVAL,?B,?C
~CMP I,INTVAL+1
~BPL C
~CMP I,0-INTVAL
~BMI C
~JMP B
C:
.ENDM
;-----------------------------
.MACRO IFEQA A,B
~LDA A
~.IF NE,B
~CMP I,B
~.ENDC
~IFEQ
.ENDM
