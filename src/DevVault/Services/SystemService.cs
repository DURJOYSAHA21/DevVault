using System.Diagnostics;
using DevVault.Models;

namespace DevVault.Services;

public class SystemService
{
    public DiagnosticResult CheckWindows()
    {
        return new DiagnosticResult
        {
            Name = "Windows",
            IsInstalled = OperatingSystem.IsWindows(),
            Version = Environment.OSVersion.Version.ToString(),
            Message = OperatingSystem.IsWindows()
                ? "Windows detected"
                : "Windows is required"
        };
    }

    public DiagnosticResult CheckPowerShell()
    {
        try
        {
            var process = new Process();

            process.StartInfo.FileName = "powershell";
            process.StartInfo.Arguments =
                "-Command \"$PSVersionTable.PSVersion.ToString()\"";

            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            string version =
                process.StandardOutput.ReadToEnd().Trim();

            process.WaitForExit();

            return new DiagnosticResult
            {
                Name = "PowerShell",
                IsInstalled = process.ExitCode == 0,
                Version = version,
                Message = process.ExitCode == 0
                    ? "PowerShell detected"
                    : "PowerShell check failed"
            };
        }
        catch
        {
            return new DiagnosticResult
            {
                Name = "PowerShell",
                IsInstalled = false,
                Message = "PowerShell was not found"
            };
        }
    }
}