using System.Collections.Generic;
using System.Collections.Immutable;
using Polo.Exceptions;

namespace Polo.Lexer;

internal class Scanner
{
    private int lastIndent = 0;
    private int indent = 0;
    private int line = 1;
    private int start;
    private int current;

    private readonly string source;
    // All detected tokens in current file
    private readonly List<Token> tokens;
    // All detected tokens on current line
    private readonly List<Token> lineTokens;

    public Scanner(string source)
    {
        this.source = source;
        tokens = new List<Token>();
        lineTokens = new List<Token>();
    }

    public ImmutableArray<Token> Run()
    {
        while (!IsAtEnd())
        {
            start = current;
            Scan();
        }

        AddToken(TokenType.EndOfFile);
        return tokens.ToImmutableArray();
    }

    private bool Match(char expected)
    {
        if (Peek() == expected)
        {
            current++;
            return true;
        }

        return false;
    }

    private bool IsAtEnd()
    {
        return current >= source.Length;
    }

    private char Advance()
    {
        return IsAtEnd() ? '\0' : source[current++];
    }

    private char Peek(int distance = 0)
    {
        return IsAtEnd() ? '\0' : source[current + distance];
    }

    private void NextLine()
    {
        line++;
        lastIndent = indent;
        var deltaIndent = indent - lastIndent;
        switch (deltaIndent)
        {
            case 1:
            {
                AddToken(TokenType.Indent);
                break;
            }
            case < 0:
            {
                while (deltaIndent != 0)
                {
                    AddToken(TokenType.UnIndent);
                    deltaIndent++;
                }

                break;
            }
            case > 1:
            {
                Error($"Unexpected increase in indentation of {deltaIndent}");
                break;
            }
        }

        indent = 0;
        lineTokens.Clear();
    }
    
    private void Scan()
    {
        var character = Advance();
        switch (character)
        {
            case '\n':
            {
                NextLine();
                break;
            }
            case ' ':
            {
                // If we (^) are at end of indents section, such as "    codecode" "   ^codecode" and the next token
                // is not an indent but there is a space, then they have used a space to indent
                var withinIndents = tokens.Count > 0 && !lineTokens.Any(token => token.Type is not (TokenType.Indent or TokenType.UnIndent));
                if (withinIndents /*&& tokens[current].Type is not (TokenType.Indent or TokenType.UnIndent)*/)
                {
                    Warn("Character ' ' has no semantic meaning and will be ignored when defining block scopes. Use 'tab' character instead");
                }
                break;
            }
            case '\r': break;
            case '\t':
                var beyondIndents = lineTokens.Any(token => token.Type is not (TokenType.Indent or TokenType.UnIndent));
                if (!beyondIndents)
                {
                    indent++;
                }
                break;
            case '!':
                AddToken(Match('=') ? TokenType.BangEqual : TokenType.Bang);
                break;
            case '=':
                AddToken(Match('=') ? TokenType.EqualEqual : TokenType.Equal);
                break;
            case '<':
                AddToken(Match('=') ? TokenType.LessEqual : TokenType.Less);
                break;
            case '>':
                AddToken(Match('=') ? TokenType.GreaterEqual : TokenType.Greater);
                break;
            case '%':
                AddToken(TokenType.Modulo); 
                break;
            case '&':
                AddToken(Match('&') ? TokenType.And : TokenType.BitwiseAnd);
                break;
            case '|':
                AddToken(Match('|') ? TokenType.Or : TokenType.BitwiseOr);
                break;
            case >= '0' and <= '9':
            {
                while (char.IsDigit(Peek()))
                {
                    Advance();
                }

                if (Peek() == '.' && char.IsDigit(Peek(1)))
                {
                    Advance();

                    while (char.IsDigit(Peek()))
                    {
                        Advance();
                    }
                }

                AddToken(TokenType.Number, double.Parse(source[start..current]));
                break;
            }
            case >= 'a' and <= 'z' or '_':
            {
                while (char.IsDigit(Peek()) || char.IsLetter(Peek()) || Match('_'))
                {
                    Advance();
                }

                var identifier = source[start..current];

                AddToken(
                    ReservedIdentifiers.Keywords.TryGetValue(identifier, out var identifierType)
                        ? identifierType
                        : TokenType.Identifier, identifier);
                break;
            }
            case '"':
            {
                while (Peek() != '"' && !IsAtEnd())
                {
                    if (Peek() == '\n')
                    {
                        line++;
                    }

                    Advance();
                }

                if (IsAtEnd())
                {
                    Error("Unterminated string");
                }

                // The closing quote.
                Advance();

                var content = source[(start + 1)..(current - 1)];
                AddToken(TokenType.String, content);
                break;
            }
            case '\'':
            {
                var strStart = current;
                char c;
                if (Match('\\'))
                {
                    c = Advance();
                }
                var @char = Advance();
                
                while (Peek() != '"' && !IsAtEnd())
                {
                    if (Peek() == '\n')
                    {
                        line++;
                    }

                    Advance();
                }
                
                if (IsAtEnd())
                {
                    Error("Unterminated string");
                }
                break;
            }
            case '#':
            {
                // Multiline ## comment
                if (Match('#'))
                {
                    while (true)
                    {
                        if (Peek() == '\n')
                        {
                            line++;
                        }
                        else if (Peek() == '#' && Peek(1) == '#')
                        {
                            Advance();
                            Advance();
                            break;
                        }

                        if (IsAtEnd())
                        {
                            Error("Unexpected 'end of file' within multi-line comment. Did you forget to close it?");
                            break;
                        }
                        
                        Advance();
                    }
                }
                // Single line # comment
                else
                {
                    while (Peek() != '\n' && !IsAtEnd())
                    {
                        Advance();
                    }
                }
                break;
            }
            case '/':
            {
                AddToken(TokenType.Slash);
                break;
            }
            default:
            {
                if (ReservedIdentifiers.SingleCharacters.TryGetValue(character, out var characterType))
                {
                    AddToken(characterType, character);
                }

                else
                {
                    Error($"Unknown character '{character}'");

                }
                break;
            }
        }
    }

    private ScanningErrorException Error(string message)
    {
        throw new ScanningErrorException(message);
    }

    private void Warn(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    private void AddToken(TokenType type, object? value = null)
    {
        var token = new Token(type, value, line);
        lineTokens.Add(token);
        tokens.Add(token);
    }
}