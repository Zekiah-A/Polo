using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Polo.SyntaxAnalysis;
using Type = Polo.SyntaxAnalysis.Type;

namespace Polo.TypeAnalysis;

internal class TypeAnalyser : IExpressionVisitor<object?, object?>, IStatementVisitor<object?>
{
    private ImmutableArray<Statement> statements;
    private readonly TargetArchitecture architecture;
    private readonly DefinedTypes definedTypes;
    
    public TypeAnalyser(ImmutableArray<Statement> statements, TargetArchitecture architecture)
    {
        this.statements = statements;
        this.architecture = architecture;
        definedTypes = new DefinedTypes();
    }
    
    internal DefinedTypes Run()
    {
        definedTypes.AddGlobal(new MintType("int", architecture.IntSize));
        definedTypes.AddGlobal(new MintType("uint", architecture.IntSize));
        definedTypes.AddGlobal(new MintType("float", architecture.FloatSize));
        definedTypes.AddGlobal(new MintType("char", architecture.CharSize));
        definedTypes.AddGlobal(new MintType("void", 0));
        
        return definedTypes;
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
        throw new NotImplementedException();
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

    public object? VisitTypeStatement(Type type)
    {
        throw new NotImplementedException();
    }
}
