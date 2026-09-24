using System.Diagnostics;
using DevVault.Models;

namespace DevVault.Services;

public class NodeService
{
    public DiagnosticResult CheckNode()
    {
        try
        {
            var process = new Process();

            process.StartInfo.FileName = "node";
            process.StartInfo.Arguments = "--version";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            string version =
                process.StandardOutput
                    .ReadToEnd()
                    .Trim();

            process.WaitForExit();

            return new DiagnosticResult
            {
                Name = "Node.js",
                IsInstalled = process.ExitCode == 0,
                Version = version,
                Message = process.ExitCode == 0
                    ? "Node.js detected"
                    : "Node.js check failed"
            };
        }
        catch
        {
            return new DiagnosticResult
            {
                Name = "Node.js",
                IsInstalled = false,
                Message = "Node.js was not found"
            };
        }
    }
}