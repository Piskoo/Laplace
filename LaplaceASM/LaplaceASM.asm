.code
;parameters
;rcx - input
;rdx - output
;r8 - filter
;r9 - current byte index
;[rsp+40] - stride

;purpose of registers
;r10 - used to iterate through kernel convolution 
;r11 - amount of bytes left to process

processImage proc export
; set amount of bytes to process
	MOV R11, [RSP+40]
	SUB R11, 8
NextPixel:
	MOV R10, R9 ; middle 
	SUB R10, [RSP+40] ; top middle
	SUB R10, 4 ; top left
	;set xmm registers with 3x3 matrix dword color coded pixels
	PMOVZXBD xmm0, [RCX+R10] ; top left
	PMOVZXBD xmm1, [RCX+R10+4] ; top middle
	PMOVZXBD xmm2, [RCX+R10+8] ; top right
	ADD R10, [RSP+40] ; next line 
	PMOVZXBD xmm3, [RCX+R10] ; middle left
	PMOVZXBD xmm4, [RCX+R10+4] ; middle
	PMOVZXBD xmm5, [RCX+R10+8] ; middle right
	ADD R10, [RSP+40] ; next line
	PMOVZXBD xmm6, [RCX+R10] ; bottom left
	PMOVZXBD xmm7, [RCX+R10+4] ; bottom middle
	PMOVZXBD xmm8, [RCX+R10+8] ; bottom right

	; multiply filter value by each pixel
	PMOVSXBD xmm9, [R8]
	VPBROADCASTD xmm9, xmm9
	PMULLD xmm0, xmm9

	PMOVSXBD xmm9, [R8+4]
	VPBROADCASTD xmm9, xmm9
	PMULLD xmm1, xmm9

	PMOVSXBD xmm9, [R8+8]
	VPBROADCASTD xmm9, xmm9
	PMULLD xmm2, xmm9

	PMOVSXBD xmm9, [R8+12]
	VPBROADCASTD xmm9, xmm9
	PMULLD xmm3, xmm9

	PMOVSXBD xmm9, [R8+16]
	VPBROADCASTD xmm9, xmm9
	PMULLD xmm4, xmm9

	PMOVSXBD xmm9, [R8+20]
	VPBROADCASTD xmm9, xmm9
	PMULLD xmm5, xmm9

	PMOVSXBD xmm9, [R8+24]
	VPBROADCASTD xmm9, xmm9
	PMULLD xmm6, xmm9

	PMOVSXBD xmm9, [R8+28]
	VPBROADCASTD xmm9, xmm9
	PMULLD xmm7, xmm9

	PMOVSXBD xmm9, [R8+32]
	VPBROADCASTD xmm9, xmm9
	PMULLD xmm8, xmm9

	; sum pixel compounds to xmm0
	PADDD xmm0, xmm1
	PADDD xmm0, xmm2
	PADDD xmm0, xmm3
	PADDD xmm0, xmm4
	PADDD xmm0, xmm5
	PADDD xmm0, xmm6
	PADDD xmm0, xmm7
	PADDD xmm0, xmm8

	;check if pixel value is between 0 and 255 and save pixel output ptr
Blue:
	MOVD EAX, xmm0
	CMP EAX, 255
	JG BlueGreater
	CMP EAX, 0
	JL BlueLesser
Green:
	MOV [RDX+R9], AL
	PSRLDQ xmm0,4
	MOVD EAX, xmm0
	CMP EAX, 255
	JG GreenGreater
	CMP EAX, 0
	JL GreenLesser
Red:
	MOV [RDX+R9+1], AL
	PSRLDQ xmm0,4
	MOVD EAX, xmm0
	CMP EAX, 255
	JG RedGreater
	CMP EAX, 0
	JL RedLesser
Alpha:
	MOV [RDX+R9+2], AL
	MOV EAX, 255
	MOV [RDX+R9+3], AL
	ADD R9, 4
	SUB R11, 4
	CMP R11, 0 ; check if no more bytes to process
	JNE NextPixel
	ret

BlueGreater:
	MOV AL, 255
	JMP Green
BlueLesser:
	MOV AL, 0
	JMP Green

GreenGreater:
	MOV AL, 255
	JMP Red
GreenLesser:
	MOV AL, 0
	JMP Red

RedGreater:
	MOV AL, 255
	JMP Alpha
RedLesser:
	MOV AL, 0
	JMP Alpha

processImage endp
end