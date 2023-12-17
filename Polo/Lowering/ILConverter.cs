using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Polo.Lexer;
using Polo.Runtime;
using Polo.SyntaxAnalysis;

namespace Polo.Lowering;

internal unsafe class ILConverter : IExpressionVisitor<object?, object?>, IStatementVisitor<object?>
{
    private byte* data;
    private long dataOffset;
    private readonly int dataSize = 1024;
    
    public ILConverter(ImmutableArray<Statement> statements)
    {
    }
    
    private object? Evaluate(Expression expression)
    {
        return expression.Accept(this);
    }

    public object? VisitBinaryExpression(Binary binary)
    {
        throw new NotImplementedException();
    }

    public object? VisitGroupingExpression(Grouping grouping)
    {
        throw new NotImplementedException();
    }
    
    public object? VisitLiteralExpression(Literal literal)
    {
        throw new NotImplementedException();
    }

    public object? VisitUnaryExpression(Unary unary)
    {
        throw new NotImplementedException();
    }

    public object? VisitVariableExpression(Variable variable)
    {
        throw new NotImplementedException();
    }

    public object? VisitAssignExpression(Assign assign)
    {
        throw new NotImplementedException();
    }

    public object? VisitLogicalExpression(Logical logical)
    {
        throw new NotImplementedException();
    }

    public object? VisitFunctionCallExpression(FunctionCall functionCall)
    {
        throw new NotImplementedException();
    }

    public object? VisitBlockStatement(Block block)
    {
        throw new NotImplementedException();
    }

    public object? VisitStatementExpression(StatementExpression statementExpression)
    {
        throw new NotImplementedException();
    }

    public object? VisitIfStatement(If @if)
    {
        throw new NotImplementedException();
    }

    public object? VisitLetStatement(Let let)
    {
        return null;
    }

    public object? VisitDefStatement(Def def)
    {
        throw new NotImplementedException();
    }

    public object? VisitWhileStatement(While @while)
    {
        throw new NotImplementedException();
    }

    public object? VisitFunctionStatement(Function function)
    {
        throw new NotImplementedException();
    }

    public object? VisitDebugStatement(Debug debug)
    {
        throw new NotImplementedException();
    }

    public object? VisitReturnStatement(Return @return)
    {
        throw new NotImplementedException();
    }
}