namespace Polo.Lexer;

public enum TokenType
{
    // Single-character tokens.
    LeftParen, RightParen, LeftBrace, RightBrace, Colon,
    Comma, Dot, Minus, Plus, Semicolon, Slash, Star, Modulo,

    // One or two character tokens.
    Bang, BangEqual,
    Equal, EqualEqual,
    Greater, GreaterEqual,
    Less, LessEqual,
    Or, And, BitwiseAnd,
    BitwiseOr,
    
    // Storage
    SizeOf, TypeOf, Let, Const, Get, Defer, Type, Impl, Copy, Static, Def, NullRef, False, True, This,
    
    // Control
    Function, Assert, Debug, If, Else, While, For, Return, Crash, Goto, Break,
        
    // Preprocess
    CompileTime, Import, Include, Line, File, Date, Time, Counter,
    
    // Literals
    Identifier, Character, String, Number,

    Indent, UnIndent,
    EndOfFile
}