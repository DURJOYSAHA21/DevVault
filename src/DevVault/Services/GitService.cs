using System.Diagnostics;
using DevVault.Models;

namespace DevVault.Services;

public class GitService
{
    public DiagnosticResult CheckGit()
    {
        try
        {
            var process = new Process();

            process.StartInfo.FileName = "git";
            process.StartInfo.Arguments = "--version";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            string output = process.StandardOutput.ReadToEnd().Trim();

            process.WaitForExit();

            return new DiagnosticResult
            {
                Name = "Git",
                IsInstalled = process.ExitCode == 0,
                Version = output.Replace("git version ", ""),
                Message = process.ExitCode == 0
                    ? "Git detected"
                    : "Git check failed"
            };
        }
        catch
        {
            return new DiagnosticResult
            {
                Name = "Git",
                IsInstalled = false,
                Message = "Git was not found"
            };
        }
    }
}