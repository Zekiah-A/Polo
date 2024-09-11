namespace Polo.Lowering;

public class QbeBuilder
{
    private readonly TargetArchitecture architecture;
    
    // TODO: invoke QBE and nasm/cc/system asm compiler sequentially to produce binary 
    // Will invoke QBE and nasm/cc/system asm compiler sequentially to produce binary
    public QbeBuilder(string source, TargetArchitecture architecture)
    {
        this.architecture = architecture;
        Console.WriteLine(source);
    }

    public FileInfo Run()
    {
        return null!;
    }
}