	mov rbx, 0
	push rbx ; return int
	call program.main
	add rsp, 8
	pop rax
	mov 0x00, rax
	exit
	
	
	
program.main:
	push rbp
	mov rbp, rsp
	
	sub rsp, 8 ; allocate int 'a' at [rbp-8]
	mov qword [rbp-8], 42
	
	sub rsp, 8 ; allocate int 'b' at [rbp-16]
	mov qword [rbp-16], 3
	
; -- Literal = 2
	sub rsp, 8 ; allocate long 'anon_1' at [rbp-24]
	mov qword [rbp-24], 2
	
	sub rsp, 8 ; allocate int 'anon_2' at [rbp-32]
	mov rdi, [rbp-8]
	mov rax, [rbp-16]
	mul rdi
	mov [rbp-32], rax
	
	sub rsp, 8 ; allocate int 'anon_3' at [rbp-40]
	mov rbx, [rbp-24]
	mov rdx, [rbp-32]
	add rbx, rdx
	mov [rbp-40], rbx
	
	sub rsp, 8 ; allocate int 'c' at [rbp-48]
	mov qword rbx, [rbp-40]
	mov qword [rbp-48], rbx
	
	
	mov rbx, [rbp-48]
	mov [rbp+24], rbx
	mov rsp, rbp
	pop rbp
	ret