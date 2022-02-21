
;  filename CAL.MAC

;-----------------------
;  message display board
AL.MDB:
~TRAI 30 AL.X
~TRAI 7C AL.Y
~LDA #0A0
~JSR SC.ERA
~TRAI 84 AL.Y
~LDA #0A0
~JSR SC.ERA
~TRAI 8C AL.Y
~LDA #0A0
~JSR SC.ERA
~RTS

;-------------------------------------------------------
;  draw message at beginning of wave, or in attract mode
MS.MWV:

~JSR AL.BER
~LDA ATRACT
~IFNE
~LDA WV.XCO
~STA TEMP5+1

~LDA WV.MFL
~IFNE

~LDA WV.MNM
~IFEQ		;  on wave 1
~LDX WV.WAR
~IFNE
~STX TEMP5+1
~LDA #1B
~ENDIF
~ELSE
~CMP #1
~IFEQ
~LDX EEEXTR
~IFNE
~LDA #2B	;  if no extra life, use ordinary title
~ENDIF
~ENDIF
~ENDIF
~JSR MS.DRW	; wave titles
~ENDIF

~LDA #1F
~JSR MS.DRW	; level

~LDA TEMP5+1	;  level number
~ADD #1
~JSR DG.2OT
~ELSE
~LDA #19	; play crystal castles
~JSR MS.DRW
~ENDIF

~RTS

;--------------------------------
;  display score for both players
SC.2PL:
~JSR SC.OT2
~LDA PL.FLG
~IFNE
~JSR SC.SWT
~JSR SC.OT2
~JSR SC.SWT
~ENDIF
~RTS
SC.SWT:
~LDA PL.UP		;  switch who is up
~EOR #0FF
~STA PL.UP
~TR24AM P1.SCO TEMP1	;  and switch score
~TR24AM P2.SCO P1.SCO
~TR24AM TEMP1 P2.SCO
~RTS

;---------------------------------
;   display score
SC.OUT:
SC.OT2:
~LDA PL.UP
~IFEQ
~LDA #2E
~ELSE
~LDA #37
~ENDIF
~STA AL.Y
~TRAI 10 AL.X

~TR24AM SC.SCO SC.NM	;  score
~JSR SC.NDS
~RTS

;----------------------
;  6 - digit number display
SC.NDS:
~TRAI 6*6 SC.LEF	; pixels left to erase
~TRAI 0 SC.FNZ	; 1= found first non-zero digit

;  first digit
~LDA SC.NM+2
~LSRS 4
~STA SC.DIG
~JSR AL.DGO
;  second digit
~LDA SC.NM+2
~AND #0F
~STA SC.DIG
~JSR AL.DGO

;  third digit
~LDA SC.NM+1
~LSRS 4
~STA SC.DIG
~JSR AL.DGO

;  fourth digit
~LDA SC.NM+1
~AND #0F
~STA SC.DIG
~JSR AL.DGO

;  fifth digit
~LDA SC.NM
~LSRS 4
~STA SC.DIG
~JSR AL.DGO

;  sixth digit
~INC SC.FNZ	;  output digit even if score=0
~LDA SC.NM
~AND #0F
~STA SC.DIG
~JSR AL.DGO

~LDA SC.LEF
~JSR SC.ERA
~RTS


;------------------------------------------
;  lives and score display at start of wave
SC.LDS:
~JSR SC.LD2
~JSR SC.OT2

~RTS

SC.LD2:
~TRAM WV.LIV TEMP5
~TRAI 0E8 AL.Y
~TRAI 010 AL.X

~LDA #6*6
~JSR SC.ERA

;  life symbol
~TRAI 25 SC.DIG

~LDY TEMP5
~DEY
~STY TEMP5
10$:
~DEC TEMP5
~BMI 20$
~JSR AL.DGO
~JMP 10$
20$:
~RTS

;-----------------------
;  board erase

