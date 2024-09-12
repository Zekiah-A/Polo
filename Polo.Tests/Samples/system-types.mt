##
--- system-types.mt ---
This file only makes use of compiler primitives, hence having no
dependency on the standard library.
##

# System int
let i1:int = 4
let i2:int = 6
let intResult:int = i1 + i2
debug(intResult)

debug('-','-','-','-')

# System float
let f1:float = 1.1231254125
let f2:float = 3.3523251341
let floatResult:float = f1 + f2
debug(floatResult)

debug('-','-','-','-')

# System char + block redefenition
let c1:char = 'a'
debug(c1)
	let c1:char = 'b'
	debug(c1)
debug(c1)

debug('-','-','-','-')

# System char + block assignment
let c2:char = 'c'
debug(c2)
	c2 = 'b'
	debug(c2)
debug(c2)
