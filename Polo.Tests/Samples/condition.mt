##
--- conition.mt ---
This file only makes use of compiler primitives, hence having no 
dependency on the standard library.
##
let a: i32 = 4
let b: i32 = 6
let result:i32 = a + b

if result == 9
	debug("a + b = 9", result)
##
else if result == 11
	debug("This should never run #1")
else
	debug("This should never run #2")
if true
	debug("True is true")
if false
	debug("This should never run #3")
##