using System.Collections.Generic;
using System.Collections.Immutable;
using Polo.Exceptions;

namespace Polo.Lexer;

internal class Scanner
{
    private int lastIndent;
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
        CheckIndents();
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
        lineTokens.Clear();
        CheckIndents();
    }

    private void CheckIndents()
    {
        // Count indents when encountering start of new line
        if (lineTokens.Count != 0)
        {
            return;
        }

        // x
        //     x           | Indent
        //         x       | Indent
        // x               | UnIndent, UnIndent
        //         x       | Error - Invalid increase in indentation
        var indent = 0;
        while (Match('\t'))
        {
            indent++;
        }

        var deltaIndent = indent - lastIndent;
        // Indenting by more than 1 for a new block is unsupported
        if (deltaIndent > 1)
        {
            throw Error($"Unexpected increase in indentation of {deltaIndent}");
        }

        // Add new indent level
        if (deltaIndent == 1)
        {
            AddToken(TokenType.Indent);
        }

        // Subtract any indents from the previous line
        if (deltaIndent < 0)
        {
            for (var i = deltaIndent; i < 0; i++)
            {
                AddToken(TokenType.UnIndent);
            }
        }
        lastIndent = indent;
    }
    
    private void Scan()
    {
        var character = Advance();
        switch (character)
        {
            case '\r':
                break;
            case '\n':
            {
                NextLine();
                break;
            }
            case ' ':
            {
                // If we (^) are at end of indents before this line's statement, such as "    code" "   ^code"
                // and the next token is not an indent but there is a space, then they have used spaces to indent.
                if (!lineTokens.Any(token => token.Type is not TokenType.Indent))
                {
                    Warn("Character ' ' has no semantic meaning and will be ignored when defining block scopes. Use 'tab' character instead.");
                }
                break;
            }
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

                AddToken(TokenType.Number, source[start..current]);
                break;
            }
            case >= 'a' and <= 'z' or '_':
            {
                while (char.IsDigit(Peek()) || char.IsLetter(Peek()) || Match('_'))
                {
                    Advance();
                }

                var identifier = source[start..current];
                AddToken(ReservedIdentifiers.Keywords.GetValueOrDefault(identifier, TokenType.Identifier), identifier);
                break;
            }
            case '"':
            {
                var stringBuilder = new System.Text.StringBuilder();
                while (Peek() != '"' && !IsAtEnd())
                {
                    if (Peek() == '\n')
                    {
                        line++;
                    }

                    if (Peek() == '\\')
                    {
                        Advance(); // Skip the backslash
                        char escapedChar = Advance();
                        switch (escapedChar)
                        {
                            case 'n':
                                stringBuilder.Append('\n');
                                break;
                            case 't':
                                stringBuilder.Append('\t');
                                break;
                            case '"':
                                stringBuilder.Append('"');
                                break;
                            case '\\':
                                stringBuilder.Append('\\');
                                break;
                            default:
                                stringBuilder.Append('\\').Append(escapedChar); // Handle unknown escape sequences
                                break;
                        }
                    }
                    else
                    {
                        stringBuilder.Append(Advance());
                    }
                }

                if (IsAtEnd())
                {
                    throw Error("Unterminated string");
                }

                // Handle closing quote
                Advance();

                var content = stringBuilder.ToString();
                AddToken(TokenType.String, content);
                break;
            }
            case '\'':
            {
                var charBuilder = new System.Text.StringBuilder();
                if (Match('\\'))
                {
                    var escapedChar = Advance();
                    switch (escapedChar)
                    {
                        case 'n':
                            charBuilder.Append('\n');
                            break;
                        case 't':
                            charBuilder.Append('\t');
                            break;
                        case '\'':
                            charBuilder.Append('\'');
                            break;
                        case '\\':
                            charBuilder.Append('\\');
                            break;
                        default:
                            throw Error($"Unknown escape sequence \\{escapedChar}");
                    }
                }
                else
                {
                    charBuilder.Append(Advance());
                }

                if (charBuilder.Length != 1)
                {
                    throw Error("Invalid character literal");
                }

                if (Peek() != '\'' || IsAtEnd())
                {
                    throw Error("Unterminated character literal");
                }

                // Handle closing quote
                Advance();

                var content = charBuilder.ToString();
                AddToken(TokenType.Character, content);
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
                            throw Error("Unexpected 'end of file' within multi-line comment. Did you forget to close it?");
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
                    throw Error($"Unknown character '{character}'");
                }
                break;
            }
        }
    }

    private ScanningErrorException Error(string message)
    {
        return new ScanningErrorException($"{line}: {message}");
    }

    private void Warn(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("{0}: {1}", line, message);
        Console.ResetColor();
    }

    private void AddToken(TokenType type, object? value = null)
    {
        var token = new Token(type, value, line);
        lineTokens.Add(token);
        tokens.Add(token);
    }
}