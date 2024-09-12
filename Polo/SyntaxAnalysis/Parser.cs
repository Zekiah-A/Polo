using System.Collections.Immutable;
using Polo.Exceptions;
using Polo.Lexer;

namespace Polo.SyntaxAnalysis;

internal class Parser
{
    private int current;
    private readonly ImmutableArray<Token> source;

    public Parser(ImmutableArray<Token> source)
    {
        this.source = source;
    }

    public ImmutableArray<Statement> Run()
    {
        var statements = new List<Statement>();

        while (!IsAtEnd())
        {
            statements.Add(Statement());
        }

        return statements.ToImmutableArray();
    }

    private Statement Statement()
    {
        if (Match(TokenType.Indent))
        {
            return BlockStatement();
        }

        if (Match(TokenType.If))
        {
            return IfStatement();
        }

        if (Match(TokenType.While))
        {
            return WhileStatement();
        }
        
        if (Match(TokenType.Debug))
        {
            return DebugStatement();
        }

        if (Match(TokenType.Function))
        {
            return FunctionStatement();
        }

        if (Match(TokenType.Return))
        {
            return ReturnStatement();
        }

        if (Match(TokenType.Type))
        {
            return TypeStatement();
        }
        
        if (Match(TokenType.Let))
        {
            return LetDeclaration();
        }
        
        return ExpressionStatement();
    }
    

    private Statement LetDeclaration()
    {
        var name = Consume(TokenType.Identifier, "Expected variable name");

        Consume(TokenType.Colon, "Expected colon after variable name");
        var typeName = Consume(TokenType.Identifier, "Expected variable type");

        // Because variables can be assigned without a value.
        Expression? initializer = null;
        if (Match(TokenType.Equal))
        {
            initializer = Expression();
        }

        return new Let((string) name.Value!, (string) typeName.Value!, initializer);
    }
    
    private Statement IfStatement()
    {
        var condition = Expression();
        var thenBranch = BlockStatement();
        
        Block? elseBranch = null;
        if (Match(TokenType.Else))
        {
            elseBranch = BlockStatement();
        }

        return new If(condition, thenBranch, elseBranch);
    }

    private Statement WhileStatement()
    {
        var condition = Expression();
        Consume(TokenType.Indent, "Expected 'indent' after while declaration");
        var body = BlockStatement();
        Consume(TokenType.UnIndent, "Expected 'unindent' after while body");

        return new While(condition, body);
    }

    private Block BlockStatement()
    {
        var statements = new List<Statement>();
        while (!CheckType(TokenType.UnIndent))
        {
            statements.Add(Statement());
        }
        Consume(TokenType.UnIndent, "Expected unindent after block definition");
        
        return new Block(statements);
    }

    private Statement FunctionStatement()
    {
        // TODO: Make indent and unindent a real token type, use in place of brace blocking
        var name = Consume(TokenType.Identifier, "Expected method name");
        Consume(TokenType.LeftParen, "Expected function opening parenthesis");
        // Actually needs to be declaration/assign list
        var @params = new List<Token>();
        while (!CheckType(TokenType.RightParen))
        {
            if (IsAtEnd())
            {
                throw Error(source[current],"Unexpected 'end of file' in function Parameters");
            }
            
            @params.Add(Advance());
        }
        Consume(TokenType.RightParen, "Expected ')' after function Parameters");
        
        Consume(TokenType.Colon, "Expected ':' before function return type");
        var returnType = Consume(TokenType.Identifier, "Expected function return type");

        var body = BlockStatement();
        
        return new Function(name, @params, body, (string) returnType.Value!);
    }

    private Statement ReturnStatement()
    {
        Expression? returnExpression = null;
        // TODO: Sus
        try
        {
            returnExpression = Expression();
        }
        catch (ParsingErrorException e) { }

        return new Return(returnExpression);
    }

    private Statement TypeStatement()
    {
        //  if (Match(TokenType.Def))
        // {
        //     return DefDeclaration();
        // }
        
        throw new NotImplementedException();
    }

    private Statement DefDeclaration()
    {
        throw new NotImplementedException();
    }

    private Statement DebugStatement()
    {
        Consume(TokenType.LeftParen, "Expected opening '(' after debug invocation");
        var parameters = CallParameters();
        Consume(TokenType.RightParen, "Expected closing ')' after debug invocation");
        return new Debug(parameters);
    }

    private Statement ExpressionStatement()
    {
        var expression = Expression();
        return new StatementExpression(expression);
    }
    
    private Expression Expression()
    {
        return Assignment();
    }

    private Expression Assignment()
    {  
        var expression = Or();

        if (Match(TokenType.Equal))
        {
            var equals = Previous();
            var value = Assignment();

            if (expression is Variable variable)
            {
                var name = variable.Name;
                return new Assign(name, value);
            }

            throw Error(equals, "Invalid assignment target");
        }

        return expression;
    }

