using DevVault.Models;

namespace DevVault.Services;

public class NodeProjectAnalyzer : IProjectAnalyzer
{
    public bool CanAnalyze(string projectPath)
    {
        return File.Exists(
            Path.Combine(projectPath, "package.json"));
    }

    public ProjectAnalysis Analyze(string projectPath)
    {
        return new ProjectAnalysis
        {
            ProjectPath = projectPath,
            ProjectType = "Node.js / JavaScript",
            Framework = DetectFramework(projectPath),
            HasGit = Directory.Exists(
                Path.Combine(projectPath, ".git")),
            HasDockerfile = File.Exists(
                Path.Combine(projectPath, "Dockerfile")),
            HasEnvironmentFile = File.Exists(
                Path.Combine(projectPath, ".env")),
            HasPackageJson = true,
            HasSolutionFile = false,
            HasProjectFile = false,
            HasRequirementsFile = false
        };
    }

    private static string DetectFramework(string projectPath)
    {
        var packageJsonPath = Path.Combine(
            projectPath,
            "package.json");

        string content = File.ReadAllText(packageJsonPath);

        if (content.Contains("\"next\""))
            return "Next.js";

        if (content.Contains("\"react\""))
            return "React";

        if (content.Contains("\"express\""))
            return "Express.js";

        if (content.Contains("\"vue\""))
            return "Vue.js";

        return "Node.js";
    }
}