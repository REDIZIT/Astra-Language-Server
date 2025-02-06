	section .data
	str_Hello_from_Astra_via_Console_instance_#2 db 40, "Hello from Astra via Console instance #2", 0
	
	section .text
	global main
main:
	push rbp
	mov rbp, rsp
	
	sub rsp, 8 ; allocate string 'str' at [rbp-8]
	mov qword [rbp-8], str_Hello_from_Astra_via_Console_instance_#2
	
	sub rsp, 8 ; allocate long 'anon_1' at [rbp-16]
	
; -- new Console
	sub rsp, 8 ; allocate Console 'anon_2' at [rbp-24]
; -- heap alloc
	mov qword [rbp-16], 0x110
	mov rbx, [0x100]
	add rbx, 1
	mov [0x100], rbx
	sub rsp, 8 ; allocate long 'console' at [rbp-32]
	mov qword rbx, [rbp-16]
	mov qword [rbp-32], rbx
	
; -- Astra.Compilation.Node_FieldAccess.message()
; -- arguments generation
; -- arguments pushing
	mov rbx, [rbp-32] ; self
	push rbx
	mov rbx, [rbp-8] ; arg[0] = str
	push rbx
	call message
	add rsp, 16
	
	
	mov rsp, rbp
	pop rbp
	ret
message:
	push rbp
	mov rbp, rsp
	
	sub rsp, 8 ; allocate ptr 'vga_pointer' at [rbp-8]
	mov qword [rbp-8], 0
	
; -- generating target for assign
; -- generating value for assign
; -- Assign vga_pointer = 0x200000
	mov qword [rbp-8], 0x200000
	sub rsp, 8 ; allocate byte 'color' at [rbp-16]
	mov qword [rbp-16], 0x2f
	
	sub rsp, 8 ; allocate int 'i' at [rbp-24]
	mov qword [rbp-24], 0
	
while_condition:
	
; -- String get length str
	sub rsp, 8 ; allocate ptr 'anon_1' at [rbp-32]
; -- ToPtr str (heap data)
	mov rbx, rbp
	add rbx, 16
	mov rbx, [rbx]
	mov [rbp-32], rbx
	sub rsp, 8 ; allocate byte 'anon_2' at [rbp-40]
; -- Ptr get
	mov rbx, [rbp-32]
	mov rdx, [rbx]
	mov byte [rbp-40], dl
	sub rsp, 8 ; allocate bool 'anon_3' at [rbp-48]
	mov rbx, [rbp-24]
	mov rdx, [rbp-40]
	cmp rbx, rdx
	mov rbx, 0
	setl bl
	mov [rbp-48], rbx
	
	mov rbx, [rbp-48]
	cmp rbx, 0
	jle while_end
	
; -- String get char str[i]
	sub rsp, 8 ; allocate ptr 'anon_4' at [rbp-56]
; -- ToPtr str (heap data)
	mov rbx, rbp
	add rbx, 16
	mov rbx, [rbx]
	mov [rbp-56], rbx
; -- Ptr shift
	mov rbx, [rbp-56]
	mov rdx, [rbp-24]
	add rbx, rdx
	add rbx, 1 ; additionalShift
	mov [rbp-56], rbx
	sub rsp, 8 ; allocate byte 'anon_5' at [rbp-64]
; -- Ptr get
	mov rbx, [rbp-56]
	mov rdx, [rbx]
	mov byte [rbp-64], dl
	sub rsp, 8 ; allocate byte 'c' at [rbp-72]
	mov qword rbx, [rbp-64]
	mov qword [rbp-72], rbx
	
; -- PtrSet c to vga_pointer
; -- Ptr set
	mov rbx, [rbp-8]
	mov rdx, [rbp-72]
	mov byte [rbx], dl
	
; -- Literal = 1
	sub rsp, 8 ; allocate long 'anon_6' at [rbp-80]
	mov qword [rbp-80], 1
	
; -- Shift pointer vga_pointer by anon_6
; -- Ptr shift
	mov rbx, [rbp-8]
	mov rdx, [rbp-80]
	add rbx, rdx
	mov [rbp-8], rbx
	
; -- PtrSet color to vga_pointer
; -- Ptr set
	mov rbx, [rbp-8]
	mov rdx, [rbp-16]
	mov byte [rbx], dl
	
; -- Literal = 1
	sub rsp, 8 ; allocate long 'anon_7' at [rbp-88]
	mov qword [rbp-88], 1
	
; -- Shift pointer vga_pointer by anon_7
; -- Ptr shift
	mov rbx, [rbp-8]
	mov rdx, [rbp-88]
	add rbx, rdx
	mov [rbp-8], rbx
	
; -- generating target for assign
; -- generating value for assign
; -- Literal = 1
	sub rsp, 8 ; allocate long 'anon_8' at [rbp-96]
	mov qword [rbp-96], 1
	
	sub rsp, 8 ; allocate int 'anon_9' at [rbp-104]
	mov rbx, [rbp-24]
	mov rdx, [rbp-96]
	add rbx, rdx
	mov [rbp-104], rbx
	
; -- Assign i = anon_9
	mov qword rbx, [rbp-104]
	mov qword [rbp-24], rbx
	jmp while_condition
while_end:
	
	
	mov rsp, rbp
	pop rbp
	ret