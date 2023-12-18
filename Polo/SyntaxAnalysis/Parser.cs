using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Polo.Exceptions;
using Polo.Lexer;

namespace Polo.SyntaxAnalysis;

internal class Parser
{
    private int current;
    private readonly ImmutableArray<Token> source;
    private readonly Dictionary<string, MintType> foundTypes;

    public Parser(ImmutableArray<Token> source)
    {
        this.source = source;
    }

    public ImmutableArray<Statement> Run()
    {
        var statements = new List<Statement>();

        while (!IsAtEnd())
        {
            var declaration = Declaration();
            if (declaration is not null)
            {
                statements.Add(declaration);
            }
        }

        return statements.ToImmutableArray();
    }

    private Statement Declaration()
    {
        if (Match(TokenType.Let))
        {
            return LetDeclaration();
        }
        if (Match(TokenType.Def))
        {
            return DefDeclaration();
        }

        return Statement();
    }

    private Statement DefDeclaration()
    {
        var name = Consume(TokenType.Identifier, "Expected variable name");

        Expression? initializer = null;

        if (Match(TokenType.Equal))
            initializer = Expression();
        
        // Because variables can be assigned without a value.
        return new Def(name, initializer);
    }

    private void ValidateType(Token typeToken)
    {
    }

    private Statement LetDeclaration()
    {
        var name = Consume(TokenType.Identifier, "Expected variable name");
        Consume(TokenType.Colon, "Expected colon after variable name");
        var type = Advance();
        if (type.Value is not string typeName)
        {
            throw Error(type, "Expected type name after variable identifier");
        }
        Expression? initializer = null;

        if (Match(TokenType.Equal))
        {
            initializer = Expression();
        }

        // Because variables can be assigned without a value.
        return new Let(name, typeName, initializer);
    }

    private Statement Statement()
    {
        if (Match(TokenType.If))
        {
            return IfStatement();
        }

        if (Match(TokenType.Indent))
        {
            return new Block(BlockStatement());
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
        
        return ExpressionStatement();
    }
    
    private Statement IfStatement()
    {
        var condition = Expression();
        var thenBranch = Statement();
        Statement? elseBranch = null;

        if (Match(TokenType.Else))
            elseBranch = Statement();

        return new If(condition, thenBranch, elseBranch);
    }

    private Statement WhileStatement()
    {
        var condition = Expression();
        Consume(TokenType.Indent, "Expected 'indent' after while declaration");
        var body = Statement();
        Consume(TokenType.UnIndent, "Expected 'unindent' after while body");

        return new While(condition, body);
    }

    private List<Statement> BlockStatement()
    {
        var statements = new List<Statement>();
        while (!CheckType(TokenType.RightBrace) && !IsAtEnd())
        {
            statements.Add(Declaration());
        }

        Consume(TokenType.UnIndent, "Expected 'unindent' after the code block");
        return statements;
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
        
        var returnType = Advance();
        if (!IsTypeToken(returnType))
        {
            throw Error(returnType, "Expected function return type");
        }
        
        return new Function(name, @params, new List<Statement>(), returnType);
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
            return new Literal(false, LiteralType.Binary);
        }
        
        if (Match(TokenType.True))
        {
            return new Literal(true, LiteralType.Binary);
        }
        
        if (Match(TokenType.NullRef))
        {
            return new Literal(null, LiteralType.Null);
        }
        
        if (Match(TokenType.Number))
        {
            return new Literal(Previous().Value, LiteralType.Integer);
        }
        
        if (Match(TokenType.String))
        {
            return new Literal(Previous().Value, LiteralType.String);
        }
        
        if (Match(TokenType.Identifier))
        {
            return new Variable(Previous());
        }
        
        if (Match(TokenType.LeftParen))
        {
            var expression = Expression();
            Consume(TokenType.RightParen, "Expected ')' after Expression");
            return new Grouping(expression);
        }

        throw Error(Peek(), "Expected Expression");
    }

    private List<Expression> CallParameters()
    {
        var parameters = new List<Expression>();
        // TODO: Sus code
        try
        {
            while (true)
            {
                parameters.Add(Expression());
                Consume(TokenType.Comma, "Expected ',' between method call arguments");
            }
        }
        catch (ParsingErrorException)
        {
            
        }

        return parameters;
    }

    private ParsingErrorException Error(Token token, string message)
        => new ParsingErrorException($"{message} (Line: {token.Line}) (Token Type: {token.Type})");

    private bool CheckType(TokenType type)
        => !IsAtEnd() && Peek().Type == type;

    private static bool IsTypeToken(Token token)
    {
        return token.Type is TokenType.U8 or TokenType.I8 or TokenType.U16 or TokenType.I16 or TokenType.U32
            or TokenType.I32 or TokenType.U64 or TokenType.I64 or TokenType.U128 or TokenType.I128 or TokenType.F16
            or TokenType.F32 or TokenType.F64 or TokenType.F128 or TokenType.F256 or TokenType.Int or TokenType.UInt
            or TokenType.Float or TokenType.Bool or TokenType.Void or TokenType.Char or TokenType.Function;
    }

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