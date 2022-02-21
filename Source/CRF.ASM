;	filename  CRF.MAC
; root file for Crystal Castles
; programmed by Franz Lanzinger
; Copyright 1983,  all rights reserved
~.TITLE ROOT
~.INCLUDE CSTART.MAC
~.ASECT
~.= WV.STR	;  split storage: program
~CTROM = .
~EW.NUM=16*16*2+WV.STR
~EW.BLK=EW.NUM+1
~.INCLUDE CGR.MAC
.WARN .	;  cgr
~.INCLUDE CWV.MAC
.WARN . ; cwv
~.INCLUDE CEN.MAC
.WARN .	;  cen
~.INCLUDE CCT.MAC
.WARN .	;  cct
~.INCLUDE CAL.MAC
.WARN .	;  cal
~.INCLUDE CCUBE.MAC	; includes checksum for ROM 2 (0C000-0E000)
.WARN .	;  ccube
~.REPT VC.SN-.
~.BYTE 0FF
~.ENDR
~.IF GE,.-VC.SN-1
~.ERROR	; code overlap
~.ENDC
.WARN .	;    should be CCE0
~.= VC.IN		;  0E000
~.INCLUDE CIN.MAC
.WARN . ; cin
~.INCLUDE CDB.MAC
.WARN . ; cdb
~.INCLUDE CMS.MAC
.WARN . ; cms
~.INCLUDE CSL.MAC
.WARN .	;  csl
~.INCLUDE CMR.MAC
.WARN .	;  cmr
~.INCLUDE CMN.MAC
.WARN . ; cmn
~.INCLUDE CMTB.MAC
.WARN .	;  cmtb
~.INCLUDE CST.MAC
.WARN . ;  cst
~.INCLUDE CSS.MAC
.WARN .	;  css
~.INCLUDE CEE.MAC
.WARN .	;  cee.mac
~.INCLUDE CEL.MAC
.WARN .	;  cel
~.INCLUDE CCN.MAC
.WARN .	;  ccn
~.INCLUDE CET.MAC
.WARN .	;  cet.mac
;~.REPT 0FFF8-.
;~.BYTE 0FF
;~.ENDR
.error	;  dummy
~HLL65
.IF EQ,CG.ROM-1
.VCTRS 0FFF8,INTER,INTER,START,INTER
.ENDC
.IF EQ,CG.ROM
.VCTRS 0FFF8,INTER,INTER,START
.ENDC
.END
