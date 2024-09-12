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
        // System defined types
        var intType = new MintType("int", architecture.IntSize);
        definedTypes.AddGlobal(intType);
        var uintType = new MintType("uint", architecture.IntSize);
        definedTypes.AddGlobal(uintType);
        var floatType = new MintType("float", architecture.FloatSize);
        definedTypes.AddGlobal(floatType);
        var charType = new MintType("char", architecture.CharSize);
        definedTypes.AddGlobal(charType);
        var voidType = new MintType("void", 0);
        definedTypes.AddGlobal(voidType);
        
        // Mint compiler primitive types
        definedTypes.AddGlobal(new MintType("u0", 0, voidType));
        definedTypes.AddGlobal(new MintType("u8", 1, architecture.CharSize == 1 ? charType : null));
        definedTypes.AddGlobal(new MintType("i8", 1, architecture.CharSize == 1 ? charType : null));
        definedTypes.AddGlobal(new MintType("u16", 2, architecture.IntSize == 2 ? uintType : null));
        definedTypes.AddGlobal(new MintType("i16", 2, architecture.IntSize == 2 ? intType : null));
        definedTypes.AddGlobal(new MintType("u32", 4, architecture.IntSize == 4 ? uintType : null));
        definedTypes.AddGlobal(new MintType("i32", 4, architecture.IntSize == 4 ? intType : null));
        definedTypes.AddGlobal(new MintType("u64", 8, architecture.IntSize == 8 ? intType : null));
        definedTypes.AddGlobal(new MintType("i64", 8, architecture.IntSize == 8 ? intType : null));
        //AddGlobal(new MintType("u128", 16));
        //AddGlobal(new MintType("i128", 16));
        //AddGlobal(new MintType("f16", 2));
        definedTypes.AddGlobal(new MintType("f32", 4, architecture.FloatSize == 4 ? floatType : null));
        definedTypes.AddGlobal(new MintType("f64", 8, architecture.FloatSize == 8 ? floatType : null));
        
        
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
