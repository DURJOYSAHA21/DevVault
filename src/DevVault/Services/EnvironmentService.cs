using DevVault.Models;

namespace DevVault.Services;

public class EnvironmentService
{
    public DiagnosticResult CheckDotNetEnvironment()
    {
        var dotnetRoot = Environment.GetEnvironmentVariable("DOTNET_ROOT");

        bool exists = !string.IsNullOrWhiteSpace(dotnetRoot);

        return new DiagnosticResult
        {
            Name = "DOTNET_ROOT",
            IsInstalled = exists,
            Version = dotnetRoot ?? string.Empty,
            Message = exists
                ? ".NET environment variable is configured"
                : "DOTNET_ROOT is not configured"
        };
    }
}