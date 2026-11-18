using Microsoft.Extensions.Configuration;
using Roster26FS001.Services;

namespace Roster26FS001.Tests.Services;

[TestClass]
public sealed class AzureMySqlConnectionSourceTests
{
    [TestMethod]
    public void Resolve_PrefersConfiguredConnectionString()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:localdb"] = "Server=config-host;Port=1"
            })
            .Build();

        var result = new AzureMySqlConnectionSource().Resolve(
            configuration,
            "connection.ini",
            _ => true,
            _ => "Server=file-host;Port=2");

        StringAssert.Contains(result!, "config-host");
    }

    [TestMethod]
    public void Resolve_ReadsRawConnectionStringFile()
    {
        var configuration = new ConfigurationBuilder().Build();

        var result = new AzureMySqlConnectionSource().Resolve(
            configuration,
            "connection.ini",
            _ => true,
            _ => "Server=file-host;Port=53381;Database=roster");

        Assert.AreEqual("Server=file-host;Port=53381;Database=roster", result);
    }

    [TestMethod]
    public void Resolve_ReadsNamedIniAssignment()
    {
        var configuration = new ConfigurationBuilder().Build();

        var result = new AzureMySqlConnectionSource().Resolve(
            configuration,
            "connection.ini",
            _ => true,
            _ => "MYSQLCONNSTR_localdb=\"Server=file-host;Port=53381;Database=roster\"");

        Assert.AreEqual("Server=file-host;Port=53381;Database=roster", result);
    }

    [TestMethod]
    public void Resolve_ReturnsNullWhenFileIsMissing()
    {
        var configuration = new ConfigurationBuilder().Build();

        var result = new AzureMySqlConnectionSource().Resolve(
            configuration,
            "missing.ini",
            _ => false,
            _ => throw new AssertFailedException("The file should not be read."));

        Assert.IsNull(result);
    }
}
