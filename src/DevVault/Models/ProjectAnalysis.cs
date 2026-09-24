namespace DevVault.Models;

public class ProjectAnalysis
{
    public string ProjectPath { get; set; } = string.Empty;

    public string ProjectType { get; set; } = string.Empty;

    public string Framework { get; set; } = string.Empty;

    public List<string> ProjectTypes { get; set; } = new();

    public bool HasGit { get; set; }

    public bool HasDockerfile { get; set; }

    public bool HasEnvironmentFile { get; set; }

    public bool HasSolutionFile { get; set; }

    public bool HasProjectFile { get; set; }

    public bool HasPackageJson { get; set; }

    public bool HasRequirementsFile { get; set; }

    public bool HasPyProjectFile { get; set; }
}