using System.Text.Json;
using System.Xml.Linq;
using DevVault.Models;

namespace DevVault.Services;

public class DependencyService
{
    public List<ProjectDependency> GetDependencies(
        string projectPath)
    {
        var dependencies = new List<ProjectDependency>();

        GetDotNetDependencies(
            projectPath,
            dependencies);

        GetNodeDependencies(
            projectPath,
            dependencies);

        GetPythonDependencies(
            projectPath,
            dependencies);

        MarkDuplicates(dependencies);

        return dependencies;
    }

    // ==============================
    // .NET / NUGET
    // ==============================

    private static void GetDotNetDependencies(
        string projectPath,
        List<ProjectDependency> dependencies)
    {
        var projectFiles = DirectoryScanner.GetFiles(
            projectPath,
            "*.csproj");

        foreach (var projectFile in projectFiles)
        {
            var document = XDocument.Load(projectFile);

            var packageReferences =
                document.Descendants("PackageReference");

            foreach (var package in packageReferences)
            {
                var name =
                    package.Attribute("Include")?.Value;

                if (string.IsNullOrWhiteSpace(name))
                    continue;

                var version =
                    package.Attribute("Version")?.Value
                    ?? "Version managed elsewhere";

                dependencies.Add(
                    new ProjectDependency
                    {
                        Name = name,
                        Version = version,
                        Ecosystem = "NuGet",
                        Source = Path.GetFileName(projectFile)
                    });
            }
        }
    }

    // ==============================
    // NODE.JS / NPM
    // ==============================

    private static void GetNodeDependencies(
        string projectPath,
        List<ProjectDependency> dependencies)
    {
        var packageFiles = DirectoryScanner.GetFiles(
            projectPath,
            "package.json");

        foreach (var packageFile in packageFiles)
        {
            try
            {
                string json =
                    File.ReadAllText(packageFile);

                using var document =
                    JsonDocument.Parse(json);

                ReadNpmDependencyGroup(
                    document.RootElement,
                    "dependencies",
                    dependencies,
                    packageFile);

                ReadNpmDependencyGroup(
                    document.RootElement,
                    "devDependencies",
                    dependencies,
                    packageFile);
            }
            catch
            {
                // Ignore invalid package.json files.
            }
        }
    }

    private static void ReadNpmDependencyGroup(
        JsonElement root,
        string propertyName,
        List<ProjectDependency> dependencies,
        string packageFile)
    {
        if (!root.TryGetProperty(
            propertyName,
            out var dependencyObject))
        {
            return;
        }

        foreach (var dependency in
            dependencyObject.EnumerateObject())
        {
            dependencies.Add(
                new ProjectDependency
                {
                    Name = dependency.Name,
                    Version =
                        dependency.Value.GetString()
                        ?? "Unknown",
                    Ecosystem = "npm",
                    Source =
                        $"{Path.GetFileName(packageFile)}:{propertyName}"
                });
        }
    }

    // ==============================
    // PYTHON
    // ==============================

    private static void GetPythonDependencies(
        string projectPath,
        List<ProjectDependency> dependencies)
    {
        GetRequirementsDependencies(
            projectPath,
            dependencies);

        GetPyProjectDependencies(
            projectPath,
            dependencies);
    }

    // ==============================
    // REQUIREMENTS.TXT
    // ==============================

    private static void GetRequirementsDependencies(
        string projectPath,
        List<ProjectDependency> dependencies)
    {
        var requirementFiles =
            DirectoryScanner.GetFiles(
                projectPath,
                "requirements.txt");

        foreach (var requirementFile in requirementFiles)
        {
            var lines =
                File.ReadAllLines(requirementFile);

            foreach (var line in lines)
            {
                string dependency = line.Trim();

                if (string.IsNullOrWhiteSpace(dependency))
                    continue;

                if (dependency.StartsWith("#"))
                    continue;

                string name = dependency;
                string version = "Unknown";

                int separator =
                    dependency.IndexOf(
                        "==",
                        StringComparison.Ordinal);

                if (separator >= 0)
                {
                    name = dependency[..separator]
                        .Trim();

                    version =
                        dependency[(separator + 2)..]
                        .Trim();
                }

                dependencies.Add(
                    new ProjectDependency
                    {
                        Name = name,
                        Version = version,
                        Ecosystem = "PyPI",
                        Source =
                            Path.GetFileName(
                                requirementFile)
                    });
            }
        }
    }

    // ==============================
    // PYPROJECT.TOML
    // ==============================

    private static void GetPyProjectDependencies(
        string projectPath,
        List<ProjectDependency> dependencies)
    {
        var pyprojectFiles =
            DirectoryScanner.GetFiles(
                projectPath,
                "pyproject.toml");

        foreach (var pyprojectFile in pyprojectFiles)
        {
            var lines =
                File.ReadAllLines(pyprojectFile);

            bool insideDependencies = false;

            foreach (var line in lines)
            {
                string trimmed = line.Trim();

                if (trimmed.Equals(
                    "dependencies = [",
                    StringComparison.OrdinalIgnoreCase))
                {
                    insideDependencies = true;
                    continue;
                }

                if (!insideDependencies)
                    continue;

                if (trimmed == "]")
                {
                    insideDependencies = false;
                    continue;
                }

                if (!trimmed.StartsWith("\""))
                    continue;

                string dependency =
                    trimmed.TrimEnd(',');

                dependency =
                    dependency.Trim('"');

                string name = dependency;
                string version = "Unknown";

                int separator =
                    dependency.IndexOfAny(
                        new[] { '=', '>', '<', '!' });

                if (separator >= 0)
                {
                    name = dependency[..separator]
                        .Trim();

                    version =
                        dependency[separator..]
                        .Trim();
                }

                dependencies.Add(
                    new ProjectDependency
                    {
                        Name = name,
                        Version = version,
                        Ecosystem = "PyPI",
                        Source =
                            Path.GetFileName(
                                pyprojectFile)
                    });
            }
        }
    }

    // ==============================
    // DUPLICATE DETECTION
    // ==============================

    private static void MarkDuplicates(
        List<ProjectDependency> dependencies)
    {
        var groups = dependencies
            .GroupBy(
                dependency =>
                    $"{dependency.Ecosystem}:{dependency.Name}",
                StringComparer.OrdinalIgnoreCase);

        foreach (var group in groups)
        {
            if (group.Count() <= 1)
                continue;

            foreach (var dependency in group)
            {
                dependency.Status = "Duplicate";
            }
        }
    }
}