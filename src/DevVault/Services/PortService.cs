
using System.Diagnostics;

namespace DevVault.Services;

public class PortService
{
    public void ShowListeningPorts()
    {
        var process = new Process();

        process.StartInfo.FileName = "netstat";
        process.StartInfo.Arguments = "-ano";
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;

        process.Start();

        string output = process.StandardOutput.ReadToEnd();

        process.WaitForExit();

        Console.WriteLine();
        Console.WriteLine("DevVault Port Inspector");
        Console.WriteLine("======================================");
        Console.WriteLine();

        Console.WriteLine("LISTENING PORTS");
        Console.WriteLine();

        Console.WriteLine(
            $"{ "PORT",-8} { "PID",-8} PROCESS");

        Console.WriteLine(
            $"{ "----",-8} { "---",-8} -------");

        var lines = output.Split(
            Environment.NewLine,
            StringSplitOptions.RemoveEmptyEntries);

        var displayedPorts = new HashSet<string>();

        foreach (var line in lines)
        {
            if (!line.Contains("LISTENING"))
                continue;

            var parts = line
                .Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 5)
                continue;

            string localAddress = parts[1];
            string pid = parts[^1];

            string port = ExtractPort(localAddress);

            if (string.IsNullOrWhiteSpace(port))
                continue;

            if (!displayedPorts.Add(port))
                continue;

            string processName = GetProcessName(pid);

            Console.WriteLine(
                $"{port,-8} {pid,-8} {processName}");
        }

        Console.WriteLine();
    }

    public void CheckPort(int port)
    {
        var process = new Process();

        process.StartInfo.FileName = "netstat";
        process.StartInfo.Arguments = "-ano";
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;

        process.Start();

        string output = process.StandardOutput.ReadToEnd();

        process.WaitForExit();

        var lines = output.Split(
            Environment.NewLine,
            StringSplitOptions.RemoveEmptyEntries);

        string? foundPid = null;

        foreach (var line in lines)
        {
            if (!line.Contains("LISTENING"))
                continue;

            var parts = line.Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 5)
                continue;

            string localAddress = parts[1];
            string currentPort = ExtractPort(localAddress);

            if (currentPort == port.ToString())
            {
                foundPid = parts[^1];
                break;
            }
        }

        Console.WriteLine();
        Console.WriteLine("DevVault Port Inspector");
        Console.WriteLine("======================================");
        Console.WriteLine();

        if (foundPid == null)
        {
            Console.WriteLine($"Port {port} is available.");
        }
        else
        {
            string processName = GetProcessName(foundPid);

            Console.WriteLine($"Port {port} is currently in use.");
            Console.WriteLine($"PID: {foundPid}");
            Console.WriteLine($"Process: {processName}");
        }

        Console.WriteLine();
    }

    private static string ExtractPort(string address)
    {
        int lastColon = address.LastIndexOf(':');

        if (lastColon == -1)
            return string.Empty;

        return address[(lastColon + 1)..];
    }

    private static string GetProcessName(string pid)
    {
        if (!int.TryParse(pid, out int processId))
            return "Unknown";

        try
        {
            using var process = Process.GetProcessById(processId);

            return process.ProcessName;
        }
        catch
        {
            return "Unknown";
        }
    }
}
