;  filename CMR.MAC
;  message routines
;  must be self-contained,  used by self-test
;--------------------------------------------

;  draw message (A) cointains message number
MS.DRW:
~.IF EQ,CG.ST
~.IF EQ,CG.ROM
~RTS
~.ENDC
~.ENDC

~ASL
~TAX

~.IF NE,CG.ST
~TR16AM MS.TAB(X) MS.PTR
~INXS 2
~TR16AM MS.TAB(X) MS.LEN
~.ENDC

~SB16AM MS.LEN MS.PTR

~TRAI 07F AL.COL
~LDY #1
~TRAM @MS.PTR(Y) AL.Y
~DEY
~TRAM @MS.PTR(Y) AL.X
~STA AL.LMG		;  left margin

~SBAI 2 MS.LEN

~BEGIN			;  loop through words
~LDY #2
~LDA @MS.PTR(Y)		;  word number
~CMP #0A			;  if not CR-directive
~IFCS
~JSR WR.DRW
~ELSE
~CMP #6
~IFPL		;  6,7,8 map to 8,12.,16.
~ASLS 2
~SUB #10
~ENDIF
~ASL
~STA TEMP1
~ASL
~ADD TEMP1
~ADD AL.LMG	; 0B1+6*CRdirective
~STA AL.X	; hor position
~ADAI 08 AL.Y  ; vert position
~ENDIF
~INC16 MS.PTR
~DEC MS.LEN
~EQEND

~RTS

;-------------------------------------
;  draw word, (A) contains word number
WR.DRW:
~.IF EQ,CG.ST
~.IF EQ,CG.ROM
~RTS
~.ENDC
~.ENDC

~SUB #0A
~ASL				;  up to 246 words
~STA TEMP1
~IFCC
~TRAI 0 1+TEMP1
~ELSE
~TRAI 1 1+TEMP1
~ENDIF			;  TEMP1,1+TEMP1  cointain table offset
~TR16AI WR.TAB WR.TPT
~AD16AM WR.TPT TEMP1	; points to table entry of word
~LDY #0
~TRAM @WR.TPT(Y) WR.PTR
~INY
~TRAM @WR.TPT(Y) WR.PTR+1
~INY
~TRAM @WR.TPT(Y) WR.LEN	;  next word
~INY
~TRAM @WR.TPT(Y) WR.LEN+1
~SB16AM WR.LEN WR.PTR
~BEGIN
~LDY #0
~TRAM @WR.PTR(Y) AL.DIG
~JSR AL.DRW

~LDA AL.X
~ADD #6		;  works only for 5x5
~STA AL.X

~INC16 WR.PTR
~DEC WR.LEN
~EQEND
~ADAI 6 AL.X
~RTS

;-----------------------------
;  draw symbol
AL.DRW:
~TRAI 000 HW.AY	; y auto dec
~TRAI 0FF HW.YIN

