# Polo - A compiler for a WIP lang

A preview Mint programming language analyser, interpreter and compiler, based on the Nephrite backend,
https://github.com/Zekiah-A/Nephrite.

```
type Vector2(T)
	def x:T
	def y:T

	fn *(Vector2 other)
		return Vector2{x * other.x, y * other.y}
	
let vectorOne:Vector2 = Vector2{5, 5}
let vectorTwo:Vector2 = Vector2{10, 10}

let dotProduct = vectorOne * vector2
debug(Str.format("Result: %f, %f", {dotProduct.x, dotProduct.y}))
```

## Getting started:
There are some sample files located within the [Polo.Tests/Samples](./Polo.Tests/Samples) directory. A
[convenience script](./Polo.Tests/run-compile-test.sh) has been created in order to assist with quickly building
and debugging these example projects.

## Details:
Also known as Glow, Mint or Ax, this prototype language aims be a simple, low-level and performant language,
taking influences from languages such as Zig and C with a modern twist. Currently, there is a work-in-progress
interpreter, and native compiler, using the QBE backend.

## License:
All assets within this project is licensed freely under the [GNU GPL-3.0](https://www.gnu.org/licenses/gpl-3.0.en.html)
unless stated otherwise. Used libraries and dependencies of this project may have their own licenses.