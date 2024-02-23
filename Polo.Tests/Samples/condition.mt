##
--- conition.mt ---
This file only makes use of compiler primitives, hence having no 
dependency on the standard library.
##
let a: i32 = 4
let b: i32 = 6
let result:i32 = a + b

if result == 10
	debug("A + B was", result)
##
else if result == 11
	debug("This should never run #1")
else if result == 12
	debug("This should never run #2")
else
	debug("This should never run #3")

if true
	debug("True is true")
if false
	debug("This should never run #4")
##