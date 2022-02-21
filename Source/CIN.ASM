;	filename   CIN.MAC
;-------------------------------
; interrupt routine and start
;----------------------
START:	    ;  this must be at E000 so no bank select
~;  problems at power up
~TRAI 0 HW.BSL	;  select bank 0 and jump
~JMP MN.PWR
~RTS	;  general purpose RTS

;  WARNING:  don't change the above

;----------------------------
;  interrupt routine
INTER:
~PHA
~TXA
~PHA
~TYA
~PHA
~CLD		;  set to binary mode
~TSX
~BPL STJUMP

~LDA $$CRDT
~CMP #20
~IFCS
STJUMP:	 JMP START
~ENDIF

~INC ISYNC	;  interrupt sync
~INC $INTCT

;  bank select
~LDX #0
~LDA WV.STR
~IFEQ		;  detect that using bank 1
~TRAI 0FF IN.BSL
~STX HW.BSL
~ELSE
~STX IN.BSL
~ENDIF

;  only VBLANK
~LDA HW.VBL
~AND #MA.VBL
~IFNE
~JSR VBHAND
~TRAI 0 ISYNC
~ENDIF

;  every interrupt
~JSR MN.SNM	;  sound modify

~LDA ST.CRF
~IFNE
~TRAM $LAM $COINA
~JSR MOOLAH	;  coin routine

~LDA $CCTIM+1
~ROL
~ROL
~STA $CNCTR

~LDA $CCTIM+2
~ROL
~ROL
~STA $CNCTR+1
~ENDIF

;  no more than 18 credits
~LDA #12
~CMP $$CRDT
~IFCC
~STA $$CRDT
~ENDIF

;  restore bank select
~LDA IN.BSL
~STA HW.BSL

~STA HW.IAC	;  interrupt acknowledge
~STA HW.WDC	;  woof
~PLA
~TAY
~PLA
~TAX
~PLA
~RTI

;-----------------------------------
; routine to handle VBLANK interrupt
VBHAND:
~INC SYNC
~BEQ STJUMP		;  software watchdog
;  if copyright violation
~LDA AT.PRT
~IFNE
~INC TEMP1
~ENDIF

~TRAM HW.FLV HW.FLP	;  flip

;  do scrolling near beginning of VBLANK !
~LDA WV.SCF
~CMP #1
~IFEQ		;  scroll mode 1: camera to right
~ADAI 4 HW.HSV
~JMP 10$
~ENDIF

~CMP #2
~IFEQ		;  scroll mode 2: camera to left
~SBAI 4 HW.HSV
~JMP 10$
~ENDIF

~CMP #3
~IFEQ		;  scroll mode 3: to the top
~SBAI 4 HW.VSV
~CMP #14
~IFEQ
~TRAI 0FC HW.VSV
~ENDIF
~JMP 10$
~ENDIF

~;  scroll mode 0:  no scroll
~TRAI 0 HW.HSV
~TRAI 18 HW.VSV
10$:
~TRAM HW.VSV HW.VSC	; vert scroll
~TRAM HW.HSV HW.HSC	; horiz scroll

20$:

;  statistics
~INC ST.VBC
~IFEQ

~LDA ST.PLY	;  if in game play
~IFNE
~SED
~CLC
~LDA ST.TIM
~ADC #1
~STA ST.TIM
~LDA 1+ST.TIM
~ADC #0
~IFCC		;  no overflow
~STA 1+ST.TIM
~ENDIF
~CLD

~ENDIF
~ENDIF


;  now do rest
~JSR MT.TRA	;  motion object transfer
~JSR CL.TRA	;  color transfer

;  flash trackball
~LDX TFLASH
~CMP #0FF
~IFNE
~LDX #0FF
~LDA FRAME
~AND TFLASH
~IFEQ
~LDX #0
~ENDIF
~ENDIF
~STX HW.TL

~RTS


~.BYTE CHK03
.PAGE
;-------------------------------------------------------
; power on
MN.PWR:
;  init hardware
MN.HIN:
~SEI			; disable interrupts
~CLD			; no decimal mode

~LDX #0
~STX EE.SIR	;  enable recall, disable store
~STX EE.REC	;  recall EE.ROM on power on
~STX EE.RE2
~DEX
~STX EE.RE2	;  alternate recall

MN.SLT:			;  jump here for self-test
~SEI
~CLD

;  bothram and trackball init
~LDX #0
~STX HW.BHR		;  bothram not tied

;  wait until VBLANK
~BEGIN
~LDA HW.VBL
~AND #MA.VBL
~NEEND
~TRAI 18 HW.VSC
~STX HW.HSC
~STX HW.FLP

;  auto inc
~DEX		; 0FF
~STX HW.AX
~STX HW.AY
~STX HW.XIN
~STX HW.YIN


~.IF NE,CG.ST
~.IFT
~JMP MN.ST
~.IFF
~JMP MN.HIC
~.ENDC



