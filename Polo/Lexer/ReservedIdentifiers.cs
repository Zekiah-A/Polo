using System.Collections.Generic;

namespace Polo.Lexer;

internal static class ReservedIdentifiers
{
    public static readonly IReadOnlyDictionary<string, TokenType> Keywords =
        new Dictionary<string, TokenType>
        {
            // Compiler recognised primitives
            { "__u8", TokenType.U8 },
            { "__i8", TokenType.I8 },
            { "__u16", TokenType.U16 },
            { "__i16", TokenType.I16 },
            { "__u32", TokenType.U32 },
            { "__i32", TokenType.I32 },
            { "__u64", TokenType.U64 },
            { "__i64", TokenType.I64 },
            { "__u128", TokenType.U128 },
            { "__i128", TokenType.I128 },
            { "__f16", TokenType.F16 },
            { "__f32", TokenType.F32 },
            { "__f64", TokenType.F64 },
            { "__f128", TokenType.F128 },
            { "__f256", TokenType.F256 },
            { "int", TokenType.Int },
            { "uint", TokenType.UInt },
            { "float", TokenType.Float },
            { "bool", TokenType.Bool },
            { "void", TokenType.Void },
            { "char", TokenType.Char },
            { "fn", TokenType.Function },
            // Storage
            { "sizeof", TokenType.SizeOf },
            { "typeof", TokenType.TypeOf },
            { "let", TokenType.Let },
            { "const", TokenType.Const },
            { "get", TokenType.Get },
            { "defer", TokenType.Defer },
            { "type", TokenType.Type },
            { "impl", TokenType.Impl },
            { "copy", TokenType.Copy },
            { "static", TokenType.Static },
            { "def", TokenType.Def },
            { "nullref", TokenType.NullRef },
            // Control
            { "assert", TokenType.Assert },
            { "if", TokenType.If },
            { "else", TokenType.Else },
            { "for", TokenType.For },
            { "return", TokenType.Return },
            { "induceruntimecrashforrealz", TokenType.Crash },
            { "goto", TokenType.Goto },
            { "break", TokenType.Break },
            // Preprocess
            { "@c", TokenType.CompileTime },
            { "@import", TokenType.Import },
            { "@include", TokenType.Include },
            { "@line", TokenType.Line },
            { "@file", TokenType.File },
            { "@date", TokenType.Date },
            { "@time", TokenType.Time },
            { "@counter", TokenType.Counter }
        };

    public static readonly IReadOnlyDictionary<char, TokenType> SingleCharacters =
        new Dictionary<char, TokenType>
        {
            { '(', TokenType.LeftParen },
            { ')', TokenType.RightParen },
            { '{', TokenType.LeftBrace },
            { '}', TokenType.RightBrace },
            { ',', TokenType.Comma },
            { '.', TokenType.Dot },
            { '-', TokenType.Minus },
            { '+', TokenType.Plus },
            { ';', TokenType.Semicolon },
            { '*', TokenType.Star },
            { '%', TokenType.Modulo },
            { '=', TokenType.Equal },
            { ':', TokenType.Colon }
        };
}