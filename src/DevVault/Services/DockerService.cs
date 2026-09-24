using System.Diagnostics;
using DevVault.Models;

namespace DevVault.Services;

public class DockerService
{
    public DiagnosticResult CheckDocker()
    {
        try
        {
            var process = new Process();

            process.StartInfo.FileName = "docker";
            process.StartInfo.Arguments = "--version";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            string output = process.StandardOutput.ReadToEnd().Trim();

            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                return new DiagnosticResult
                {
                    Name = "Docker",
                    IsInstalled = false,
                    Message = "Docker command failed"
                };
            }

            return new DiagnosticResult
            {
                Name = "Docker",
                IsInstalled = true,
                Version = ExtractVersion(output),
                Message = "Docker CLI detected"
            };
        }
        catch
        {
            return new DiagnosticResult
            {
                Name = "Docker",
                IsInstalled = false,
                Message = "Docker CLI was not found in PATH"
            };
        }
    }

    private static string ExtractVersion(string output)
    {
        const string prefix = "Docker version ";

        if (output.StartsWith(prefix))
        {
            return output[prefix.Length..]
                .Split(',')[0]
                .Trim();
        }

        return output;
    }
}