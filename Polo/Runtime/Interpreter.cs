using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Polo.Exceptions;
using Polo.Lexer;
using Polo.SyntaxAnalysis;
using Polo.TypeAnalysis;
using Debug = Polo.SyntaxAnalysis.Debug;
using Type = Polo.SyntaxAnalysis.Type;

namespace Polo.Runtime;

internal class Interpreter : IExpressionVisitor<RuntimeValue, object?>, IStatementVisitor<object>
{
    private readonly MintEnvironment environment;
    private readonly DefinedTypes definedTypes;
    private readonly ImmutableArray<Statement> statements;
    
    public Interpreter(ImmutableArray<Statement> statements, DefinedTypes definedTypes)
    {
        environment = new MintEnvironment();
        this.statements = statements;
        this.definedTypes = definedTypes;
    }

    public void Run()
    {
        environment.PushFrame();
        foreach (var statement in statements)
        {
            Execute(statement);
        }
        environment.ExitFrame();
    }

    public unsafe RuntimeValue VisitBinaryExpression(Binary binary)
    {
        // RuntimeValue
        var left = Evaluate(binary.Left);
        var right = Evaluate(binary.Right);

        // For structs, and non-primitive derived types. The interpreter will attempt
        // to find the mint handler for this cast.
        switch (binary.Operator.Type)
        {
            case TokenType.Plus:
            {
                if (left.TypeName != right.TypeName)
                {
                    throw new NotImplementedException(
                        $"Couldn't add operands of type {left} and ${right}.  No suitable conversion exists");
                }

                switch (left.TypeName)
                {
                    case "int":
                    {
                        var newValue = *(int*)left.Value + *(int*)right.Value;
                        return RuntimeValue.CreateFrom(newValue, "int");
                    }
                    case "float":
                    {
                        var newValue = *(float*)left.Value + *(float*)right.Value;
                        return RuntimeValue.CreateFrom(newValue, "float");
                    }
                }
                
                throw new NotImplementedException();
            }
            /*case TokenType.Minus:
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
                };*/
        }

        throw new RuntimeErrorException("Unknown operator.");
    }

    public RuntimeValue VisitGroupingExpression(Grouping grouping)
    {
        return Evaluate(grouping.Expression);
    }

