using DevVault.Models;

namespace DevVault.Services;

public class ProjectDiscoveryService
{
    public List<ProjectInfo> Discover(string rootPath)
    {
        var projects = new List<ProjectInfo>();

        DiscoverDotNetProjects(rootPath, projects);
        DiscoverNodeProjects(rootPath, projects);
        DiscoverPythonProjects(rootPath, projects);

        return projects;
    }

    // ==============================
    // .NET PROJECTS
    // ==============================

    private static void DiscoverDotNetProjects(
        string rootPath,
        List<ProjectInfo> projects)
    {
        var projectFiles = DirectoryScanner.GetFiles(
            rootPath,
            "*.csproj");

        foreach (var projectFile in projectFiles)
        {
            var directory = Path.GetDirectoryName(projectFile);

            if (directory == null)
                continue;

            projects.Add(new ProjectInfo
            {
                Name = Path.GetFileNameWithoutExtension(projectFile),
                Path = directory,
                Type = "C# / .NET",
                Framework = DetectDotNetFramework(projectFile)
            });
        }
    }

    private static string DetectDotNetFramework(string projectFile)
    {
        string content = File.ReadAllText(projectFile);

        if (content.Contains("net10.0"))
            return ".NET 10";

        if (content.Contains("net9.0"))
            return ".NET 9";

        if (content.Contains("net8.0"))
            return ".NET 8";

        if (content.Contains("net7.0"))
            return ".NET 7";

        if (content.Contains("net6.0"))
            return ".NET 6";

        return "Unknown .NET version";
    }

    // ==============================
    // NODE.JS PROJECTS
    // ==============================

    private static void DiscoverNodeProjects(
        string rootPath,
        List<ProjectInfo> projects)
    {
        var packageFiles = DirectoryScanner.GetFiles(
            rootPath,
            "package.json");

        foreach (var packageFile in packageFiles)
        {
            var directory = Path.GetDirectoryName(packageFile);

            if (directory == null)
                continue;

            string content = File.ReadAllText(packageFile);

            string framework = DetectNodeFramework(content);

            projects.Add(new ProjectInfo
            {
                Name = Path.GetFileName(directory),
                Path = directory,
                Type = "Node.js / JavaScript",
                Framework = framework
            });
        }
    }

    private static string DetectNodeFramework(string content)
    {
        if (content.Contains(
            "\"next\"",
            StringComparison.OrdinalIgnoreCase))
            return "Next.js";

        if (content.Contains(
            "\"react\"",
            StringComparison.OrdinalIgnoreCase))
            return "React";

        if (content.Contains(
            "\"express\"",
            StringComparison.OrdinalIgnoreCase))
            return "Express.js";

        if (content.Contains(
            "\"vue\"",
            StringComparison.OrdinalIgnoreCase))
            return "Vue.js";

        return "Node.js";
    }

    // ==============================
    // PYTHON PROJECTS
    // ==============================

    private static void DiscoverPythonProjects(
        string rootPath,
        List<ProjectInfo> projects)
    {
        string[] markers =
        {
            "requirements.txt",
            "pyproject.toml",
            "Pipfile",
            "setup.py",
            "setup.cfg"
        };

        var directories = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase);

        foreach (var marker in markers)
        {
            var files = DirectoryScanner.GetFiles(
                rootPath,
                marker);

            foreach (var file in files)
            {
                var directory = Path.GetDirectoryName(file);

                if (directory == null)
                    continue;

                // Prevent the same Python project
                // from being discovered multiple times.
                if (!directories.Add(directory))
                    continue;

                string framework = DetectPythonFramework(
                    directory);

                projects.Add(new ProjectInfo
                {
                    Name = Path.GetFileName(directory),
                    Path = directory,
                    Type = "Python",
                    Framework = framework
                });
            }
        }
    }

    private static string DetectPythonFramework(
        string projectPath)
    {
        string[] files =
        {
            "requirements.txt",
            "pyproject.toml",
            "Pipfile",
            "setup.py",
            "setup.cfg"
        };

        foreach (var file in files)
        {
            string path = Path.Combine(
                projectPath,
                file);

            if (!File.Exists(path))
                continue;

            string content = File.ReadAllText(path);

            if (content.Contains(
                "fastapi",
                StringComparison.OrdinalIgnoreCase))
                return "FastAPI";

            if (content.Contains(
                "django",
                StringComparison.OrdinalIgnoreCase))
                return "Django";

            if (content.Contains(
                "flask",
                StringComparison.OrdinalIgnoreCase))
                return "Flask";
        }

        return "Python";
    }
}