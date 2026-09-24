using DevVault.Models;

namespace DevVault.Services;

public class ProjectAnalyzerManager
{
    private readonly List<IProjectAnalyzer> _analyzers;

    public ProjectAnalyzerManager()
    {
        _analyzers = new List<IProjectAnalyzer>
        {
            new ProjectAnalyzerService(),
            new NodeProjectAnalyzer(),
            new PythonProjectAnalyzer()
        };
    }

    public ProjectAnalysis Analyze(string projectPath)
    {
        var analysis = new ProjectAnalysis
        {
            ProjectPath = projectPath,
            ProjectType = "Unknown",
            Framework = "Unknown"
        };

        foreach (var analyzer in _analyzers)
        {
            if (!analyzer.CanAnalyze(projectPath))
                continue;

            var result = analyzer.Analyze(projectPath);

            if (!string.IsNullOrWhiteSpace(result.ProjectType))
            {
                analysis.ProjectTypes.Add(
                    result.ProjectType);
            }

            if (analysis.ProjectType == "Unknown")
            {
                analysis.ProjectType =
                    result.ProjectType;

                analysis.Framework =
                    result.Framework;
            }

            analysis.HasGit |= result.HasGit;

            analysis.HasDockerfile |=
                result.HasDockerfile;

            analysis.HasEnvironmentFile |=
                result.HasEnvironmentFile;

            analysis.HasSolutionFile |=
                result.HasSolutionFile;

            analysis.HasProjectFile |=
                result.HasProjectFile;

            analysis.HasPackageJson |=
                result.HasPackageJson;

            analysis.HasRequirementsFile |=
                result.HasRequirementsFile;

            analysis.HasPyProjectFile |=
                result.HasPyProjectFile;
        }

        return analysis;
    }
}