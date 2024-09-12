using System.Diagnostics;
using System.Runtime.InteropServices;
using Polo.Exceptions;
using Polo.Lexer;
using Polo.Runtime;
using Polo.SyntaxAnalysis;
using Polo.Lowering;
using Polo.TypeAnalysis;

namespace Polo;

public class MintRunner
{
    public void Interpret(string source, FileInfo? fileInfo = null)
    {
        try
        {
            var tokens = new Scanner(source).Run();
            var statements = new Parser(tokens).Run();
            var definedTypes = new TypeAnalyser(statements, TargetArchitecture.Interpreter).Run();
            new Interpreter(statements, definedTypes).Run();
        }
        catch (Exception error) when (error is ScanningErrorException or ParsingErrorException or RuntimeErrorException)
        {
            ReportError(error.StackTrace == null ? error.Message : $"{error.Message}\n{error.StackTrace}");
        }
    }
    
    private static TargetArchitecture GetTargetArchitecture()
    {
        return RuntimeInformation.ProcessArchitecture switch
        {
            Architecture.Arm64 => TargetArchitecture.Arm64,
            Architecture.X64 => TargetArchitecture.Amd64,
            _ => throw new NotImplementedException("This architecture is not supported.")
        };
    }
    
    public Task Compile(string source, FileInfo? fileInfo = null, TargetArchitecture? architecture = null)
    {
        architecture ??= GetTargetArchitecture();

        try
        {
            var tokens = new Scanner(source).Run();
            var statements = new Parser(tokens).Run();
            var definedTypes = new TypeAnalyser(statements, architecture).Run();
            var irResult = new QbeConverter(statements, definedTypes).Run();
            var runner = new QbeBuilder(irResult, architecture).Run();
        }
        catch (Exception error) when (error is ScanningErrorException or ParsingErrorException or RuntimeErrorException)
        {
            ReportError(error.StackTrace == null ? error.Message : $"{error.Message}\n{error.StackTrace}");
        }

        return Task.CompletedTask;
    }

    private static void ReportError(string message)
        => WriteConsoleColour(ConsoleColor.Red, message);

    private static void WriteConsoleColour(ConsoleColor colour, string text)
    {
        Console.ForegroundColor = colour;
        Console.Write(text);
        Console.ResetColor();
    }
}
