##
--- operators.mt ---
This file only makes use of compiler primitives, hence having no 
dependency on the standard library. Intended to demonstrate the
absolute simplest possible operations in the programming language.
##

# Assignment
let a:i32 = 4
let b:i32 = 6
debug("a:", a, "b:", b)

# Add -> 10
debug("a + b:", a + b)

# Subtract -> -2
debug("a - b:", a - b)

# Multiply -> 24
debug("a * b:", a * b)

# Divide -> 0
debug("a / b:", a / b)

# Remainder -> 4
debug("a % b:", a % b)

# Equality -> 1
debug("a == a:", a == a)

# Equality -> 0
debug("a == b:", a == b)

# Operator assignment
let c:i32 = 0
let d:i32 = 0
debug("c:", c, "d:", d)
debug("c += a", c += a)
debug("d -= b", d -= b)
