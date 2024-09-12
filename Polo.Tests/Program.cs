using System.CommandLine;
using System.Diagnostics;
using Polo;

var rootCommand = new RootCommand("Preview Mint programming language analyser and interpreter");

// Define the interpret command and its option
var interpretCommand = new Command("interpret", "Execute mint source file with basic interpreter");
var interpretOption = new Option<FileInfo?>(
    name: "--file",
    description: "Target mint source file to execute with basic interpreter");
interpretCommand.AddOption(interpretOption);
interpretCommand.SetHandler(InterpretFile, interpretOption);
rootCommand.AddCommand(interpretCommand);

// Define the compile command and its option
var compileCommand = new Command("compile", "Compile mint source file to native binary");
var compileOption = new Option<FileInfo?>(
    name: "--file",
    description: "Target mint source file to compile to native binary");
compileCommand.AddOption(compileOption);
compileCommand.SetHandler(CompileFile, compileOption);
rootCommand.AddCommand(compileCommand);

return await rootCommand.InvokeAsync(args);

// TODO: Consider print-comments option for debugging and sample testing, where program will print
// TODO: all source code comments as it runs.

static void InterpretFile(FileInfo? fileInfo)
{
    if (fileInfo is null || !fileInfo.Name.EndsWith(".mt") || !File.Exists(fileInfo.FullName))
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine("[FATAL]: Specified file was not a mint (.mt) source file or could not be found.");
        Console.ResetColor();
        return;
    }

    Console.ForegroundColor = ConsoleColor.Green;
    Console.Error.WriteLine($"Interpreting {fileInfo.Name}");
    Console.ResetColor();
    
    var environment = new MintRunner();
    var source = File.ReadAllText(fileInfo.FullName);
    
    var stopwatch = new Stopwatch();
    stopwatch.Start();
    environment.Interpret(source, fileInfo);
    stopwatch.Stop();
    Console.Error.WriteLine($"[Interpreting finished in {stopwatch.ElapsedMilliseconds}ms]");
}

static void CompileFile(FileInfo? fileInfo)
{
    if (fileInfo is null || !fileInfo.Name.EndsWith(".mt") || !File.Exists(fileInfo.FullName))
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine("[FATAL]: Specified file was not a mint (.mt) source file or could not be found.");
        Console.ResetColor();
        return;
    }

    Console.WriteLine("Compiling");
    var environment = new MintRunner();
    var source = File.ReadAllText(fileInfo.FullName);

    var stopwatch = new Stopwatch();
    stopwatch.Start();
    environment.Compile(source, fileInfo);
    stopwatch.Stop();
    Console.Error.WriteLine($"[Compilation finished in {stopwatch.ElapsedMilliseconds}ms]");
}