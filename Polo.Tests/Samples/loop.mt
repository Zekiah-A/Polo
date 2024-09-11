##
--- loop.mt ---
This file only makes use of compiler primitives, hence having no
dependency on the standard library.
##
let a:i32 = 0

# While loop
while a < 10
	debug(a)
	a++

# For loop
for let b:i32 = 10; b >= 0; b--
	debug(b)

# Nested while loop
let c:i32 = 0
while c < 10
	let d:i32 = 0
	dLoop:
	while d < 10
		if d == 3
			# One two, skip a few
			d += 4
			goto dLoop
		debug(c, d)
		d++
	c += 1
