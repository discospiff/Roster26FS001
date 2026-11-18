using Roster26FS001.Services;

namespace Roster26FS001.Tests.Services;

[TestClass]
public sealed class AzureInAppConnectionStringNormalizerTests
{
    [TestMethod]
    public void Normalize_PreservesGeneratedPort()
    {
        var connection = new AzureInAppConnectionStringNormalizer().Normalize(
            "Server=azure-host;Port=43817;Database=roster;User ID=app;Password=secret");

        Assert.AreEqual("azure-host", connection.Host);
        Assert.AreEqual((uint)43817, connection.Port);
        StringAssert.Contains(connection.ToConnectionString(), "Port=43817");
    }

    [TestMethod]
    public void Normalize_RejectsMissingPort()
    {
        Assert.ThrowsExactly<InvalidOperationException>(() => new AzureInAppConnectionStringNormalizer().Normalize(
            "Server=azure-host;Database=roster;User ID=app;Password=secret"));
    }

    [TestMethod]
    public void Normalize_DoesNotExposePasswordInConfigurationErrors()
    {
        var exception = Assert.ThrowsExactly<InvalidOperationException>(() => new AzureInAppConnectionStringNormalizer().Normalize(
            "Server=azure-host;Port=not-a-port;Database=roster;User ID=app;Password=secret-value"));

        Assert.DoesNotContain("secret-value", exception.Message);
    }
}