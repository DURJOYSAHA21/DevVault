using DevVault.Models;
using DevVault.Services;

namespace DevVault.Commands;

public static class DoctorCommand
{
    public static void Run()
    {
        var systemService = new SystemService();
        var dotNetService = new DotNetService();
        var gitService = new GitService();
        var dockerService = new DockerService();
        var pathService = new PathService();

        var requiredResults = new[]
        {
            systemService.CheckWindows(),
            systemService.CheckPowerShell(),
            pathService.CheckPath(),
            dotNetService.CheckDotNet(),
            gitService.CheckGit()
        };

        var dockerResult = dockerService.CheckDocker();

        Console.WriteLine();
        Console.WriteLine("DevVault Doctor");
        Console.WriteLine("======================================");
        Console.WriteLine();

        Console.WriteLine("SYSTEM");

        foreach (var result in requiredResults.Take(3))
        {
            PrintResult(result);
        }

        Console.WriteLine();
        Console.WriteLine("DEVELOPMENT TOOLS");

        foreach (var result in requiredResults.Skip(3))
        {
            PrintResult(result);
        }

        Console.WriteLine();
        Console.WriteLine("OPTIONAL TOOLS");

        PrintOptionalResult(dockerResult);

        Console.WriteLine();
        Console.WriteLine("--------------------------------------");

        int problems = requiredResults.Count(
            result => !result.IsInstalled);

        if (problems == 0)
        {
            Console.WriteLine(
                "Result: Required environment looks healthy.");
        }
        else
        {
            Console.WriteLine(
                $"Result: {problems} required issue(s) found.");
        }

        Console.WriteLine();
    }

    private static void PrintResult(DiagnosticResult result)
    {
        string symbol = result.IsInstalled
            ? "[OK]"
            : "[FAIL]";

        string version = string.IsNullOrWhiteSpace(result.Version)
            ? ""
            : $" ({result.Version})";

        Console.WriteLine(
            $"{symbol} {result.Name}{version} - {result.Message}");
    }

    private static void PrintOptionalResult(
        DiagnosticResult result)
    {
        string symbol = result.IsInstalled
            ? "[OK]"
            : "[INFO]";

        string version = string.IsNullOrWhiteSpace(result.Version)
            ? ""
            : $" ({result.Version})";

        Console.WriteLine(
            $"{symbol} {result.Name}{version} - " +
            $"{(result.IsInstalled ? result.Message : "Not installed (optional)")}");
    }
}