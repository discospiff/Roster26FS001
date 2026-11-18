using Microsoft.Extensions.Hosting;
using Roster26FS001.Services;

namespace Roster26FS001.Tests.Services;

[TestClass]
public sealed class RosterStoreSelectorTests
{
    [TestMethod]
    public void Select_UsesInMemoryOutsideAzureEvenWhenConfigurationExists()
    {
        var environment = new TestHostEnvironment { EnvironmentName = Environments.Development };

        var mode = new RosterStoreSelector().Select(environment, "Server=host;Port=1234");

        Assert.AreEqual(StorageMode.InMemory, mode);
    }

    [TestMethod]
    public void Select_UsesAzureWhenAzureEnvironmentAndConfigurationExist()
    {
        var environment = new TestHostEnvironment { EnvironmentName = "Azure" };

        var mode = new RosterStoreSelector().Select(environment, "Server=host;Port=1234");

        Assert.AreEqual(StorageMode.AzureMySqlInApp, mode);
    }

    private sealed class TestHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = string.Empty;
        public string ApplicationName { get; set; } = "Roster26FS001";
        public string ContentRootPath { get; set; } = string.Empty;
        public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; } = null!;
    }
}