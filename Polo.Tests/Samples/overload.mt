##
--- overload.mt ---
This file demonstrates the ability to overload operators, in order
to execute special methods to handle the usage of such operators on
certain types.
##
type Vector2
	def x:f32
	def y:f32
	
	# Special operator overload methods
	fn *(Vector2 other)
		return Vector2{x * other.x, y * other.y}
	
	fn +(Vector2 other)
		return Vector2{x + other.x, y + other.y}

	fn -(Vector2 other)
		return Vector2{x - other.x, y - other.y}

let vectorOne:Vector2 = Vector2{5, 5}
let vectorTwo:Vector2 = Vector2{10, 10}

# No primitive multiplication operator exists, so overload method
# will be used. This code can be thought of as vectorOne.*(vector2)
let dotProduct:Vector2 = vectorOne * vector2

debug(Str.format("Result: %f, %f", {dotProduct.x, dotProduct.y}))