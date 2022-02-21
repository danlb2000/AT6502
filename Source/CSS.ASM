;  filename CSS.MAC

;  ROMs are good
MOREST:
~JSR RS.INI
~JSR MN.SNI
~JSR WV.INI

~JSR EEINIT
~TRAI 0FF EE.SIR		;  some time has passed
~;  since power up so
~;  OK to store

~CLI			;  interrupts OK


~LDA #0FF	;  one before init arrow state
~STA AR.STA

~JSR ST.EXI	;  goto game if Selftest switch off
~JMP ST.MEN	;  goto selftest menu

;-------------------------
;  wait until button is pressed, then go back to main menu
ST.WAI:
10$:
~LSR SYNC
~BCC 10$
~JSR ST.EXI

~JSR EN.BRD
~BEQ 10$

;  main menu
ST.MEN:
~TRAI 7 AR.MAX
~JSR AR.INC		;  increment arrow state
~JSR GR.SCL		;  clear sceen
~LDA #26
~JSR MS.DRW


;  main menu loop
10$:
~LSR SYNC
~BCC 10$
~JSR ST.EXI

~JSR AR.UPD	;  update arrow

~LDA EN.JBP
~CMP #MA.BU2
~IFEQ
~LDA AR.STA
~ASL
~TAY
~JMPIN AR.JTB
~ENDIF
~JMP 10$

AR.JTB:	.WORD ST.GRD-1,ST.ACC-1,ST.CAC-1
~.WORD ST.OPT-1,ST.EET-1,ST.SWT-1
~.WORD ST.REH-1

ST.EXI:
~LDA HW.STS
~AND #MA.STS
~IFNE
~JMP MN.HIC	;  jump to game start
~ENDIF
~RTS

ST.SWX:	.BYTE 0A0,80,0C0,080,0A0
ST.SWY: .BYTE 68,68,70,70,70
ST.SWM: .BYTE 73,72,71,72,73
;-------------------------------
;
ST.SWT:
;  switch test
;  motion objects,  switch inputs,  trackballs
;  etc.
~JSR GR.SCL
~LDA #27
~JSR MS.DRW

;  draw red labels
~TRAI 48 AL.X
~TRAI 0A8 AL.Y
~TRAI 07 TEMP2
~BEGIN
~LDA TEMP2
~JSR DG.2OT
~ADAI 0A AL.X
~DEC TEMP2
~MIEND

;  draw color bars
~LDX #0C0
~BEGIN
~TXA
~AND #0F0
~ADD #040

~STX XB
~LDY #0E0
~BEGIN
~STY YB
~STA VB
~DEY
~CPY #0B0
~EQEND
~DEX
~CPX #40
~EQEND

~JSR MT.INI
~JSR EN.INI

10$:
~LSR SYNC
~BCC 10$
~JSR ST.EXI

~TRAI 0FF TEMP2	; index
~TRAI 0C7 TEMP3	; select mask for which switch to display
~TRAI 0   TEMP4	; initial mask

~SEC
~BEGIN
20$:
~ROR TEMP4
~ASL TEMP3
~BCC 20$

~INC TEMP2
~LDY TEMP2
~LDA ST.SWX(Y)
~STA AL.X
~LDA ST.SWY(Y)
~STA AL.Y
~LDA HW.VBL
~AND TEMP4
~IFEQ
~LDA ST.SWM(Y)
~JSR WR.DRW
~ELSE
~LDA #5*6
~JSR SC.ERA
~ENDIF

~CLC
~LDA TEMP3
~EQEND

;  trackball and motion object test
~JSR EN.BRD
~IFNE
~INC TEMP5+1
~CMP #MA.BU2
~IFEQ
~LDA TEMP5+1
~ADD #1F
~STA TEMP5+1
~ENDIF
~ENDIF

~LDX #0
~BEGIN
~TRAI 1 EN.MX(X)
~STA    EN.MY(X)

~TXA
~ASL
~ADD TEMP5+1
~JSR EN.PCF

~TXA
~SUB #0A
~IFMI
~TXA
~ENDIF
~ASLS 3
~ADD HW.TBH
~STA EN.X(X)

~LDA #0
~CPX #0A
~IFCS
~LDA #20
~ENDIF
~ADD HW.TBV
~STA EN.Y(X)

~TXA
~AND #0F0
~IFNE
~LDA #0FF
~ENDIF
~STA EN.PR1(X)

~INXS 2
~CPX #2*EN.MAX
~PLEND

~JSR MT.UPD	;  update motion object info


;  color test
~LDA HW.TBH
~LSRS 5
~STA TEMP1
~LDA HW.TBV
~LSRS 2
~AND #38
~ORA TEMP1
~STA TEMP1

~LDX #07
~BEGIN
~TXA
~ASLS 6
~ORA TEMP1
;	STA CL.SHR+10(X)
~STA CL.SHR+18(X)
~TXA
~AND #04
~ASLS 3
;	STA CL.SHR+30(X)
~STA CL.SHR+38(X)
~DEX
~MIEND

; output trackball values, and colors

~TRAI 088 AL.Y
~TRAI 048 AL.X
~STA SC.FNZ
~LDA HW.TBH
~CMP CL.OLH
~IFNE
~STA CL.OLH
~JSR DG.2HT

~TRAI 098 AL.Y
~TRAI 048 AL.X
~LDA HW.TBH
~JSR CL.CNV
~JSR DG.2OT
~ENDIF

