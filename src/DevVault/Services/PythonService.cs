using System.Diagnostics;
using DevVault.Models;

namespace DevVault.Services;

public class PythonService
{
    public DiagnosticResult CheckPython()
    {
        try
        {
            var process = new Process();

            process.StartInfo.FileName = "python";
            process.StartInfo.Arguments = "--version";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            string output =
                process.StandardOutput
                    .ReadToEnd()
                    .Trim();

            string error =
                process.StandardError
                    .ReadToEnd()
                    .Trim();

            process.WaitForExit();

            string version =
                !string.IsNullOrWhiteSpace(output)
                    ? output
                    : error;

            return new DiagnosticResult
            {
                Name = "Python",
                IsInstalled = process.ExitCode == 0,
                Version = version,
                Message = process.ExitCode == 0
                    ? "Python detected"
                    : "Python check failed"
            };
        }
        catch
        {
            return new DiagnosticResult
            {
                Name = "Python",
                IsInstalled = false,
                Message = "Python was not found"
            };
        }
    }
}