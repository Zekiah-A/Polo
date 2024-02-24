using System.Collections.Immutable;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.Primitives;
using OneOf;
using Polo.Exceptions;
using Polo.Lexer;
using Polo.Runtime;
using Polo.SyntaxAnalysis;

namespace Polo.Lowering;

internal unsafe class ILConverter : IExpressionVisitor<ContextSource, object?>, IStatementVisitor<string>
{
    private StringBuilder data;
    private Dictionary<string, List<string>> definedIdentifiers; // Method, Identifier

    public ILConverter(ImmutableArray<Statement> statements)
    {
        definedIdentifiers = new Dictionary<string, List<string>>
        {
            { "main", new List<string> {} }
        };
        data = new StringBuilder();
    }

    public string Run(ImmutableArray<Statement> statements)
    {
        var ilBuilder = new StringBuilder();
        ilBuilder.AppendLine("export function w $main() {");
        ilBuilder.AppendLine("@start");

        foreach (var statement in statements)
        {
            var source = (string) Execute(statement);
            ilBuilder.Append(source);
        }

        ilBuilder.AppendLine("    ret 0");
        ilBuilder.AppendLine("}");
        ilBuilder.AppendLine(data.ToString());
        return ilBuilder.ToString();
    }
    
    private ContextSource Evaluate(Expression expression)
    {
        return expression.Accept(this);
    }

    public ContextSource VisitBinaryExpression(Binary binary)
    {
        var leftSource = Evaluate(binary.Left);
        var rightSource = Evaluate(binary.Right);
        var ilBuilder = new StringBuilder();

        // Define left and right if they are literals, otherwise put L and R source
        switch (binary.Operator.Type)
        {
            case TokenType.Plus:
                ilBuilder.Append("add ");
                ilBuilder.Append(leftSource.Source);
                ilBuilder.Append(", ");
                ilBuilder.Append(rightSource.Source);
                break;
            case TokenType.EqualEqual:
                ilBuilder.Append("ceqw " );
                ilBuilder.Append(leftSource.Source);
                ilBuilder.Append(", "); 
                ilBuilder.Append(rightSource.Source);
                break;
            default:
                throw new NotImplementedException($"Can't convert operator {binary.Operator.Type}.");
        }
        var source = ilBuilder.ToString();

        // Pass down context until a line statement can handle it
        var contextBuilder = new StringBuilder();
        contextBuilder.Append(leftSource.Context);
        contextBuilder.Append(rightSource.Context);
        var context = contextBuilder.ToString();

        var contextSource = new ContextSource(source, SourceType.Instruction, context);
        return contextSource;
    }

    public ContextSource VisitGroupingExpression(Grouping grouping)
    {
        throw new NotImplementedException();
    }
    
    public ContextSource VisitLiteralExpression(Literal literal)
    {
        // WORKAROUND: Get rid of the literal, define it into a asm variable,
        // with the let definiton as the statement context, and a variable expression
        // that is their identifier. See ContextSource summary for more info.
        var literalVariable = $"%{UniqueIdentifier("main")}";
        var ilBuilder = new StringBuilder();

        if (literal.Type == LiteralType.String)
        {
            // Add data entry
            var dataIdentifier = $"${UniqueIdentifier("main")}";
            data.Append("data ");
            data.Append(dataIdentifier);
            data.Append(" = { b \"");
            data.Append(literal.Value);
            data.Append("\", b 0 }");
            data.AppendLine();

            // Reference data entry
            ilBuilder.Append("    ");
            ilBuilder.Append(literalVariable);
            ilBuilder.Append(" =l sub ");
            ilBuilder.Append(dataIdentifier);
            ilBuilder.Append(", 0");
        }
        else if (literal.Type == LiteralType.Integer)
        {
            ilBuilder.Append("    ");
            ilBuilder.Append(literalVariable);
            ilBuilder.Append(" =w ");
            ilBuilder.Append("sub ");
            ilBuilder.Append(literal.Value);
            ilBuilder.Append(", 0");
        }

        var definitionStatement = ilBuilder.ToString();
        var contextSource = new ContextSource(literalVariable, SourceType.Variable, definitionStatement);
        return contextSource;
    }

    public ContextSource VisitUnaryExpression(Unary unary)
    {
        throw new NotImplementedException();
    }

    public ContextSource VisitVariableExpression(Variable variable)
    {
        var contextSource = new ContextSource($"%{variable.Name}", SourceType.Variable);
        return contextSource;
    }

    public ContextSource VisitAssignExpression(Assign assign)
    {
        throw new NotImplementedException();
    }

    public ContextSource VisitLogicalExpression(Logical logical)
    {
        throw new NotImplementedException();
    }

    public ContextSource VisitFunctionCallExpression(FunctionCall functionCall)
    {
        throw new NotImplementedException();
    }

    public string VisitBlockStatement(Block block)
    {
        throw new NotImplementedException();
    }

