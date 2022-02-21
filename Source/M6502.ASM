;  filename M6502.MAC

;  this is version 2 of 6502 general purpose macros,
;  		May 26, 1983
;  send mail to [FXL], Franz Lanzinger, for comments
;  contributions, improvements etc.

;  Contributors (alphabetical order):
;	FXL,RIVERA,SHEPPERD

;  Warning:
;	Many of these macros have side effects.  It is
;	recommended that you read and understand how
;	they work before you use them.

;  Conventions:
;	1.  Do not .INCLUDE this file directly, as it
;	    is likely to change frequently, at least in
;	    the next few months.  Instead copy this file
;	    to your directory.  You will be notified of
;	    new versions.
;  	2.  Macros don't call one another, so you can
;	    delete unused macros in your copy of this file
;	3.  If the accumulator gets clobbered by the
;	    macro, the letter A is near the end of the
;	    macro name.
;	4.  Use the abbreviations #,@,(X), i.e.
;		a:  TRAM ARG1(X) @ARG2(Y)     works, but
;		b:  TRAM <X,ARG1> <NY,ARG2>   doesn't.
;	    This is the reason for the expressions like
;	    1+FROM instead of FROM+1.  If you prefer b:,
;	    edit your copy of this file, and change the
;	    1+variables to variable+1.  I like a: because
;	    it is shorter.


;------------------------------------------------------
;
;  TRansfer through A register Immediate
;  from FROM to TO
.MACRO TRAI FROM,TO
~LDA I,FROM
~STA TO
~.ENDM

;  TRansfer through A register Memory
;  from FROM to TO
.MACRO TRAM FROM,TO
~LDA FROM
~STA TO
~.ENDM

;  TRansfer 16 bits through A Immediate from FROM to TO
.MACRO TR16AI FROM,TO
~LDA I,FROM&^H0FF
~STA TO
~LDA I,FROM&^H0FF00/^H100
~STA 1+TO
~.ENDM

;  transfer 24 bits trough A immediate from FROM1
; (high 8 bits) and  FROM2 (low 16 bits) to TO

.MACRO TR24AI FROM1,FROM2,TO
~TRAI FROM1 2+TO
~TR16AI FROM2 TO
~.ENDM

;  TRansfer 16 bits through A, from FROM to TO
.MACRO TR16AM FROM,TO
~LDA FROM
~STA TO
~LDA 1+FROM
~STA 1+TO
~.ENDM

;  transfer 24 bits through A, from FROM to TO
.MACRO TR24AM FROM,TO
~LDA FROM
~STA TO
~LDA 1+FROM
~STA 1+TO
~LDA 2+FROM
~STA 2+TO
~.ENDM

;  ADD to accumulator with clear carry
.MACRO ADD ARG
~CLC
~ADC ARG
~.ENDM

;  SUBtract from accumulator with set carry
.MACRO SUB ARG
~SEC
~SBC ARG
~.ENDM

;  ADD Immediate VAL to memory MEM
.MACRO ADAI VAL MEM
~LDA MEM
~CLC
~ADC I,VAL
~STA MEM
~.ENDM

;  SUBtract Immediate VAL from memory MEM
.MACRO SBAI VAL MEM
~LDA MEM
~SEC
~SBC I,VAL
~STA MEM
~.ENDM

; ADAM
.MACRO ADAM MEM VAL
~LDA MEM
~CLC
~ADC VAL
~STA MEM
~.ENDM

.MACRO SBAM MEM VAL
~LDA MEM
~SEC
~SBC VAL
~STA MEM
~.ENDM

;  INCrement 16-bit value ADDR, ?B is a dummy label
.MACRO INC16 ADDR,?B
~INC ADDR
~BNE B
~INC 1+ADDR
B:
~.ENDM

;  DECrement 16-bit using A, value at location ADDR
.MACRO DEC16A ADDR,?B
~LDA ADDR
~BNE B
~DEC 1+ADDR
B:
~DEC ADDR
~.ENDM

;  DECrement 16-bit, A register is preserved, value at ADDR
.MACRO DEC16 ADDR,?B
~PHA
~LDA ADDR
~BNE B
~DEC 1+ADDR
B:
~DEC ADDR
~PLA
~.ENDM

;  ADd 16-bit positive value POSVAL Immediate
;  to 16-bit ADDR
.MACRO AD16AI ADDR,POSVAL
~LDA ADDR
~CLC
~ADC I,POSVAL&^H0FF
~STA ADDR
~LDA 1+ADDR
~ADC I,POSVAL&^H0FF00/^H100
~STA 1+ADDR
~.ENDM

;  ADd 16-bit OFF through A to ADDR
.MACRO AD16AM ADDR,OFF
~LDA ADDR
~CLC
~ADC OFF
~STA ADDR
~LDA 1+ADDR
~ADC 1+OFF
~STA 1+ADDR
~.ENDM

; ADd to 16-bit ADDR, through A, 8-bit OFF
.MACRO AD16A8 ADDR,OFF
~LDA ADDR
~CLC
~ADC OFF
~STA ADDR
~LDA 1+ADDR
~ADC #0
~STA 1+ADDR
~.ENDM

; SuBtract 16-bit value POSVAL through A immediate
; from ADDR
.MACRO SB16AI ADDR,POSVAL
~LDA ADDR
~SEC
~SBC I,POSVAL&^H0FF
~STA ADDR
~LDA 1+ADDR
~SBC I,POSVAL&^H0FF00/^H100
~STA 1+ADDR
~.ENDM

; SuBtract 16-bit OFF through A from ADDR
.MACRO SB16AM ADDR,OFF
~LDA ADDR
~SEC
~SBC OFF
~STA ADDR
~LDA 1+ADDR
~SBC 1+OFF
~STA 1+ADDR
~.ENDM

; NEGate Accumulator
.MACRO NEGA
~EOR #0FF
~CLC
~ADC #1
~.ENDM

; NEGate Byte in memory
.MACRO NEGB ARG
~PHA
~LDA #0
~SEC
~SBC ARG
~STA ARG
~PLA
~.ENDM

;NEGate 16 bit value in memory
.MACRO NEG16 ARG
~PHA
~LDA #0
~SEC
~SBC ARG
~STA ARG
~LDA #0
~SBC 1+ARG
~STA 1+ARG
~PLA
~.ENDM

;  divide by 2
.MACRO DIV2
~IFMI
~SEC
~ROR
~ELSE
~LSR
~ENDIF
~.ENDM

;  ph and pl (X) and (Y)
.MACRO PHXYA
~TXA
~PHA
~TYA
~PHA
~.ENDM
.MACRO PLXYA
~PLA
~TAY
~PLA
~TAX
~.ENDM

;  jump indirect,  A has starting address of jump table, Y has index
.MACRO JMPIN A
~LDA Y,A+1
~PHA
~LDA Y,A
~PHA
~RTS
~.ENDM

