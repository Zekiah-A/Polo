using System.Collections.Immutable;
using Polo.SyntaxAnalysis;
using Type = Polo.SyntaxAnalysis.Type;
using Polo.QBE;
using Polo.TypeAnalysis;

namespace Polo.Lowering;

internal unsafe class QbeConverter : IExpressionVisitor<object?, object?>, IStatementVisitor<object?>
{
    private readonly List<DataDef> data;
    private readonly Dictionary<string, List<string>> identifiers; // Method, Identifier
    private readonly ImmutableArray<Statement> statements;
    private readonly DefinedTypes definedTypes;
    
    public QbeConverter(ImmutableArray<Statement> statements, DefinedTypes definedTypes)
    {
        this.statements = statements;
        this.definedTypes = definedTypes;
        identifiers = new Dictionary<string, List<string>>
        {
            { "main", [] }
        };
        data = new List<DataDef>();
    }

    public string Run()
    {
        throw new NotImplementedException();
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

    public object VisitBlockStatement(SyntaxAnalysis.Block block)
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

    public object? VisitFunctionStatement(SyntaxAnalysis.Function function)
    {
        var arguments = new List<(IType, IValue)>();
        foreach (var param in function.Parameters)
        {
            
        }

        throw new NotImplementedException();
    }

    // TODO: Create different identifier methods for within methods, data entries and labels.
    private string UniqueIdentifier(string method)
    {
        var methodIdentifiers = identifiers[method];

        var stringLength = 1;
        while (true)
        {
            var buffer = (Span<char>) stackalloc char[stringLength];
            buffer.Fill('a'); // Initialise with a

            while (true)
            {
                var generated = new string(buffer);
                if (!methodIdentifiers.Contains(generated))
                {
                    methodIdentifiers.Add(generated);
                    return generated;
                }

                var index = buffer.Length - 1;
                while (index >= 0 && buffer[index] == 'z')
                {
                    buffer[index] = 'a';
                    index--;
                }

                // All combinations exhausted
                if (index < 0)
                {
                    break;
                }
                buffer[index]++;
            }

            stringLength++;
        }
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

    private object? Execute(Statement statement)
    {
        return statement.Accept(this);
    }
}
