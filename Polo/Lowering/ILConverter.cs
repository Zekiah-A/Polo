using System.Collections.Immutable;
using System.Runtime.InteropServices;
using System.Text;
using Polo.Exceptions;
using Polo.Lexer;
using Polo.Runtime;
using Polo.SyntaxAnalysis;

namespace Polo.Lowering;

internal unsafe class ILConverter : IExpressionVisitor<object?, object?>, IStatementVisitor<object?>
{
    private ILState state;
    private StringBuilder data;
    private Dictionary<string, List<string>> definedIdentifiers; // Method, Identifier

    public ILConverter(ImmutableArray<Statement> statements)
    {
        definedIdentifiers = new Dictionary<string, List<string>>
        {
            { "main", new List<string> {} }
        };
        data = new StringBuilder();
        state = ILState.Default;
    }

    public string Run(ImmutableArray<Statement> statements)
    {
        var ilBuilder = new StringBuilder();
        ilBuilder.AppendLine("export function w $main() {");
        ilBuilder.AppendLine("@start");

        foreach (var statement in statements)
        {
            var source = (string) Execute(statement);
            ilBuilder.AppendLine(source);
        }

        ilBuilder.AppendLine("    ret 0");
        ilBuilder.AppendLine("}");
        ilBuilder.AppendLine(data.ToString());
        return ilBuilder.ToString();
    }
    
    private object? Evaluate(Expression expression)
    {
        return expression.Accept(this);
    }

    public object? VisitBinaryExpression(Binary binary)
    {
        var leftSource = Evaluate(binary.Left);
        var rightSource = Evaluate(binary.Right);

        var source = $"add {leftSource}, {rightSource}";
        return source.ToString();
    }

    public object? VisitGroupingExpression(Grouping grouping)
    {
        throw new NotImplementedException();
    }
    
    public object? VisitLiteralExpression(Literal literal)
    {
        var literalObject = ILLiteral.CreateFrom(literal.Value);
        return literalObject;
    }

    public object? VisitUnaryExpression(Unary unary)
    {
        throw new NotImplementedException();
    }

    public object? VisitVariableExpression(Variable variable)
    {
        return "%" + variable.Name;
        //throw new NotImplementedException();
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
        var ilBuilder = new StringBuilder();
        ilBuilder.Append("    %");
        ilBuilder.Append(let.Name);
        ilBuilder.Append(" =");

        definedIdentifiers["main"].Add(let.Name);

        object? value = null;
        if (let.Initializer is not null)
        {
            value = Evaluate(let.Initializer);
        }
        if (value is null)
        {
            throw new RuntimeErrorException(
                $"Could not initialise variable, no initialiser provided");
        }
        if (value is ILLiteral ilType)  // Literal assignment
        {
            ilBuilder.Append("w sub ");
            ilBuilder.Append(ilType.Value);
            ilBuilder.Append(", 0");
        }
        else if (value is string expressionValue) // Expr assignment
        {
            ilBuilder.Append("w ");
            ilBuilder.Append(expressionValue);
        }
        ilBuilder.AppendLine();
        return ilBuilder.ToString();
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

    private readonly string identifierChars = "abcdefghijklmnopqrstuvwxyz";

    private string UniqueIdentifier(string method)
    {
        var i = 0;
        var identifier = (Span<char>) stackalloc char[1];
        var methodIdentifiers = definedIdentifiers[method];
        do
        {
            var next = identifierChars[i];
            if (i == identifierChars.Length - 1)
            {
                var newIdentifier = (Span<char>) stackalloc char[identifier.Length + 1];
                identifier.CopyTo(newIdentifier);
                identifier = newIdentifier;
            }

            identifier[identifier.Length - 1] = next;
            i++;
        }
        while (methodIdentifiers.Contains(identifier.ToString()));
        return identifier.ToString();
    }

    public object? VisitDebugStatement(Debug debug)
    {
        var ilBuilder = new StringBuilder();
        var paramIdentifiers = new List<ParamIdentifier>();

        foreach (var param in debug.Parameters)
        {
            // All expressions must be turned into assignments
            var paramIdentifier = UniqueIdentifier("main");
            var let = new Let(paramIdentifier, "i32", param);
            var source = Execute(let);
            paramIdentifiers.Add(new ParamIdentifier(paramIdentifier,  "i32"));
            ilBuilder.Append(source);
        }

        ilBuilder.Append("    call $printf(");
        ilBuilder.Append("l $");
        var dataIdentifier = UniqueIdentifier("main"); // TODO:  Data should have  it's own identifiers  tbh
        ilBuilder.Append(dataIdentifier);
        ilBuilder.Append(", ...");

        if (paramIdentifiers.Count > 0)
        {
            // Add data entry
            data.Append("data $");
            data.Append(dataIdentifier);
            data.Append(" = { b \"");
            for (var i = 0; i < paramIdentifiers.Count; i++)
            {
                var identifier = paramIdentifiers[i];
                ilBuilder.Append(", w %");
                ilBuilder.Append(identifier.Name);

                // Add printf formatters to data
                if (i != 0)
                {
                    data.Append(' ');
                }
                data.Append(TypeInformation.GetFormatString(identifier.TypeName));
            }
            data.Append("\\n\", b 0 }");
            data.AppendLine();
        }

        ilBuilder.Append(")");
        ilBuilder.AppendLine();
        return ilBuilder.ToString();
    }

    public object? VisitReturnStatement(Return @return)
    {
        throw new NotImplementedException();
    }

    private object? Execute(Statement statement)
    {
        return statement.Accept(this);
    }
}
