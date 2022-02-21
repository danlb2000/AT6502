;  filename CMTB.MAC
;  message table

;  message format:
;    x,y,word1,word2, ... , wordn


;  maze titles
;  get the gems bentley bear
MC.M00:	.BYTE	0B1,28
~.BYTE	2,1B,52
~.BYTE	3,1D
~.BYTE   2,1E
~.BYTE	3,1F

;  extra life
MC.M01:	.BYTE	0B1,30
~.BYTE	2D,0A1
~.BYTE   0,2E

;  tree wave
MC.M02:	.BYTE	0B1,30
~.BYTE	30,31

;  berthildas castle
MC.M03:	.BYTE	0B1,30
~.BYTE	33,2,34

;  pyramid
MC.M04:	.BYTE	0B1,2E
~.BYTE	03A

;  hidden spiral
MC.M05:	.BYTE	0B1,30
~.BYTE	4F,0,63

;  hidden ramp
MC.M06:	.BYTE	0B1,30
~.BYTE	4F,0A8

;  berthildas fortress
MC.M07:	.BYTE	0B1,30
~.BYTE	33,0,3F

;  impossible staircase
MC.M08:	.BYTE	0B1,30
~.BYTE	41,0,42

;  maze 1
MC.M09:	.BYTE	0B1,30
~.BYTE	44

;  cross maze
MC.M0A:	.BYTE	0B1,30
~.BYTE	46,47

;  berthildas dungeon
MC.M0B:	.BYTE	0B1,30
~.BYTE	33,0,48

;  crossroads
MC.M0C:	.BYTE	0B1,30
~.BYTE	4A

;  nasty tree
MC.M0D:	.BYTE	0B1,30
~.BYTE	36,30

;   the end
;
;  program by
;    franz
;  lanzinger
MC.M0E:	.BYTE	0B1,30
~.BYTE	52,0A0
~.BYTE   0,0,0B2
~.BYTE	2,0B1
~.BYTE	0,39

;  berthildas palace
MC.M0F:	.BYTE	0B1,30
~.BYTE	33,0,4E

;  credits
MC.M10:	.BYTE  0C0,50,	12

;  get ready
MC.M11:	.BYTE  060,80,  0E,0F

;  option screen
MC.M12:	.BYTE  030,30
~.BYTE  7,13,81,0

~.BYTE  0,0,0B,8B
~.BYTE  0,0,72,8C,8B
~.BYTE  0,0,73,8C,8B
~.BYTE  0,0,13,49
~.BYTE  0,0,4D,0A1
~.BYTE  0,0,2D,0A1

~.BYTE  0,0,65,81
~.BYTE  0,0,84

;  insert coin
MC.M13:	.BYTE  0B1,48,  0,0A,0B

;  game over
MC.M14:	.BYTE  060,80,  13,0D

;  player 1
MC.M15:	.BYTE  060,88,  14,15

;  player 2
MC.M16:	.BYTE  060,88,  14,16


;  checksum for rom at pc etc.
MC.M17:	.BYTE	020,70
~.BYTE	2A,20,91,0,2A,20,92,0,2A,20,93,0
~.BYTE	2A,20,94,0,2A,20,95

;  use secret warp number 1
;  jump at back corner of maze 1 level 1
MC.M18:	.BYTE	040,0C0
~.BYTE	96,61,97,15
~.BYTE	0,8A,98,0A2,99,5B
~.BYTE	5,47,15
~.BYTE	5,2B,15

;  atract mode messages
;  crystal castles  copyright atari 1983
MC.M19:	.BYTE	0B1,2C
~.BYTE	2,18,2,19
~.BYTE	0,0,21,0,22,0,23,1,24

;  warp activated, high score
MC.M1A:	.BYTE	0B6,28
~.BYTE	0,1C

;  warp instructions
MC.M1B:	.BYTE	0B1,38
~.BYTE	5D,0,37,0A4,0,64,8A,0,60,0,83,61,83

;  i give up:  you win
MC.M1C:	.BYTE	020,60
~.BYTE	9E,0,0,50,9F
~.BYTE	0,0,0A1,57,0,0,7A,57

;  crystal castles hall of fame
MC.M1D:	.BYTE	030,30
~.BYTE	18,19,5A,5B,5C

; enter your initials
MC.M1E:	.BYTE	040,0F0
~.BYTE	5D,5E,5F

;  level
MC.M1F:	.BYTE	0B1,58
~.BYTE   2,2B

;  you got the last gem
;  bonus 1000
MC.M20:	.BYTE	040,80
~.BYTE   50,51,52,53,54,6,57

;  they got the last gem
;  no bonus
MC.M21:	.BYTE	040,80
~.BYTE   55,51,52,53,54,6,56,57

