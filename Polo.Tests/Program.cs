using System.CommandLine;
using System.CommandLine.Parsing;
using Polo;

var interpretOption = new Option<FileInfo?>(
    name: "--interpret",
    description: "Target mint source file to execute with basic interpreter");
var compileOption = new Option<FileInfo?>(
    name: "--compile",
    description: "Target mint source file to compile to native binary");

var rootCommand = new RootCommand("Preview Mint programming language analyser and interpreter");
rootCommand.AddOption(interpretOption);
rootCommand.SetHandler(InterpretFile, interpretOption);
rootCommand.AddOption(compileOption);
rootCommand.SetHandler(CompileFile, compileOption);

return await rootCommand.InvokeAsync(args);

static void InterpretFile(FileInfo? fileInfo)
{
    if (fileInfo is null || !fileInfo.Name.EndsWith(".mt") || !File.Exists(fileInfo.FullName))
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("[FATAL]: Specified file was not a mint (.mt) source file or could not be found");
        Console.ResetColor();
        return;
    }

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"Started interpreter execution of file {fileInfo.Name}");
    Console.ResetColor();
    var environment = new MintRunner();
    var source = File.ReadAllText(fileInfo.FullName);
    environment.Interpret(source);
}

static void CompileFile(FileInfo? fileInfo)
{
    if (fileInfo is null || !File.Exists(fileInfo.FullName))
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("[FATAL]: Specified file could not be found");
        Console.ResetColor();
        return;
    }
    if (fileInfo.Name.EndsWith(".mt"))
    {
        var environment = new MintRunner();
        var source = File.ReadAllText(fileInfo.FullName);
        environment.CompileQBE(source);
    }
}
