using DevVault.Services;

namespace DevVault.Tests;

public class UnitTest1
{
    [Fact]
    public void Discover_ShouldFindDevVaultProjects()
    {
        var service = new ProjectDiscoveryService();

        string projectPath = Directory.GetCurrentDirectory();

        var projects = service.Discover(projectPath);

        Assert.NotNull(projects);
    }
}