  ##
--- nodeps.mt ---
This file only makes use of compiler primitives, hence having no 
dependency on the standard library.
##
fn addOne(i: __i32): __i32
	i = i + 1
	return i

# Add one to input number
let number:__i32 = 0
number = addOne(number)
debug("Value of a is now", number)