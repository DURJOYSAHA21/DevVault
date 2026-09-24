using DevVault.Models;

namespace DevVault.Services;

public interface IProjectAnalyzer
{
    bool CanAnalyze(string projectPath);

    ProjectAnalysis Analyze(string projectPath);
}