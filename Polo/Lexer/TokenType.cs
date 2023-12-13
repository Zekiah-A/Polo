namespace Polo.Lexer;

internal enum TokenType
{
    // Single-character tokens.
    LeftParen, RightParen, LeftBrace, RightBrace, Colon,
    Comma, Dot, Minus, Plus, Semicolon, Slash, Star, Modulo,

    // One or two character tokens.
    Bang, BangEqual,
    Equal, EqualEqual,
    Greater, GreaterEqual,
    Less, LessEqual,
    
    // Compiler recognised primitives
    U8, I8, U16, I16, U32, I32, U64, I64, U128, I128, F16, F32, F64, F128, F256,
    Int, // size_t,
    UInt, // usize_t,
    Float, // 32/64 bit depending on sys
    Bool, // u8/u1, 1 | 0
    Void,
    Char, // uint8_t
    Function,
    
    // Storage
    SizeOf, TypeOf, Let, Const, Get, Defer, Type, Impl, Copy, Static, Def, NullRef, False, True, This,
    
    // Control
    Assert, If, Else, While, For, Return, Crash, Goto, Break,
        
    // Preprocess
    CompileTime, Import, Include, Line, File, Date, Time, Counter,
    
    // Literals
    Identifier, String, Number,

    Indent, UnIndent,
    EndOfFile
}