AL.BER:
~TRAI 8 TEMP2
~TRAI 0B0 AL.X
~TRAI 02E AL.Y
~BEGIN
~LDA #0B*6+3
~JSR SC.ERA
~ADAI 08 AL.Y
~DEC TEMP2
~EQEND
~RTS


;-----------------------------
;  hall of fame drawing
;  (A) has index of upper left score
HF.DRW:

~STA TEMP4
~SUB #20			; display  32 entries
~STA TEMP4+1

~JSR GR.SCL
~LDA #1D
~JSR MS.DRW		;  hall of fame title

~HF.YOF=48
~TRAI HF.YOF AL.Y

~TRAI 0 TEMP5
~STA AT.PRT
~BEGIN
~LDA TEMP5
~AND #080
~ADD #2*6
~STA AL.X

~LDA #HFSIZ
~SUB TEMP4
~JSR AL.CNV

~STA SC.NM
~STX 1+SC.NM
~TRAI 0 2+SC.NM
~JSR SC.NDS		;  rank

~LDA TEMP5
~AND #080
~ADD #6*6
~STA AL.X

~LDX TEMP4
~LDA SC.HS1(X)
~STA SC.NM
~LDA SC.HS2(X)
~STA 1+SC.NM
~LDA SC.HS3(X)
~STA 2+SC.NM
~JSR SC.NDS


;  initials
~LDA TEMP5
~AND #080
~ADD #0D*6
~STA AL.X

~LDX TEMP4
~TRAM   SC.HI1(X) AL.DIG
~JSR AL.DRW
~ADAI 6 AL.X
~LDX TEMP4
~TRAM   SC.HI2(X) AL.DIG
~JSR AL.DRW
~ADAI 6 AL.X
~LDX TEMP4
~TRAM   SC.HI3(X) AL.DIG
~JSR AL.DRW

~ADAI 8 AL.Y
~LDA TEMP5
~CMP #78
~IFEQ
~TRAI HF.YOF AL.Y
~ENDIF
~ADAI 8 TEMP5
~DEC TEMP4
~LDA TEMP4
~CMP TEMP4+1
~EQEND

~LDA ATRACT
~IFEQ
~TRAI 4 1+MN.DEL
~LDA HW.ST1
~AND #MA.ST1
~IFEQ
~LDA HW.ST2
~AND #MA.ST2
~IFEQ
~JSR EEACC
~ENDIF
~ENDIF

~BEGIN
~JSR MN.FRA
~LDA $$CRDT
~BNE 30$
~DEC MN.DEL
~IFEQ
~DEC 1+MN.DEL
~ENDIF
~LDA 1+MN.DEL
~EQEND
~30$:
~ENDIF
~RTS

;-------------------------
;  update high score table
SC.HSU:
~TRAI HFSIZ-1 SC.NWP	;  HFSIZ entries

~BEGIN
~LDX SC.NWP
~SED
~LDA SC.HS1(X)
~SUB SC.SCO
~LDA SC.HS2(X)
~SBC 1+SC.SCO
~LDA SC.HS3(X)
~SBC 2+SC.SCO
~CLD
~IFCC		;  if current score higher
~CPX #1		;  shift down lower scores
~IFCS		;  if necessary
~LDY #0
~LDX #1
~BEGIN
~TRAM SC.HS1(X) SC.HS1(Y)
~TRAM SC.HS2(X) SC.HS2(Y)
~TRAM SC.HS3(X) SC.HS3(Y)
~TRAM SC.HI1(X) SC.HI1(Y)
~TRAM SC.HI2(X) SC.HI2(Y)
~TRAM SC.HI3(X) SC.HI3(Y)
~INX
~INY
~CPY SC.NWP
~EQEND
~ENDIF
~LDX SC.NWP	;  record score
~TRAM   SC.SCO SC.HS1(X)
~TRAM 1+SC.SCO SC.HS2(X)
~TRAM 2+SC.SCO SC.HS3(X)
~LDA #4A
~STA SC.HI1(X)	;  blanks for initials for now
~STA SC.HI2(X)
~STA SC.HI3(X)
~JMP 50$
~ENDIF