~TRAI 090 AL.Y
~TRAI 048 AL.X
~STA SC.FNZ
~LDA HW.TBV
~CMP CL.OLV
~IFNE
~STA CL.OLV
~JSR DG.2HT

~TRAI 0A0 AL.Y
~TRAI 048 AL.X
~LDA HW.TBV
~JSR CL.CNV
~JSR DG.2OT
~ENDIF

~JMP 10$

CL.CNV:
~LSRS 5
~STA TEMP1
~LDA #07
~SUB TEMP1
~RTS

;------------------------------
;  routine to draw arrow
AR.DRW:
~LDA #ARROW
~STA AL.DIG
~LDA AR.STA
~ASLS 4
~ADD #50
~STA AL.Y
~TRAI 20 AL.X
~JSR AL.DRW
AR.RTS:	RTS

AR.UPD:
~JSR AR.DRW

~JSR EN.BRD
~CMP #MA.BUT	;  left button
~BNE AR.RTS	;  exit if not pressed

~JSR AR.DRW
~LDA #8
~JSR SC.ERA	;  erasing necessary

;  increment arrow
AR.INC:
~LDA AR.STA
~ADD #1
~CMP AR.MAX
~IFEQ
~LDA #0
~ENDIF		; increment arrow state
~STA AR.STA
~RTS

;------------------------------------
;  display and change option switches
ST.OPT:

~LDA AR.STA
~PHA		;  remember old arrow state

ST.OP2:			;  entry for after restoring factory options
;  display option switch values

~JSR GR.SCL
~LDA #12
~JSR MS.DRW

~TRAI EEOPTS TEMP2
~BEGIN
~JSR ST.OPD
~DEC TEMP2
~MIEND

;  get ready

~TRAI 0 AR.STA
~STA ST.COP
~TRAI NEEOPT+2 AR.MAX	;  options+restore+exit

10$:	LSR SYNC
~BCC 10$
~JSR ST.EXI

~JSR AR.UPD

~LDA EN.JBP
~CMP #MA.BU2
~IFEQ

~LDA AR.STA
~CMP #NEEOPT
~IFCC
~LDX #0
~STX EE.SIR	;  no storing while data inconsistent
~TAX
~LDA EEOPT(X)
~ADD #1
~CMP ST.OMX(X)
~IFCS
~LDA #0
~ENDIF
~STA EEOPT(X)
~STX TEMP2
~JSR ST.OPD
~LDA TEMP2
~ADD #EEOPT-EEROM
~TAY
~LDX #0
~JSR EEUPD
~JSR EE.CMD
~ELSE
~CMP #NEEOPT
~IFEQ			;  restore to factory options
~TRAI 0 EE.SIR	;  dont store
~LDY #EEOPT-EEROM+EEOPTS
~BEGIN
~JSR EERES
~DEY
~CPY #EEOPT-EEROM
~CCEND
~TRAI 0FF EE.SIR	;  ok to store now
~JMP ST.OP2
~ELSE
~PLA
~STA AR.STA
~JMP ST.MEN
~ENDIF
~ENDIF
~ENDIF

~JMP 10$

;-----------------------------
;  display option number (TEMP2)
ST.OPD:
~LDA TEMP2
~ASLS 4
~ADD #50
~STA AL.Y
~TRAI 0A0 AL.X
~LDA #50
~JSR SC.ERA

~LDX TEMP2
~LDA ST.WRI(X)
~ADD EEOPT(X)
~TAX
~LDA ST.WRD(X)
~JSR WR.DRW
~RTS

ST.WRI:	.BYTE 0,4,6,10.,14.,17.
ST.WRD:	.BYTE 8F,90,59,8E,15,16,15,26,27,28,9A,9B,9C,9D
~.BYTE 25,26,27,0AF,56
ST.OMX:	.BYTE 4,2,4,4,3,2

;--------------------------------
;  reset high scores
ST.REH:
~TRAI 0 EE.SIR	;  dont store
~LDY #17.
~BEGIN
~JSR EERES
~DEY
~MIEND
~STY EE.SIR	;  0FF, ok to store now
~JMP ST.MEN

;---------------------------------
;  clear accounting, and display it
ST.CAC:	JSR EECLAC

;-------------------------
;  display accounting info
ST.ACC:
~JSR EEACC
~JMP ST.WAI

;-----------------------------
;  test ROM portion of EEROM
ST.EET:
~JSR GR.SCL
~JSR EETST1
~BEQ 10$
;	IFNE
~AND #0F0
~IFNE
~LDA #2C	; PC location 4A  high nib
~ELSE
~LDA #2D	; PC location 4B  high nib
~ENDIF
~BNE 20$	; BRA
;	ELSE
10$:
~LDA #24
20$:
;	ENDIF
~JSR MS.DRW
~JMP ST.WAI

;--------------------------------
;   draw convergence grid
ST.GRD:
~JSR GR.SCL

;  draw grid
~TRAI 0 TEMP1
~BEGIN
~LDA TEMP1
~CMP #80
~IFCC
~ADC #8
~ENDIF
~TAY
~LDA #7F

~LDX #0
~BEGIN
~STX XB
~STY YB
~STA VB
~INX
~EQEND

~LDX TEMP1
~LDY #0
~BEGIN
~STX XB
~STY YB
~STA VB
~INY
~EQEND

~LDA TEMP1
~ADD #10
~CMP #80
~IFEQ
~LDA #0FF-70
~ENDIF
~STA TEMP1
~CMP #00F
~EQEND

~JMP ST.WAI

~RTS


