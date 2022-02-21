; filename CEL.MAC
;	JMP EC.UPD

;-----------------------------
;  routine to update elevators
EC.UPD:
~LDA EN.LMD
~IFEQ			;  stop elevators if player
~;  dead
~TR16AI EC.BLK,EC.PTR
~TRAM EC.NUM,EC.CUR
~IFNE
~BEGIN
~JSR EC.TRA
~JSR EL.MOV
~JSR EL.UPD
~DEC EC.CUR
~EQEND
~ENDIF
~ENDIF
~RTS

;-------------------------------------
;  routine to transfer elevator status
EC.TRA:
~LDY I,EC.BSZ-1
10$:	LDA NY,EC.PTR
~STA EL.MOD(Y)
~DEY
~BPL 10$

~RTS

~.INCLUDE BITPAT.MAC
EL.UPD:
~LDY I,EC.USZ-1
10$:	LDA EL.MOD(Y)
~STA NY,EC.PTR
~DEY
~BPL 10$
~AD16AI EC.PTR,EC.BSZ

~RTS

;-----------------------------------
;  routine to move a single elevator
EL.MOV:
~LDA EL.PRI
~ROR
~ROR
~JSR CL.PR

~LDY #0
~TR16AM EL.MAT TEMP1
~DEC16A TEMP1
~TRAM @TEMP1(Y) EL.TOP
~AD16AI TEMP1 2
~TRAM @TEMP1(Y) EL.BOT

~LDX EL.MOD
~BEQ 40$
~DEX
~BEQ 30$
~DEX
~BEQ 20$

~JSR EL.DW	; fall to bottom ** MODE 3
~TRAI 0,EL.UPF
~LDY #0	;
~LDA NY,EL.MAT
~SUB #1
~STA NY,EL.MAT
~CMP EL.BOT
~BNE 50$
~TRAM EL.WAI,EL.SIT
~TRAI 0,EL.MOD
~JMP 50$

20$:	DEC EL.SIT ;	sit at top	** MODE 2
~BNE 50$
~TRAI 0FF,EL.UPF
~JMP 45$

30$:	JSR EL.UP
~TRAI 0,EL.UPF
~LDY #0 ;	rise to top	** MODE 1
~LDA NY,EL.MAT
~ADD #1
~STA NY,EL.MAT
~CMP EL.TOP
~BNE 50$
~TRAM EL.WAI,EL.SIT
~JMP 45$
40$:	DEC EL.SIT ;	sit at bottom   ** MODE 0
~BNE 50$
~TRAI 0FF,EL.UPF
45$:	INC EL.MOD
50$:
~RTS

;-----------------------------
;  routine to move elevator up
EL.UP:
~LDA EL.VP
~LDY #0
~SUB @EL.MAT(Y)
~SUB #04
~STA LN.VCR

~LDA EL.HP
~ADD #04
~STA LN.HCR	;  start at the upper right corner

~LDA FC.BV3
~STA LN.CO2
~JSR LN.F2
~DEC LN.VCR
~TRAM FC.BVB LN.CO2
~JSR LN.F2

~LDA LN.HCR
~SUB #4+8
~STA LN.HCR
~INC LN.VCR
~INC LN.VCR	;  now at left corner

~TRAM FC.BVB LN.CO1
~JSR LN.F1
~INC LN.VCR
~TRAM FC.BV3 LN.CO1
~JSR LN.F1

~LDA LN.HCR
~ADD #8
~STA LN.HCR
~INC LN.VCR	; now at front corner

~TRAM FC.BVB LN.CO1
~STA LN.CO2
~JSR LN.F1
~JSR LN.F2
~INC LN.VCR

~BIT EL.UPF
~BMI 30$

~TRAM FC.BV1 LN.CO1
~TRAM FC.BV2 LN.CO2
30$:
~JSR LN.F1
~JSR LN.F2

~TRAM LN.HCR XB	 ; front corner pixel
~TRAM LN.VCR YB
~TRAM FC.BVB VB

~RTS

;----------------------------------
;  routine to move elevator down  -
EL.DW:
~LDA EL.VP
~LDY #0
~SUB @EL.MAT(Y)
~STA LN.VCR
~TRAM EL.HP LN.HCR	; start at the front corner

~TRAM FC.BV3 LN.CO2
~STA LN.CO1
~JSR LN.F1
~JSR LN.F2

~INC LN.VCR
~TRAM FC.BVB LN.CO1
~STA LN.CO2
~JSR LN.F1
~JSR LN.F2

~LDA LN.HCR
~SUB #8
~STA LN.HCR
~DEC LN.VCR
~DEC LN.VCR	; now at left corner

~TRAM FC.BVB LN.CO1
~JSR LN.F1
~DEC LN.VCR

~BIT EL.UPF
~BMI 10$

~TRAM FC.BV1 LN.CO1
10$:
~JSR LN.F1

~TRAM LN.HCR XB	; left corner pixel
~TRAM LN.VCR YB
~TRAM FC.BVB VB

~LDA LN.HCR
~ADD #4+8
~STA LN.HCR
~DEC LN.VCR	; now at right corner

~TRAM FC.BVB LN.CO2
~JSR LN.F2
~DEC LN.VCR

~BIT EL.UPF
~BMI 20$

~TRAM FC.BV2 LN.CO2
20$:
~JSR LN.F2

~TRAM LN.HCR XB	; right corner pixel
~TRAM LN.VCR YB
~TRAM FC.BVB VB

~LDA XB		; upper corner pixel
~SUB #08
~STA XB
~DEC YB
~DEC YB
~TRAM FC.BVB VB

~RTS


