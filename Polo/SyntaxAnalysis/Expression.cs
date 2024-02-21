using System.Collections.Immutable;
using Polo.Lexer;

namespace Polo.SyntaxAnalysis;

internal abstract record Expression()
{
    public abstract T? Accept<T, TContext>(IExpressionVisitor<T, TContext> visitor);
}

internal interface IExpressionVisitor<out T, TContext>
{
    T? VisitBinaryExpression(Binary binary);
    T? VisitGroupingExpression(Grouping grouping);
    T? VisitLiteralExpression(Literal literal);
    T? VisitUnaryExpression(Unary unary);
    T? VisitVariableExpression(Variable variable);
    T? VisitAssignExpression(Assign assign);
    T? VisitLogicalExpression(Logical logical);
    T? VisitFunctionCallExpression(FunctionCall functionCall);
}

internal record Binary(Expression Left, Token Operator, Expression Right) : Expression
{
    public override T Accept<T, TContext>(IExpressionVisitor<T, TContext> visitor)
        => visitor.VisitBinaryExpression(this);
}

internal record Grouping(Expression Expression) : Expression
{
    public override T Accept<T, TContext>(IExpressionVisitor<T, TContext> visitor)
        => visitor.VisitGroupingExpression(this);
}

internal record Literal(object? Value, LiteralType Type) : Expression
{
    public override T Accept<T, TContext>(IExpressionVisitor<T, TContext> visitor)
        => visitor.VisitLiteralExpression(this);
}

internal record Unary(Token Operator, Expression Right) : Expression
{
    public override T Accept<T, TContext>(IExpressionVisitor<T, TContext> visitor)
        => visitor.VisitUnaryExpression(this);
}

internal record Variable(string Name) : Expression
{
    public override T Accept<T, TContext>(IExpressionVisitor<T, TContext> visitor)
        => visitor.VisitVariableExpression(this);
}

internal record FunctionCall(Token Name, IReadOnlyList<Expression> Parameters) : Expression
{
    public override T Accept<T, TContext>(IExpressionVisitor<T, TContext> visitor)
        => visitor.VisitFunctionCallExpression(this);

}

internal record Assign(string Name, Expression Value) : Expression
{
    public override T Accept<T, TContext>(IExpressionVisitor<T, TContext> visitor)
        => visitor.VisitAssignExpression(this);
}

internal record Logical(Expression Left, Token Operator, Expression Right) : Expression
{
    public override T Accept<T, TContext>(IExpressionVisitor<T, TContext> visitor)
        => visitor.VisitLogicalExpression(this);
}
