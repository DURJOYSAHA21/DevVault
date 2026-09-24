namespace DevVault.Models;

public class DiagnosticResult
{
    public string Name { get; set; } = string.Empty;

    public bool IsInstalled { get; set; }

    public string Version { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}