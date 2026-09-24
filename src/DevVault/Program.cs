
using System.CommandLine;
using DevVault.Commands;
using DevVault.Services;

var rootCommand = new RootCommand(
    "DevVault - Windows Developer Environment Assistant");


// ==============================
// DOCTOR COMMAND
// ==============================

var doctorCommand = new Command(
    "doctor",
    "Diagnose your development environment");

doctorCommand.SetAction(_ =>
{
    DoctorCommand.Run();
});

rootCommand.Subcommands.Add(doctorCommand);


// ==============================
// ANALYZE COMMAND
// ==============================

var analyzeCommand = new Command(
    "analyze",
    "Analyze a project");

var pathOption = new Option<string?>(
    "--path")
{
    Description = "Path of the project to analyze"
};

analyzeCommand.Options.Add(pathOption);

analyzeCommand.SetAction(parseResult =>
{
    var analyzer = new ProjectAnalyzerManager();
    var discoveryService = new ProjectDiscoveryService();
    var dependencyService = new DependencyService();

    string currentDirectory =
        parseResult.GetValue(pathOption)
        ?? Directory.GetCurrentDirectory();

    if (!Directory.Exists(currentDirectory))
    {
        Console.WriteLine();
        Console.WriteLine(
            $"Error: Directory not found: {currentDirectory}");
        Console.WriteLine();

        return;
    }

    currentDirectory = Path.GetFullPath(
        currentDirectory);

    var analysis = analyzer.Analyze(
        currentDirectory);

    var projects = discoveryService.Discover(
        currentDirectory);

    var dependencies = dependencyService.GetDependencies(
        currentDirectory);

    Console.WriteLine();
    Console.WriteLine("DevVault Project Analysis");
    Console.WriteLine("======================================");
    Console.WriteLine();

    Console.WriteLine(
        $"Project: {analysis.ProjectPath}");

    Console.WriteLine("PROJECT TYPES");

    if (analysis.ProjectTypes.Count == 0)
    {
        Console.WriteLine("[INFO] Unknown project type");
    }
    else
    {
        foreach (var projectType in analysis.ProjectTypes)
        {
            Console.WriteLine(
                $"[{projectType}]");
        }
    }

    Console.WriteLine();

    Console.WriteLine(
        $"Primary Type: {analysis.ProjectType}");

    Console.WriteLine(
        $"Primary Framework: {analysis.Framework}");

    Console.WriteLine();

    Console.WriteLine("PROJECTS");

    foreach (var project in projects)
    {
        Console.WriteLine(
            $"[{project.Type}] " +
            $"{project.Name} - " +
            $"{project.Framework}");
    }

    Console.WriteLine();

    Console.WriteLine("PROJECT FILES");

    Console.WriteLine(
        analysis.HasGit
            ? "[OK] Git repository"
            : "[FAIL] Git repository");

    Console.WriteLine(
        analysis.HasSolutionFile
            ? "[OK] Solution file"
            : "[INFO] No solution file");

    Console.WriteLine(
        analysis.HasProjectFile
            ? "[OK] .NET project file"
            : "[INFO] No .NET project file");

    Console.WriteLine();

    Console.WriteLine("CONFIGURATION");

    Console.WriteLine(
        analysis.HasEnvironmentFile
            ? "[FOUND] .env file"
            : "[INFO] No .env file");

    Console.WriteLine(
        analysis.HasPackageJson
            ? "[FOUND] package.json"
            : "[INFO] No package.json");

    Console.WriteLine(
        analysis.HasRequirementsFile
            ? "[FOUND] requirements.txt"
            : "[INFO] No requirements.txt");

    Console.WriteLine(
        analysis.HasPyProjectFile
            ? "[FOUND] pyproject.toml"
            : "[INFO] No pyproject.toml");

    Console.WriteLine();

    Console.WriteLine("CONTAINERIZATION");

    Console.WriteLine(
        analysis.HasDockerfile
            ? "[FOUND] Dockerfile"
            : "[INFO] No Dockerfile");

    Console.WriteLine();

    Console.WriteLine("DEPENDENCIES");

    if (dependencies.Count == 0)
    {
        Console.WriteLine(
            "[INFO] No dependencies found");
    }
    else
    {
        foreach (var dependency in dependencies)
        {
            Console.WriteLine(
                $"[{dependency.Ecosystem}] " +
                $"{dependency.Name} " +
                $"({dependency.Version}) " +
                $"- {dependency.Source} " +
                $"[{dependency.Status}]");
        }
    }

    Console.WriteLine();

    Console.WriteLine("DEPENDENCY SUMMARY");

    if (dependencies.Count == 0)
    {
        Console.WriteLine(
            "[INFO] No dependencies found");
    }
    else
    {
        var summary = dependencies
            .GroupBy(dependency => dependency.Ecosystem)
            .OrderBy(group => group.Key);

        foreach (var group in summary)
        {
            Console.WriteLine(
                $"{group.Key}: {group.Count()}");
        }

        int duplicateCount =
            dependencies.Count(
                dependency =>
                    dependency.Status == "Duplicate");

        Console.WriteLine(
            $"Duplicates: {duplicateCount}");

        Console.WriteLine(
            $"Total: {dependencies.Count}");
    }

    Console.WriteLine();
});

rootCommand.Subcommands.Add(analyzeCommand);


// ==============================
// DEPENDENCIES COMMAND
// ==============================

var dependenciesCommand = new Command(
    "dependencies",
    "Check project dependencies");

var dependenciesPathOption = new Option<string?>(
    "--path")
{
    Description = "Path of the project to check"
};

dependenciesCommand.Options.Add(
    dependenciesPathOption);

dependenciesCommand.SetAction(parseResult =>
{
    string projectPath =
        parseResult.GetValue(
            dependenciesPathOption)
        ?? Directory.GetCurrentDirectory();

    if (!Directory.Exists(projectPath))
    {
        Console.WriteLine();
        Console.WriteLine(
            $"Error: Directory not found: {projectPath}");
        Console.WriteLine();

        return;
    }

    projectPath = Path.GetFullPath(projectPath);

    DependenciesCommand.Run(projectPath);
});

rootCommand.Subcommands.Add(dependenciesCommand);


// ==============================
// PORTS COMMAND
// ==============================

var portsCommand = new Command(
    "ports",
    "Inspect local ports");

var portOption = new Option<int>(
    "--port")
{
    Description = "Check whether a specific port is in use"
};

portsCommand.Options.Add(portOption);

portsCommand.SetAction(parseResult =>
{
    var portService = new PortService();

    int port = parseResult.GetValue(
        portOption);

    if (port > 0)
    {
        portService.CheckPort(port);
    }
    else
    {
        portService.ShowListeningPorts();
    }
});

rootCommand.Subcommands.Add(portsCommand);


// ==============================
// RUN CLI
// ==============================

return rootCommand.Parse(args).Invoke();

