using DevVault.Models;

namespace DevVault.Services;

public class PathService
{
    public DiagnosticResult CheckPath()
    {
        var path = Environment.GetEnvironmentVariable("PATH");

        bool exists = !string.IsNullOrWhiteSpace(path);

        return new DiagnosticResult
        {
            Name = "PATH",
            IsInstalled = exists,
            Message = exists
                ? "PATH environment variable is configured"
                : "PATH environment variable is missing"
        };
    }
}