﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Polo.Exceptions;
using Polo.Lexer;
using Polo.SyntaxAnalysis;

namespace Polo.Runtime;

internal class Interpreter : IExpressionVisitor<object>, IStatementVisitor<object>
{
    private MintEnvironment environment;
    
    private readonly HashSet<Type> numericTypes = new HashSet<Type>
    {
        typeof(byte), typeof(sbyte), typeof(short), typeof(ushort),
        typeof(int), typeof(uint), typeof(long), typeof(ulong),
        typeof(decimal), typeof(double), typeof(float)
    };

    public Interpreter()
    {
        environment = new MintEnvironment();
    }

    public void Run(ImmutableArray<Statement> statements)
    {
        foreach (var statement in statements)
            Execute(statement);
    }

    public object VisitBinaryExpression(Binary binary)
    {
        var left = Evaluate(binary.Left);
        var right = Evaluate(binary.Right);

        switch (binary.Operator.Type)
        {
            case TokenType.Plus:
            {
                return left switch
                {
                    double d when right is double d1 => d + d1,
                    string s when right is string s1 => s + s1,
                    _ => throw new RuntimeErrorException("Operands must be two numbers or two strings.")
                };
            }
            case TokenType.Minus:
            {
                CheckNumberOperands(binary.Operator, left, right);
                return (double)left - (double)right;
            }
            case TokenType.Slash:
            {
                CheckNumberOperands(binary.Operator, left, right);
                return (double)left / (double)right;
            }
            case TokenType.Star:
            {
                CheckNumberOperands(binary.Operator, left, right);
                return (double)left * (double)right;
            }
            case TokenType.EqualEqual:
            {
                return IsEqual(left, right);
            }
            case TokenType.BangEqual:
            {
                return !IsEqual(left, right);
            }
            case TokenType.Greater:
            {
                return left switch
                {
                    double d when right is double d1 => d > d1,
                    string s when right is string s1 => s.Length > s1.Length,
                    _ => throw new RuntimeErrorException("Operands must be two numbers or two strings.")
                };
            }
            case TokenType.GreaterEqual:
            {
                return left switch
                {
                    double d when right is double d1 => d >= d1,
                    string s when right is string s1 => s.Length >= s1.Length,
                    _ => throw new RuntimeErrorException("Operands must be two numbers or two strings.")
                };
            }
            case TokenType.Less:
            {
                return left switch
                {
                    double d when right is double d1 => d < d1,
                    string s when right is string s1 => s.Length < s1.Length,
                    _ => throw new RuntimeErrorException("Operands must be two numbers or two strings.")
                };
            }
            case TokenType.LessEqual:
            {
                return left switch
                {
                    double d when right is double d1 => d <= d1,
                    string s when right is string s1 => s.Length <= s1.Length,
                    _ => throw new RuntimeErrorException("Operands must be two numbers or two strings.")
                };
            }
            case TokenType.Modulo:
                return left switch
                {
                    double d when right is double d1 => d % d1,
                    string s when right is string s1 => int.Parse(s) % int.Parse(s1),
                    _ => throw new RuntimeErrorException("Operands must be two numbers or two strings.")
                };
        }

        throw new RuntimeErrorException("Unknown operator.");
    }

    public object VisitGroupingExpression(Grouping grouping)
        => Evaluate(grouping.Expression);

    public object? VisitLiteralExpression(Literal literal)
        => literal.Value;

    public object VisitUnaryExpression(Unary unary)
    {
        var right = Evaluate(unary.Right);
        switch (unary.Operator.Type)
        {
            case TokenType.Bang:
                return !IsTruthy(right);

            case TokenType.Minus:
                CheckNumberOperands(unary.Operator, right);
                return -(double)right;
        }

        throw new RuntimeErrorException("Unknown operator.");
    }

    public object? VisitVariableExpression(Variable variable)
        => environment.Get(variable.Name);

    public object VisitAssignExpression(Assign assign)
    {
        var value = Evaluate(assign.Value);

        environment.Assign(assign.Name, value);
        return value;
    }

    public object VisitLogicalExpression(Logical logical)
    {
        var left = Evaluate(logical.Left);

        /*if (logical.Operator.Type == TokenType.Or)
        {
            if (IsTruthy(left))
                return left;
        }
        else
        {
            if (!IsTruthy(left))
                return left;
        }*/
        return left;

        return Evaluate(logical.Right);
    }

    private object? Evaluate(Expression expression)
        => expression.Accept(this);

    public object VisitBlockStatement(Block block)
    {
        ExecuteBlock(block.Statements, new MintEnvironment(environment));
        return block;
    }

    public object VisitIfStatement(If @if)
    {
        if (IsTruthy(Evaluate(@if.Condition)))
            Execute(@if.ThenBranch);

        else if (@if.ElseBranch != null)
            Execute(@if.ElseBranch);

        return @if;
    }

    public object VisitDebugStatement(Debug debug)
    {
        var value = Evaluate(debug.Expression);

        if (value == null)
        {
            Console.WriteLine("null");
        }
        else if (numericTypes.Contains(value.GetType()))
        {
            Console.WriteLine(value.ToString());
        }
        else
        {
            Console.WriteLine(value);
        }

        return value;
    }
    
    public object VisitStatementExpression(StatementExpression statementExpression)
    {
        Evaluate(statementExpression.Expression);
        return statementExpression;
    }

    public object VisitVarStatement(Let let)
    {
        object? value = null;
        if (let.Initializer != null)
            value = Evaluate(let.Initializer);

        environment.Define(let.Name, value);
        return let;
    }

    public object VisitDefStatement(Def def)
    {
        object? value = null;
        if (def.Initializer != null)
            value = Evaluate(def.Initializer);

        environment.Define(def.Name, value);
        return def;
    }

    public object VisitWhileStatement(While @while)
    {
        while (IsTruthy(Evaluate(@while.Condition)))
            Execute(@while.Body);

        return @while;
    }

    public object VisitFunctionStatement(Function function)
    {
        throw new NotImplementedException();
    }
    
    private void ExecuteBlock(List<Statement> statements, MintEnvironment environment)
    {
        var previous = this.environment;
        try
        {
            this.environment = environment;

            foreach (var statement in statements)
                Execute(statement);
        }
        finally
        {
            this.environment = previous;
        }
    }

    private void Execute(Statement statement)
    {
        // TODO: Only analyse for now
        statement.Accept(this);
    }

    private static bool IsTruthy(object? value)
    {
        if (value == null)
            return false;

        return value is not bool b || b;
    }

    private static bool IsEqual(object? left, object? right)
    {
        return left switch
        {
            null when right == null => true,
            null => false,
            _ => left.Equals(right)
        };
    }

    private void CheckNumberOperands(Token @operator, params object[] operands)
    {
        if (operands.Any(item => item is not double))
        {
            throw new RuntimeErrorException($"Operands must be a number ({@operator.Type})");
        }
    }
}