~DEC SC.NWP
~LDA SC.NWP
~CMP #0FF
~EQEND
50$:
;  calculate SC.WLT, position where to enter initials
~LDA SC.NWP
~CMP #HFSIZ-10
~IFCS
~LDA #HFSIZ-1
~STA TEMP1
~SUB SC.NWP
~ELSE
~CMP #10
~IFCC
~LDA #1F
~STA TEMP1
~SUB SC.NWP
~ELSE
~ADD #10
~STA TEMP1
~LDA #10
~ENDIF
~ENDIF
~STA SC.WLT

~RTS


HF.TBB:	.WORD SC.HI1,SC.HI2,SC.HI3
;---------------------------------
;   enter initials for high score
SC.INE:
~TRAI 5 SN.NUM
~JSR MN.SN1		;  start high score music


~TRAI 0 WV.XCO
~STA WV.YCO
~JSR WV.CMP		;  set colors

~LDA #1E
~JSR MS.DRW		;  enter initials message

~TRAI 5F AL.COL		;  red

~LDA SC.WLT		;  new pos of initials
~AND #0F
~ASLS 3
~ADD #HF.YOF
~STA AL.Y

~LDA SC.WLT
~AND #10
~ASLS 3
~ADD #0D*6
~STA AL.X

~LDA #0
~STA SC.WLT

~TR16AM SC.PRH SC.HSO


~TR16AI 0 WV.TIM
~BEGIN
~JSR MN.FRA

~JSR TR.DEL
~TRAM TR.XD TEMP1
~IFMI
~LDA #0FF
~ELSE
~LDA #0
~ENDIF
~STA 1+TEMP1

~AD16AM SC.HSO TEMP1

~LDA 1+SC.HSO
~ROR
~STA TEMP1
~LDA SC.HSO
~ROR
~ROR TEMP1
~ROR
~LSRS 3		;  this divides SC.HSO by 2^5

~ADD #4A
~STA AL.DIG
~TRAI 6 TEMP1
~JSR SC.ERA
~JSR AL.DRW

~JSR EN.BRD

~LDA EN.JBP
~IFNE
~ADAI 6 AL.X
~LDA SC.WLT
~ASL
~TAY
~TR16AM HF.TBB(Y) TEMP2
~LDY SC.NWP		;  where in table
~TRAM AL.DIG @TEMP2(Y)	;  record initial
~LDY SC.WLT
~STA SC.HSD(Y)

~TYA		; init next initial
~ASL
~TAY
~TR16AM SC.HSO SC.PRH(Y)
~INY
~INY
~TR16AM SC.PRH(Y) SC.HSO
~INC SC.WLT
~ENDIF

~LDA 1+WV.TIM	;  after 24 secs, leave
~CMP #6
~BEQ 50$

~LDA SC.WLT
~CMP #3
~EQEND

50$:
;     record in EEROM
~LDX #HFSIZ-4
~CPX SC.NWP
~IFCC
~LDY #2
~BEGIN
~INX
~TRAM SC.HS1(X)	HFPSCO(Y)
~TRAM SC.HS2(X) 3+HFPSCO(Y)
~TRAM SC.HS3(X) 6+HFPSCO(Y)
~TRAM SC.HI1(X) HFPINI(Y)
~TRAM SC.HI2(X) 3+HFPINI(Y)
~TRAM SC.HI3(X) 6+HFPINI(Y)
~DEY
~MIEND

~LDY #0
~STY EE.SIR	;  don't store while updating
~LDX #18.-1
~JSR EEUPD
~ENDIF

~TRAI 040 MN.DEL
~BEGIN
~JSR MN.FRA
~DEC MN.DEL
~EQEND

~SEI		;  stop all sounds
~JSR RCN
~CLI

~RTS


