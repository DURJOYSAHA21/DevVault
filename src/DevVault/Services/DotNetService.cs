using System.Diagnostics;
using DevVault.Models;

namespace DevVault.Services;

public class DotNetService
{
    public DiagnosticResult CheckDotNet()
    {
        try
        {
            var process = new Process();

            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = "--version";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            string version = process.StandardOutput.ReadToEnd().Trim();

            process.WaitForExit();

            return new DiagnosticResult
            {
                Name = ".NET SDK",
                IsInstalled = process.ExitCode == 0,
                Version = version,
                Message = process.ExitCode == 0
                    ? ".NET SDK detected"
                    : ".NET SDK check failed"
            };
        }
        catch
        {
            return new DiagnosticResult
            {
                Name = ".NET SDK",
                IsInstalled = false,
                Message = ".NET SDK was not found"
            };
        }
    }
}