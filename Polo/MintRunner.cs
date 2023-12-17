using Polo.Exceptions;
using Polo.Lexer;
using Polo.Lowering;
using Polo.Runtime;
using Polo.SyntaxAnalysis;

public class MintRunner
{
    public void Interpret(string source)
    {
        try
        {
            var interpreter = new Interpreter();
            var tokens = new Scanner(source).Run();
            var statements = new Parser(tokens).Run();
            interpreter.Run(statements);
        }
        catch (Exception error) when (error is ScanningErrorException or ParsingErrorException or RuntimeErrorException)
        {
            ReportError(error.StackTrace == null ? error.Message : $"{error.Message}\n{error.StackTrace}");
        }
    }
    
    public Task Compile(string source)
    {
        try
        {
            /*var interpreter = new ILInterpreterVM();
            var tokens = new Scanner(source).Run();
            var statements = new Parser(tokens).Run();
            var loadedObject = new ILConverter(statements).Run();
            interpreter.Run(loadedObject);*/
        }
        catch (Exception error) when (error is ScanningErrorException or ParsingErrorException or RuntimeErrorException)
        {
            ReportError(error.StackTrace == null ? error.Message : $"{error.Message}\n{error.StackTrace}");
        }

        return Task.CompletedTask;
    }

    private void ReportError(string message)
        => WriteConsoleColour(ConsoleColor.Red, message);

    private static void WriteConsoleColour(ConsoleColor colour, string text)
    {
        Console.ForegroundColor = colour;
        Console.Write(text);
        Console.ResetColor();
    }
}
