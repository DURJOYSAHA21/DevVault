using DevVault.Models;

namespace DevVault.Services;

public class ProjectAnalyzerService : IProjectAnalyzer
{
    public bool CanAnalyze(string projectPath)
    {
    	return Directory.GetFiles(
        	projectPath,
        	"*.csproj",
        	SearchOption.AllDirectories).Length > 0;
    }
    public ProjectAnalysis Analyze(string projectPath)
    {
        var analysis = new ProjectAnalysis
        {
            ProjectPath = projectPath,

            HasGit = Directory.Exists(
                Path.Combine(projectPath, ".git")),

            HasDockerfile = File.Exists(
                Path.Combine(projectPath, "Dockerfile")),

            HasEnvironmentFile = File.Exists(
                Path.Combine(projectPath, ".env")),

            HasPackageJson = File.Exists(
                Path.Combine(projectPath, "package.json")),

            HasRequirementsFile = File.Exists(
                Path.Combine(projectPath, "requirements.txt")),

            HasSolutionFile =
                Directory.GetFiles(
                    projectPath,
                    "*.sln",
                    SearchOption.AllDirectories).Length > 0
                ||
                Directory.GetFiles(
                    projectPath,
                    "*.slnx",
                    SearchOption.AllDirectories).Length > 0,

            HasProjectFile = Directory.GetFiles(
                projectPath,
                "*.csproj",
                SearchOption.AllDirectories).Length > 0
        };

        DetermineProjectType(analysis, projectPath);

        return analysis;
    }

    private static void DetermineProjectType(
        ProjectAnalysis analysis,
        string projectPath)
    {
        if (analysis.HasProjectFile)
        {
            analysis.ProjectType = "C# / .NET";
            analysis.Framework = DetectDotNetFramework(projectPath);
            return;
        }

        if (analysis.HasPackageJson)
        {
            analysis.ProjectType = "Node.js / JavaScript";
            return;
        }

        if (analysis.HasRequirementsFile)
        {
            analysis.ProjectType = "Python";
            return;
        }

        analysis.ProjectType = "Unknown";
    }

    private static string DetectDotNetFramework(
        string projectPath)
    {
        var projectFiles = Directory.GetFiles(
            projectPath,
            "*.csproj",
            SearchOption.AllDirectories);

        foreach (var file in projectFiles)
        {
            var content = File.ReadAllText(file);

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
        }

        return "Unknown .NET version";
    }
}