    public string VisitStatementExpression(StatementExpression statementExpression)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Convert instruction expression to variable expression in cases instruction can not be used
    /// </summary>
    /// <param name="name">Name of variable (without % variable indetifier)</param>
    /// <param name="instructionYielder">Expression which is resulting in an instruction ContextSource being produced</param>
    /// <returns>Converted variable ContextSource</returns>
    private ContextSource InstructionToVariable(string name, string type, Expression instructionYielder)
    {
        var letStatement = new Let(name, type, instructionYielder);
        var letSource = Execute(letStatement);
        return new ContextSource($"%{name}", SourceType.Variable, letSource);
    }

    public string VisitIfStatement(If @if)
    {
        var ilBuilder = new StringBuilder();
        ContextSource conditionSource = Evaluate(@if.Condition);
        ilBuilder.AppendLine(conditionSource.Context);
        var condition = conditionSource.Source;
        if (conditionSource.SourceType == SourceType.Instruction)
        {
            // WORKAROUND: We can only work with Variables/Data locations, so create a
            // variable expression from the instruction expression
            var conditionName = UniqueIdentifier("main");
            var variableSource = InstructionToVariable(conditionName, "i32", @if.Condition);
            ilBuilder.AppendLine(variableSource.Context);
            condition = variableSource.Source;
        }
        else if (conditionSource.SourceType is SourceType.Label)
        {
            throw new InvalidOperationException("If statement condition can not be a jump label");
        }

        var ifIdentifier = UniqueIdentifier("main");
        var elseIdentifier = UniqueIdentifier("main");
        var endIdentifier = UniqueIdentifier("main");
        
        // Jmp
        ilBuilder.Append("    jnz ");
        ilBuilder.Append(condition);
        ilBuilder.Append(", @");
        ilBuilder.Append(ifIdentifier);
        ilBuilder.Append(", @");
        ilBuilder.Append(elseIdentifier);
        ilBuilder.Append("          # condition");
        ilBuilder.AppendLine();

        // If label
        ilBuilder.Append("@");
        ilBuilder.Append(ifIdentifier);
        ilBuilder.Append("                         # if");
        ilBuilder.AppendLine();
        // If jmp to end
        ilBuilder.Append("    jmp @");
        ilBuilder.Append(endIdentifier);
        ilBuilder.AppendLine();
        
        // Else label
        ilBuilder.Append("@");
        ilBuilder.Append(elseIdentifier);
        ilBuilder.Append("                         # else");
        ilBuilder.AppendLine();

        ilBuilder.Append("@");
        ilBuilder.Append(endIdentifier);
        ilBuilder.Append("                         # endif");
        ilBuilder.AppendLine();

        return ilBuilder.ToString();
    }

    public string VisitLetStatement(Let let)
    {
        var ilBuilder = new StringBuilder();
        definedIdentifiers["main"].Add(let.Name);

        if (let.Initialiser is not null)
        {
            // WORKAROUND: Handles an edge case of defintion loops, where a literal (does not count as expression)
            // neeeds to be defined as a variable expression. See ContextSource summary for more info.
            string value;
            if (let.Initialiser is Literal literalCondition)
            {
                value = $"sub {literalCondition.Value}, 0";
            }
            else
            {
                var valueSource = Evaluate(let.Initialiser);
                ilBuilder.AppendLine(valueSource.Context);
                value = valueSource.Source;
            }

            ilBuilder.Append("    %");
            ilBuilder.Append(let.Name);
            ilBuilder.Append(" =w ");
            ilBuilder.Append(value);
            ilBuilder.AppendLine();
        }
        else
        {
            throw new NotImplementedException("Let statements can currently only be defined with an initialiser");
        }

        return ilBuilder.ToString();
    }

    public string VisitDefStatement(Def def)
    {
        throw new NotImplementedException();
    }

    public string VisitWhileStatement(While @while)
    {
        throw new NotImplementedException();
    }

    public string VisitFunctionStatement(Function function)
    {
        throw new NotImplementedException();
    }

    // TODO: Create different identifier methods for within methods, data entries and labels.
    private string UniqueIdentifier(string method)
    {
        var methodIdentifiers = definedIdentifiers[method];

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

    public string VisitDebugStatement(Debug debug)
    {
        var ilBuilder = new StringBuilder();
        var paramIdentifiers = new List<ParamIdentifier>();

        foreach (var paramExpression in debug.Parameters)
        {
            var paramSource = Evaluate(paramExpression);
            if (paramSource.SourceType == SourceType.Instruction)
            {
                var paramName = UniqueIdentifier("main");
                var variableSource = InstructionToVariable(paramName, "i32", paramExpression);
                ilBuilder.AppendLine(variableSource.Context);
                paramIdentifiers.Add(new ParamIdentifier(variableSource.Source,  "i32"));
            }
            else
            {
                ilBuilder.AppendLine(paramSource.Context);
                paramIdentifiers.Add(new ParamIdentifier(paramSource.Source,  "i32"));
            }
        }

        ilBuilder.Append("    call $printf(");
        ilBuilder.Append("l $");
        var dataIdentifier = UniqueIdentifier("main");
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
                ilBuilder.Append(", w ");
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

    public string VisitReturnStatement(Return @return)
    {
        throw new NotImplementedException();
    }

    private string Execute(Statement statement)
    {
        return statement.Accept(this);
    }
}
