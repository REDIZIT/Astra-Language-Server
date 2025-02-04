	section .data
	str_Hello_from_Astra db 16, "Hello from Astra", 0
	
	section .text
	mov rbx, 0
	push rbx ; return int
	call main
	add rsp, 8
	pop rax
	mov 0x00, rax
	exit
main:
	push rbp
	mov rbp, rsp
	
	sub rsp, 8 ; allocate string 'str' at [rbp-8]
	mov qword [rbp-8], str_Hello_from_Astra
	
	sub rsp, 8 ; allocate ptr 'anon_1' at [rbp-16]
; -- ToPtr str (heap data)
	mov rbx, rbp
	add rbx, -8
	mov rbx, [rbx]
	mov [rbp-16], rbx
	sub rsp, 8 ; allocate ptr 'str_pointer' at [rbp-24]
	mov qword rbx, [rbp-16]
	mov qword [rbp-24], rbx
	
	sub rsp, 8 ; allocate ptr 'vga_pointer' at [rbp-32]
	mov qword [rbp-32], 0
	
	
; -- Literal = 777
	sub rsp, 8 ; allocate long 'anon_2' at [rbp-40]
	mov qword [rbp-40], 777
	
	mov rbx, [rbp-40]
	mov [rbp+24], rbx
	mov rsp, rbp
	pop rbp
	ret