;  accounting messages
MC.M22:	.BYTE	020,20
~.BYTE	5,70,0,0,0,71,74,0,72,74,0,73,74,0
~.BYTE	75,78,74,0,15,14,76,0,16,14,76,0,75,12,77
~.BYTE	0,79,13,7A
~.BYTE	0,7B

;  explanation board
MC.M23:	.BYTE	020,20
~.BYTE	8,66,0
~.BYTE	0,0,3,67,0,0,3,68,0A3
~.BYTE	3,0A4,50,69
~.BYTE	0,0,0,6A,54,45,0,6B,62
~.BYTE	0,0,6C,6B,0,6D,54,83,54
~.BYTE	0,0,3,6E,7A,0A4,3,52,6F
~.BYTE	0,0,5,8A,3C,83,43

;  EEROM OK
MC.M24:	.BYTE	040,80
~.BYTE	7F,87

; secret warp number two
MC.M25:	.BYTE	030,0C0
~.BYTE	96,61,97,16
~.BYTE	0,0,68,0A5,0A4
~.BYTE	0,8A,98,0A6,5B,4F,0A8
~.BYTE	5,0A7,2B,25

;  self test main menu
MC.M26:	.BYTE	020,38
~.BYTE	7C,7E,72,60
~.BYTE	0,7D,7E,73,60

~.BYTE	0,2,80
~.BYTE	0,2,70
~.BYTE	0,2,29,70
~.BYTE	0,2,13,81
~.BYTE	0,2,7F,8D
~.BYTE	0,2,82,7F
~.BYTE	0,2,58,5A,5B,5C

~.BYTE	0,0,0,82,89,7F,82,83,84

;  switch test explanation
MC.M27: .BYTE	020,50
~.BYTE	7,82,7F,0,0,8A,60,0,0B,60
~.BYTE	0,7,32,7F,0,35,0,3B,0,3D,0,3E,0,40

;  ram ok
MC.M28: .BYTE	020,50
~.BYTE	85,87

;  rom ok
MC.M29:	.BYTE	020,60
~.BYTE	86,87

;  rom failure
MC.M2A:	.BYTE	020,60
~.BYTE	86,88

;  staircase
MC.M2B:	.BYTE	0B1,30
~.BYTE	42

;  eerom failure at 4a
MC.M2C:	.BYTE	020,70
~.BYTE	0C,88,20,2C

;  eerom failure at 4b
MC.M2D:	.BYTE	020,70
~.BYTE	0C,88,20,2F

;  secret warp number 3
MC.M2E:	.BYTE	040,0C0
~.BYTE   96,61,97,25
~.BYTE	0,8A,98,0A2,99
~.BYTE	0,5B,4A,0A7,2B,27

;  amazingly good
MC.M2F:	.BYTE  70,70
~.BYTE  0AB,0AE

;  ridiculously good
MC.M30:	.BYTE	70,70
~.BYTE	0AA,0AE

; fantastically good
MC.M31:	.BYTE	70,70
~.BYTE	0AC,0AE

; very good
MC.M32:	.BYTE	70,70
~.BYTE	0A9,0AE

;  a  mutant
MC.M33:	.BYTE	70,70
~.BYTE	0AD

;  a video whiz
MC.M34:	.BYTE	70,70
~.BYTE	0B0

MC.DUM:	; dummy message



;  table of message locations
;  up to 128 messages
;  must be consecutive
MS.TAB:	.WORD MC.M00
~.WORD MC.M01
~.WORD MC.M02
~.WORD MC.M03
~.WORD MC.M04
~.WORD MC.M05
~.WORD MC.M06
~.WORD MC.M07
~.WORD MC.M08
~.WORD MC.M09
~.WORD MC.M0A
~.WORD MC.M0B
~.WORD MC.M0C
~.WORD MC.M0D
~.WORD MC.M0E
~.WORD MC.M0F
~.WORD MC.M10
~.WORD MC.M11
~.WORD MC.M12
~.WORD MC.M13
~.WORD MC.M14
~.WORD MC.M15
~.WORD MC.M16
~.WORD MC.M17
~.WORD MC.M18
~.WORD MC.M19
~.WORD MC.M1A
~.WORD MC.M1B
~.WORD MC.M1C
~.WORD MC.M1D
~.WORD MC.M1E
~.WORD MC.M1F
~.WORD MC.M20
~.WORD MC.M21
~.WORD MC.M22
~.WORD MC.M23
~.WORD MC.M24
~.WORD MC.M25
~.WORD MC.M26
~.WORD MC.M27
~.WORD MC.M28
~.WORD MC.M29
~.WORD MC.M2A
~.WORD MC.M2B,MC.M2C,MC.M2D,MC.M2E
~.WORD MC.M2F,MC.M30,MC.M31,MC.M32,MC.M33,MC.M34

~.WORD MC.DUM

