using System.Collections.Generic;
using Polo.Lexer;

namespace Polo.SyntaxAnalysis;

internal abstract record Statement()
{
    public abstract T Accept<T>(IStatementVisitor<T> visitor);
}

internal interface IStatementVisitor<T>
{
    T VisitBlockStatement(Block block);
    T VisitStatementExpression(StatementExpression statementExpression);
    T VisitIfStatement(If @if);
    T VisitVarStatement(Let let);
    T VisitDefStatement(Def def);
    T VisitWhileStatement(While @while);
    T VisitFunctionStatement(Function function);
    T VisitDebugStatement(Debug debug);
}

// TODO: Implement using tab
internal record Block(List<Statement> Statements) : Statement
{
    public override T Accept<T>(IStatementVisitor<T> visitor)
        => visitor.VisitBlockStatement(this);
}

internal record StatementExpression(Expression Expression) : Statement
{
    public override T Accept<T>(IStatementVisitor<T> visitor)
        => visitor.VisitStatementExpression(this);
}

internal record If(Expression Condition, Statement ThenBranch, Statement? ElseBranch) : Statement
{
    public override T Accept<T>(IStatementVisitor<T> visitor)
        => visitor.VisitIfStatement(this);
}

internal record Let(Token Name, Token type, Expression? Initializer) : Statement
{
    public override T Accept<T>(IStatementVisitor<T> visitor)
        => visitor.VisitVarStatement(this);
}

internal record Def(Token Name, Expression? Initializer) : Statement
{
    public override T Accept<T>(IStatementVisitor<T> visitor)
        => visitor.VisitDefStatement(this);
}

internal record While(Expression Condition, Statement Body) : Statement
{
    public override T Accept<T>(IStatementVisitor<T> visitor)
        => visitor.VisitWhileStatement(this);
}
// TODO: Token type for parameters is cursed, use declaration type instead! 
internal record Function(Token Name, List<Token> Parameters, List<Statement> Body, Token returnType) : Statement
{
    public override T Accept<T>(IStatementVisitor<T> visitor)
        => visitor.VisitFunctionStatement(this);
}

internal record Debug(Expression Expression) : Statement
{
    public override T Accept<T>(IStatementVisitor<T> visitor)
        => visitor.VisitDebugStatement(this);
}