##
--- overload.mt ---
This file demonstrates the ability to overload operators, in order
to execute special methods to handle the usage of such operators on
certain types
##
type Vector2:
	let x:i32
	let y:i32
	
	Vector2(xComponent, yComponent):
		x = xComponent
		y = yComponent
	
	# Special operator overload method
	fn *(Vector2 other):
		return Vector2(x * other.x, y * other.y)
		
let vectorOne:Vector2 = Vector2(5, 5)
let vectorTwo:Vector2 = Vector2(10, 10)

# No primitive multiplication operator exists, so overload method
# will be used. This code can be thought of as vectorOne.*(vector2)
let dotProduct = vectorOne * vector2

debug("Result:", dotProduct.x, dotProduct.y)