~TRAM AL.X XB
~LDY AL.DIG
;~.REPT 0		;  no room for 8x8 font
;~CPY #0A
;~IFMI
;~TR16AI AL.88D AL.PTR	;  digit
;~TYA
;~JMP 5$
;~ENDIF
;~CPY #40
;~IFMI
;~TR16AI AL.88L AL.PTR	;  other symbol (up to 32)
;~TYA
;~SUB #0A
;~JMP 5$
;~ENDIF
;~.ENDM
;~CPY #4A
;~IFMI
;~TR16AI AL.55D AL.PTR
;~TYA
;~SUB #40
;~ELSE
;~TR16AI AL.55L AL.PTR
;~TYA
;~SUB #4A
;~ENDIF
;~JSR AL.5OT
;~.REPT 0
;~JMP 8$
;5$:
;~JSR AL.8OT
;~.ENDM
;8$:
;~TRAI 0FF HW.AY	; auto dec off
;~RTS
;
;;----------------------------
;;  output 5x5 symbol
;AL.5OT:
;~STA TEMP1	;  mult by 5 so (A) points to sym
;~ASLS 2
;~ADD TEMP1
;~STA TEMP1
;
;~TRAI 5 AL.TMP
;~LDX AL.COL
;10$:
;~LDY TEMP1
;~TRAM AL.Y YB
;~LDA @AL.PTR(Y)
;~LDY #0F
;
;~.REPT 5
;~ASL
;~IFCS
;~STX VB
;~ENDIF
;~IFCC
;~LDY VB
;~ENDIF
;;	DEC YB
;~.ENDM
;
;~INC XB
;~INC TEMP1
;~DEC AL.TMP
;~BNE 10$
;
;~RTS
;
;.REPT 0
;;----------------------
;;  output 8x8 symbol
;AL.8OT:
;~ASLS 3		;  (A) points to symbol
;~STA TEMP1
;
;~TRAI 8 AL.TMP
;~LDX AL.COL
;10$:
;~LDY TEMP1
;~TRAM AL.Y YB
;~LDA @AL.PTR(Y)
;~LDY #0F
;
;~.REPT 8
;~ASL
;~IFCS
;~STX VB
;~ENDIF
;~IFCC
;~LDY VB
;~ENDIF
;;	DEC YB
;~.ENDM
;
;~INC XB
;~INC TEMP1
;~DEC AL.TMP
;~BNE 10$
;
;~RTS
;
;~.ENDM
;
;
;
;;------------------------------------------
;;  output 2-digit number with zero suppress
;;  inputs:  (A)  2-digit hex number
;;  	    AL.X,AL.Y   x,y positions
;DG.2OT:
;~JSR AL.CNV
;~LDX #0
;~STX SC.FNZ	; 1= found first non-zero digit
;
;DG.2HT:
;~PHA
;~LSRS 4
;~STA SC.DIG
;
;~TRAI 2*6 SC.LEF	; pixels left to erase
;
;;  first digit
;~JSR AL.DGO
;
;;  second digit
;~INC SC.FNZ	;  output digit even if score=0
;~PLA
;~AND #0F
;~STA SC.DIG
;~JSR AL.DGO
;
;~LDA SC.LEF
;~JSR SC.ERA
;~RTS
;
;
;
;;-------------------------
;;  convert (A) to decimal
;;  output  (X) contains hundreds digit
;AL.CNV:
;~LDX #0
;10$:	CMP #0A
;~IFCS
;~SUB #0A
;~INX		;  counts 10's
;~JMP 10$
;~ENDIF
;~STA TEMP1
;
;~TXA		;  calculate hundreds digit
;~LDX #0
;20$:	CMP #0A
;~IFCS
;~SUB #0A
;~INX
;~JMP 20$
;~ENDIF
;
;~ASLS 4
;~ORA TEMP1
;
;~RTS
;
;
;;--------------------------------
;;  output digit as part of score
;AL.DGO:
;~LDA SC.FNZ	;  zero suppress
;~IFEQ
;~LDA SC.DIG
;~IFEQ
;~JMP 50$
;~ELSE
;~INC SC.FNZ
;~ENDIF
;~ENDIF
;
;~LDA #6
;~JSR SC.ERA
;
;~LDA SC.DIG
;~ADD #40
;~STA AL.DIG
;
;~CL.ALP=07F
;~TRAI CL.ALP AL.COL
;~JSR AL.DRW
;
;~ADAI 6 AL.X
;~SBAI 6 SC.LEF
;50$:
;~RTS
;
;
;;--------------------------------------------------
;;  erase part of screen, so you can see the message
;;  (A)  number of pixel columns to erase
;SC.ERA:
;~STA TEMP1
;~LDA AL.X
;~STA XB
;~LDA AL.Y
;~STA YB
;~INC YB
;
;~TRAI 000 HW.AY	; auto dec y
;~TRAI 0FF HW.YIN
;
;~LDA #00F
;~INC TEMP1
;10$:
;~DEC TEMP1
;~BEQ 20$
;~STA VB
;~.REPT 7
;;	DEC YB
;~STA VB
;~.ENDM
;~INC XB
;~LDY AL.Y
;~STY YB
;~INC YB
;~JMP 10$
;20$:
;~TRAI 0FF HW.AY
;~RTS
;
;
;
