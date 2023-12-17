using Polo.Exceptions;
using Polo.Lexer;
using Polo.Runtime;
using Polo.SyntaxAnalysis;

public class MintRepl
{
    private readonly List<string> replPrevious = new();
    private int replPreviousIndex;

    public async Task Start()
    {
        var runner = new MintRunner();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.ResetColor();

        var input = "";
        Console.Write(">> ");

        while (true)
        {
            var key = Console.ReadKey();
            
            switch (key.Key)
            {
                case ConsoleKey.Backspace:
                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    if (input?.Length < 1) continue;
                    input = input?[..^1];
                    Console.Write("\b");
                    continue;
                case ConsoleKey.UpArrow:
                    input = replPrevious.ElementAtOrDefault(^replPreviousIndex);
                    replPreviousIndex++;
                    replPreviousIndex = Math.Min(replPreviousIndex, replPrevious.Count);
                    continue;
                case ConsoleKey.DownArrow:
                    input = replPrevious.ElementAtOrDefault(^replPreviousIndex);
                    replPreviousIndex--;
                    replPreviousIndex = Math.Max(replPreviousIndex, 0);
                    break;
            }

            input += key.KeyChar.ToString();
            if (key.Key != ConsoleKey.Enter) continue;
            
            Console.WriteLine();
            Console.Write(">> ");

            if (!string.IsNullOrEmpty(input))
            {
                runner.Interpret(input);
            }

            replPrevious.Add(input);
            input = "";
        }
    }
}