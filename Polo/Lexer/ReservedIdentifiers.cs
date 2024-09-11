using System.Collections.Generic;

namespace Polo.Lexer;

internal static class ReservedIdentifiers
{
    public static readonly IReadOnlyDictionary<string, TokenType> Keywords =
        new Dictionary<string, TokenType>
        {
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
            { "fn", TokenType.Function },
            { "assert", TokenType.Assert },
            { "debug", TokenType.Debug },
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