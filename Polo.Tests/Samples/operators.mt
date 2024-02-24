##
--- operators.mt ---
This file only makes use of compiler primitives, hence having no 
dependency on the standard library. Intended to demonstrate the
absolute simplest possible operation in the programming language
##
let a: i32 = 4
let b: i32 = 6
debug("a:", a, "b:", b)

# Add -> 10
debug("a + b:", a + b)

# Equality -> 1
debug("a == a:", a == a)

# Equality -> 0
debug("a == b:", a == b)