    private Expression Or()
    {
        var expression = And();

        while (Match(TokenType.Or))
        {
            var @operator = Previous();
            var right = And();
            expression = new Logical(expression, @operator, right);
        }

        return expression;
    }

    private Expression And()
    {
        var expression = Equality();

        while (Match(TokenType.And))
        {
            var @operator = Previous();
            var right = Equality();
            expression = new Logical(expression, @operator, right);
        }

        return expression;
    }


    private Expression Equality()
    {
        var expression = Comparison();

        while (Match(TokenType.BangEqual, TokenType.EqualEqual))
        {
            var @operator = Previous();
            var right = Comparison();
            expression = new Binary(expression, @operator, right);
        }

        return expression;
    }

    private Expression Comparison()
    {
        var expression = Term();

        while (Match(TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual))
        {
            var @operator = Previous();
            var right = Term();
            expression = new Binary(expression, @operator, right);
        }

        return expression;
    }

    private Expression Term()
    {
        var expression = Factor();

        while (Match(TokenType.Minus, TokenType.Plus))
        {
            var @operator = Previous();
            var right = Factor();
            expression = new Binary(expression, @operator, right);
        }

        return expression;
    }

    private Expression Factor()
    {
        var expression = Unary();

        while (Match(TokenType.Slash, TokenType.Star, TokenType.Modulo))
        {
            var @operator = Previous();
            var right = Unary();
            expression = new Binary(expression, @operator, right);
        }

        return expression;
    }

    private Expression Unary()
    {
        var expression = FunctionCall();
        
        if (Match(TokenType.Bang, TokenType.Minus))
        {
            var @operator = Previous();
            var right = Unary();
            return new Unary(@operator, right);
        }

        return expression;
    }

    private Expression FunctionCall()
    {
        var expression = Primary();
        
        if (Match(TokenType.Identifier) && Match(TokenType.LeftParen))
        {
            var identifier = source[current - 2];
            Console.WriteLine(identifier);
            var parameters = CallParameters();
            Consume(TokenType.RightParen, "Expected closing ')' after method invocation");
            return new FunctionCall(identifier, parameters);
        }

        return expression;
    }
    
    private Expression Primary()
    {
        if (Match(TokenType.False))
        {
            return new Literal("false", LiteralType.Binary);
        }
        
        if (Match(TokenType.True))
        {
            return new Literal("true", LiteralType.Binary);
        }
        
        if (Match(TokenType.NullRef))
        {
            return new Literal("null", LiteralType.Null);
        }
        
        if (Match(TokenType.Number))
        {
            var number = (string)Previous().Value!;
            return number.Contains('.') ?
                new Literal(number, LiteralType.Decimal) : new Literal(number, LiteralType.Integer);
        }
        
        if (Match(TokenType.Character))
        {
            return new Literal((string) Previous().Value!, LiteralType.Char);
        }
        
        if (Match(TokenType.String))
        {
            return new Literal((string) Previous().Value!, LiteralType.String);
        }
        
        if (Match(TokenType.Identifier))
        {
            return new Variable((string) Previous().Value!);
        }
        
        if (Match(TokenType.LeftParen))
        {
            var expression = Expression();
            Consume(TokenType.RightParen, "Expected ')' after Expression");
            return new Grouping(expression);
        }

        for (var i = current; i >= 0; i--)
        {
            Console.WriteLine(source[i]);
        }
        throw Error(Peek(), "Expected Expression");
    }

    private List<Expression> CallParameters()
    {
        var parameters = new List<Expression>();
        while (true)
        {
            if (Peek().Type == TokenType.RightParen)
            {
                break;
            }
            
            parameters.Add(Expression());
            if (Peek().Type == TokenType.Comma)
            {
                Advance();
            }
        }
        return parameters;
    }

    private ParsingErrorException Error(Token token, string message)
        => new ParsingErrorException($"{message} (Line: {token.Line}) (Token Type: {token.Type})");

    private bool CheckType(TokenType type)
        => !IsAtEnd() && Peek().Type == type;
    
    private bool Match(params TokenType[] types)
    {
        foreach (var type in types)
        {
            if (CheckType(type))
            {
                Advance();
                return true;
            }
        }
        return false;
    }

    private bool IsAtEnd()
        => Peek().Type == TokenType.EndOfFile;

    private Token Consume(TokenType type, string message)
        => CheckType(type) ? Advance() : throw Error(Peek(), message);

    private Token Previous()
        => source[current - 1];

    private Token Advance()
        => IsAtEnd() ? source.Last() : source[current++];

    private Token Peek()
        => source[current];
}
