using DevVault.Models;

namespace DevVault.Services;

public class PythonProjectAnalyzer : IProjectAnalyzer
{
    public bool CanAnalyze(string projectPath)
    {
        return HasPythonProjectFile(projectPath);
    }

    public ProjectAnalysis Analyze(string projectPath)
    {
        return new ProjectAnalysis
        {
            ProjectPath = projectPath,
            ProjectType = "Python",
            Framework = DetectFramework(projectPath),
            HasGit = Directory.Exists(
                Path.Combine(projectPath, ".git")),
            HasDockerfile = File.Exists(
                Path.Combine(projectPath, "Dockerfile")),
            HasEnvironmentFile = File.Exists(
                Path.Combine(projectPath, ".env")),
            HasRequirementsFile = File.Exists(
    Path.Combine(projectPath, "requirements.txt")),
HasPyProjectFile = File.Exists(
    Path.Combine(projectPath, "pyproject.toml")),
HasPackageJson = false,
            HasSolutionFile = false,
            HasProjectFile = false
        };
    }

    private static bool HasPythonProjectFile(string projectPath)
    {
        string[] markers =
        {
            "requirements.txt",
            "pyproject.toml",
            "Pipfile",
            "setup.py",
            "setup.cfg"
        };

        return markers.Any(marker =>
            File.Exists(Path.Combine(projectPath, marker)));
    }

    private static string DetectFramework(string projectPath)
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
            string path = Path.Combine(projectPath, file);

            if (!File.Exists(path))
                continue;

            string content = File.ReadAllText(path);

            if (content.Contains("fastapi",
                StringComparison.OrdinalIgnoreCase))
                return "FastAPI";

            if (content.Contains("django",
                StringComparison.OrdinalIgnoreCase))
                return "Django";

            if (content.Contains("flask",
                StringComparison.OrdinalIgnoreCase))
                return "Flask";
        }

        return "Python";
    }
}