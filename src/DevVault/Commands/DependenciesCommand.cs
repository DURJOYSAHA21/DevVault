using DevVault.Services;

namespace DevVault.Commands;

public static class DependenciesCommand
{
    public static void Run(string projectPath)
    {
        var analyzer = new ProjectAnalyzerManager();

        var analysis = analyzer.Analyze(projectPath);

        Console.WriteLine();
        Console.WriteLine("DevVault Dependencies");
        Console.WriteLine("======================================");
        Console.WriteLine();

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

        Console.WriteLine("REQUIRED TOOLS");

        foreach (var projectType in analysis.ProjectTypes)
        {
            switch (projectType)
            {
                case "C# / .NET":
                    CheckDotNet();
                    break;

                case "Node.js / JavaScript":
                    CheckNode();
                    break;

                case "Python":
                    CheckPython();
                    break;
            }
        }

        if (analysis.ProjectTypes.Count == 0)
        {
            Console.WriteLine(
                "[INFO] No known project type detected.");
        }

        CheckGit();

        Console.WriteLine();

        Console.WriteLine("OPTIONAL TOOLS");

        CheckDocker();

        Console.WriteLine();
    }

    private static void CheckDotNet()
    {
        var service = new DotNetService();

        PrintResult(
            service.CheckDotNet());
    }

    private static void CheckNode()
    {
        var service = new NodeService();

        PrintResult(
            service.CheckNode());
    }

    private static void CheckPython()
    {
        var service = new PythonService();

        PrintResult(
            service.CheckPython());
    }

    private static void CheckGit()
    {
        var service = new GitService();

        PrintResult(
            service.CheckGit());
    }

    private static void CheckDocker()
    {
        var service = new DockerService();

        var result = service.CheckDocker();

        string symbol = result.IsInstalled
            ? "[OK]"
            : "[INFO]";

        string version =
            string.IsNullOrWhiteSpace(result.Version)
                ? ""
                : $" ({result.Version})";

        Console.WriteLine(
            $"{symbol} Docker{version} - " +
            $"{(result.IsInstalled
                ? result.Message
                : "Not installed (optional)")}");
    }

    private static void PrintResult(
        Models.DiagnosticResult result)
    {
        string symbol = result.IsInstalled
            ? "[OK]"
            : "[FAIL]";

        string version =
            string.IsNullOrWhiteSpace(result.Version)
                ? ""
                : $" ({result.Version})";

        Console.WriteLine(
            $"{symbol} {result.Name}{version} - " +
            $"{result.Message}");
    }
}