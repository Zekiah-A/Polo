using Polo.SyntaxAnalysis;

namespace Polo.Lowering;

public class Asssembly
{
    // Will invoke QBE and nasm/cc/system asm compiler sequentially to produce binary
    public Asssembly(string source)
    {
        Console.WriteLine(source);
    }

    public FileInfo Run()
    {
        return null!;
    }
}