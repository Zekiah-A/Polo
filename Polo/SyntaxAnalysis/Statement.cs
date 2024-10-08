﻿using Polo.Lexer;

namespace Polo.SyntaxAnalysis;

internal abstract record Statement()
{
    public abstract T? Accept<T>(IStatementVisitor<T?> visitor);
}

internal interface IStatementVisitor<out T>
{
    T? VisitBlockStatement(Block block);
    T? VisitStatementExpression(StatementExpression statementExpression);
    T? VisitIfStatement(If @if);
    T? VisitLetStatement(Let let);
    T? VisitDefStatement(Def def);
    T? VisitWhileStatement(While @while);
    T? VisitFunctionStatement(Function function);
    T? VisitDebugStatement(Debug debug);
    T? VisitReturnStatement(Return @return);
    T? VisitTypeStatement(Type type);
}

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

internal record If(Expression Condition, Block ThenBranch, Block? ElseBranch) : Statement
{
    public override T Accept<T>(IStatementVisitor<T> visitor)
        => visitor.VisitIfStatement(this);
}

internal record Let(string Name, string TypeName, Expression? Initialiser) : Statement
{
    public override T Accept<T>(IStatementVisitor<T> visitor)
        => visitor.VisitLetStatement(this);
}

internal record Def(string Name, Expression? Initializer) : Statement
{
    public override T Accept<T>(IStatementVisitor<T> visitor)
        => visitor.VisitDefStatement(this);
}

internal record While(Expression Condition, Block Body) : Statement
{
    public override T Accept<T>(IStatementVisitor<T> visitor)
        => visitor.VisitWhileStatement(this);
}
// TODO: Token type for Parameters is cursed, use declaration type instead! 
internal record Function(Token Name, List<Token> Parameters, Block Body, string ReturnType) : Statement
{
    public override T Accept<T>(IStatementVisitor<T> visitor)
        => visitor.VisitFunctionStatement(this);
}

internal record Return(Expression? Expression) : Statement
{
    public override T Accept<T>(IStatementVisitor<T> visitor)
        => visitor.VisitReturnStatement(this);
}

internal record Type() : Statement
{
    public override T Accept<T>(IStatementVisitor<T> visitor)
        => visitor.VisitTypeStatement(this);
}

internal record Debug(List<Expression> Parameters) : Statement
{
    public override T Accept<T>(IStatementVisitor<T> visitor)
        => visitor.VisitDebugStatement(this);
}