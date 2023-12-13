using System.CommandLine;

var runner = new MintRunner();
var fileOption = new Option<FileInfo?>(
    name: "--interpret",
    description: "Target mint source file to execute with basic interpreter");

var rootCommand = new RootCommand("Preview Mint programming language analyser and interpreter");
rootCommand.AddOption(fileOption);

rootCommand.SetHandler(ExecuteFile,
fileOption);

return await rootCommand.InvokeAsync(args);


static void ExecuteFile(FileInfo? fileInfo)
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
    environment.Execute(source);
}