namespace DevVault.Services;

public static class DirectoryScanner
{
    private static readonly HashSet<string> ExcludedDirectories =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ".git",
            ".vs",
            "bin",
            "obj",
            "node_modules"
        };

    public static IEnumerable<string> GetFiles(
        string rootPath,
        string searchPattern)
    {
        return GetFilesRecursive(rootPath, searchPattern);
    }

    private static IEnumerable<string> GetFilesRecursive(
        string directory,
        string searchPattern)
    {
        foreach (var file in Directory.GetFiles(
            directory,
            searchPattern))
        {
            yield return file;
        }

        foreach (var subDirectory in Directory.GetDirectories(
            directory))
        {
            string directoryName =
                Path.GetFileName(subDirectory);

            if (ExcludedDirectories.Contains(directoryName))
                continue;

            foreach (var file in GetFilesRecursive(
                subDirectory,
                searchPattern))
            {
                yield return file;
            }
        }
    }
}