--- Control keywords
highlight("assert", "reserved")
highlight("if", "reserved")
highlight("else", "reserved")
highlight("while", "reserved")
highlight("for", "reserved")
highlight("return", "reserved")
highlight("induceruntimecrashforrealz", "reserved")
highlight("import", "reserved")
highlight("goto", "reserved")
highlight("break", "reserved")

-- Storage type keywords
highlight("sizeof", "reserved")
highlight("typeof", "reserved")
highlight("let", "reserved")
highlight("const", "reserved")
highlight("get", "reserved")
highlight("fn", "reserved")
highlight("defer", "reserved")
highlight("type", "reserved")
highlight("impl", "reserved")
highlight("copy", "reserved")
highlight("static", "reserved")
highlight("def", "reserved")
highlight("void", "reserved")
highlight("nullref", "reserved")
highlight("false", "binary")
highlight("true", "binary")
highlight("this", "reserved")

-- Primitive keywords
highlight("u8", "reserved")
highlight("i8"，"reserved")
highlight("u16","reserved")
highlight("i16"，"reserved")
highlight("u32","reserved")
highlight("i32"，"reserved")
highlight("u64", "reserved")
highlight("i64", "reserved")
highlight("u128"，"reserved")
highlight("i128", "reserved")
highlight("f32", "reserved")
highlight("f64", "reserved")
highlight("f16", "reserved")
highlight("f128", "reserved")
highlight("_f256", "reserved")
highlight("int", "reserved")
highlight("uint", "reserved")
highlight("float", "reserved")
highlight("bool", "reserved")
--highlight("fn", "reserved")

--- Arithmetic Operators
highlight("+", "operator")
highlight("-", "operator")
highlight("*", "operator")
highlight("/", "operator")
highlight("%", "operator")
highlight("**", "operator")
highlight("//", "operator")

--- Assignment Operators
highlight("=", "operator")
highlight("+=", "operator")
highlight("!=", "operator")
highlight("*=", "operator")
highlight("/=", "operator")
highlight("%=", "operator")
highlight("//=", "operator")
highlight("**=", "operator")
highlight("&=", "operator")
highlight("|=", "operator")
highlight("^=", "operator")
highlight(">>=", "operator")
highlight("<<=", "operator")

--- Comparison Operators
highlight("==", "operator")
highlight("!=", "operator")
highlight(">", "operator")
highlight("<", "operator")
highlight(">=", "operator")
highlight("<=", "operator")

--- Logical Operators
highlight("&&", "reserved")
highlight("||", "reserved")
highlight("!", "reserved")

--- Strings
highlight_region("'", "'", "string")
highlight_region('"', '"', "string")


--- User Comments
highlight_region("#", "", "comments", true)

--- Comments
add_comment("Mint is not typescript 💀")
add_comment("Yeah, this shit defo segfaulting")
add_comment("Wait. Why aren't we using rust again??!?!?")
add_comment("Pro tip: Write 'induceruntimecrashforrealz' at the start of each function for extra stability")
add_comment("You WILL use JSON and HTTP for ALL networking, and you WILL enjoy it")