    // Convert the C# typed literal to a mint compatible runtime type
    public RuntimeValue VisitLiteralExpression(Literal literal)
    {
        switch (literal.Type)
        {
            case LiteralType.String:
            {
                throw new NotImplementedException();
            }
            case LiteralType.Integer:
            {
                if (int.TryParse(literal.Value, out var intValue))
                {
                    return RuntimeValue.CreateFrom(intValue, "int");
                }
                throw new FormatException("Invalid format for Integer.");
            }
            case LiteralType.Decimal:
            {
                if (float.TryParse(literal.Value, out var floatValue))
                {
                    return RuntimeValue.CreateFrom(floatValue, "float");
                }
                throw new FormatException("Invalid format for Decimal.");
            }
            case LiteralType.Hex:
            {
                if (int.TryParse(literal.Value, NumberStyles.HexNumber, null, out var hexValue))
                {
                    return RuntimeValue.CreateFrom(hexValue, "int");
                }
                throw new FormatException("Invalid format for Hex.");
            }
            case LiteralType.Char:
            {
                if (literal.Value.Length == 1)
                {
                    return RuntimeValue.CreateFrom(literal.Value[0], "char");
                }
                throw new FormatException("Invalid format for Char.");
            }
            case LiteralType.Binary:
            {
                if (int.TryParse(literal.Value, NumberStyles.Integer, null, out int binaryValue))
                {
                    return RuntimeValue.CreateFrom(binaryValue, "int");
                }
                throw new FormatException("Invalid format for Binary.");
            }
            case LiteralType.Boolean:
            {
                if (bool.TryParse(literal.Value, out bool boolValue))
                {
                    return RuntimeValue.CreateFrom(boolValue, "bool");
                }
                throw new FormatException("Invalid format for Boolean.");
            }
            case LiteralType.Null:
            {
                throw new NotImplementedException();
            }
            default:
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }

    public RuntimeValue VisitUnaryExpression(Unary unary)
    {
        /*var right = Evaluate(unary.Right);
        switch (unary.Operator.Type)
        {
            case TokenType.Bang:
                return !IsTruthy(right);
            case TokenType.Minus:
                CheckNumberOperands(unary.Operator, right);
                return -(double)right;
        }*/

        throw new RuntimeErrorException("Unknown operator.");
    }

    public RuntimeValue VisitVariableExpression(Variable variable)
    {
        return environment.Get(variable.Name);
    }

    public RuntimeValue VisitFunctionCallExpression(FunctionCall call)
    {
        throw new NotImplementedException();
    }

    public RuntimeValue VisitAssignExpression(Assign assign)
    {
        var value = Evaluate(assign.Value);
        if (value is null)
        {
            throw new RuntimeErrorException("Could not assign variable. Right hand side did not produce a runtime value");
        }

        environment.Assign(assign.Name, value);
        return value;
    }

    public RuntimeValue VisitLogicalExpression(Logical logical)
    {
        var left = Evaluate(logical.Left);

        if (logical.Operator.Type == TokenType.Or)
        {
            if (IsTruthy(left))
            {
                return left;
            }
        }
        else
        {
            if (!IsTruthy(left))
            {
                return left;
            }
        }

        return Evaluate(logical.Right);
    }

    private RuntimeValue Evaluate(Expression expression)
    {
        return expression.Accept(this);
    }

    public object VisitBlockStatement(Block block)
    {
        environment.PushFrame();
        foreach (var statement in block.Statements)
        {
            Execute(statement);
        }
        environment.ExitFrame();
        return block;
    }

    public object VisitIfStatement(If @if)
    {
        if (IsTruthy(Evaluate(@if.Condition)))
        {
            Execute(@if.ThenBranch);
        }
        else if (@if.ElseBranch != null)
        {
            Execute(@if.ElseBranch);
        }

        return @if;
    }

    public object VisitReturnStatement(Return @return)
    {
        var result = Evaluate(@return.Expression);
        return result;
    }

    public object VisitTypeStatement(Type type)
    {
        throw new NotImplementedException();
    }

    public unsafe object VisitDebugStatement(Debug debug)
    {
        var builder = new StringBuilder();
        foreach (var expression in debug.Parameters)
        {
            // Should be a RuntimeType
            var value = Evaluate(expression);
            if (value is null)
            {
                throw new Exception("Evaluated expression did not produce a valid RuntimeValue");
            }

            switch (value.TypeName)
            {
                case "void":
                {
                    builder.Append("void");
                    break;
                }
                case "u8":
                {
                    var byteValue = *(byte*)value.Value;
                    builder.Append(byteValue);
                    break;
                }
                case "i8":
                {
                    var byteValue = *(sbyte*)value.Value;
                    builder.Append(byteValue);
                    break;
                }
                case "u16":
                {
                    var uintValue = *(ushort*)value.Value;
                    builder.Append(uintValue);
                    break;
                }
                case "i16":
                {
                    var shortValue = *(short*)value.Value;
                    builder.Append(shortValue);
                    break;
                }
                case "char":
                {
                    var charValue = *(char*)value.Value;
                    builder.Append(charValue);
                    break;
                }
                case "uint" or "u32":
                {
                    var shortValue = *(short*)value.Value;
                    builder.Append(shortValue);
                    break;
                }
                case "int" or "i32":
                { 
                    var intValue = *(int*)value.Value;
                    builder.Append(intValue);
                    break;
                }
                case "u64":
                {
                    var ulongValue = *(ulong*)value.Value;
                    builder.Append(ulongValue);
                    break;
                }
                case "i64":
                {
                    var longValue = *(ulong*)value.Value;
                    builder.Append(longValue);
                    break;
                }
                case "f32" or "float":
                {
                    var floatValue = *(float*)value.Value;
                    builder.Append(floatValue);
                    break;
                }
                case "f64":
                {
                    var doubleValue = *(double*)value.Value;
                    builder.Append(doubleValue);
                    break;
                }
            }
        }

        var result = builder.ToString();
        Console.WriteLine(result);
        return result;
    }
    
    public object VisitStatementExpression(StatementExpression statementExpression)
    {
        Evaluate(statementExpression.Expression);
        return statementExpression;
    }

    private unsafe RuntimeValue ImplicitCast(RuntimeValue runtimeValue, string targetType)
    {
        throw new NotImplementedException();

        // If type is a primitive, casting will be done by the interpreter
        switch (targetType)
        {
            case "i32":
            {
                switch (runtimeValue.TypeName)
                {
                    case "f64":
                        var doubleValue = *(double*) runtimeValue.Value;
                        var intValue = Convert.ToInt32(doubleValue);
                        //var rtType = RuntimeValue.CreateFrom(intValue);
                        //return rtType;
                }
                break;
            }
        }
        
        // TODO: For mint non-primitive types. The interpreter will find find the mint handler for this cast and call
        // TODO: it to achieve the result
        throw new RuntimeErrorException($"Can not perform implicit cast between incompatible types {runtimeValue.TypeName} and {targetType}");
    }

    public object VisitLetStatement(Let let)
    {
        object? value = null;
        if (let.Initialiser != null)
        {
            value = Evaluate(let.Initialiser);
        }
        if (value is not RuntimeValue runtimeValue)
        {
            throw new RuntimeErrorException("Could not initialise variable. Initializer did not produce a runtime value");
        }
        if (runtimeValue.TypeName != let.TypeName)
        {
            runtimeValue = ImplicitCast(runtimeValue, let.TypeName);
        }

        environment.PushStack(runtimeValue, let.Name);
        return let;
    }

    public object VisitDefStatement(Def def)
    {
        object? value = null;
        if (def.Initializer != null)
        {
            value = Evaluate(def.Initializer);
        }

        //environment.Define(def.Name, value);
        return def;
    }

    public object VisitWhileStatement(While @while)
    {
        while (IsTruthy(Evaluate(@while.Condition)))
        {
            Execute(@while.Body);
        }

        return @while;
    }

    public object VisitFunctionStatement(Function function)
    {
        throw new NotImplementedException();
    }
    
    private void Execute(Statement statement)
    {
        statement.Accept(this);
    }

    private static bool IsTruthy(RuntimeValue value)
    {
        throw new NotImplementedException();
        /*if (value == null)
        {
            return false;
        }

        return value is not bool b || b;*/
    }

    private static bool IsEqual(RuntimeValue left, RuntimeValue right)
    {
        throw new NotImplementedException();
        /*return left switch
        {
            null when right == null => true,
            null => false,
            _ => left.Equals(right)
        };*/
    }
}
