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

    [TestMethod]
    public void Normalize_SupportsAzureMySqlAliases()
    {
        var connection = new AzureInAppConnectionStringNormalizer().Normalize(
            "Data Source=azure-host;Port=43817;Initial Catalog=roster;Uid=app;Pwd=secret");

        Assert.AreEqual("azure-host", connection.Host);
        Assert.AreEqual((uint)43817, connection.Port);
        Assert.AreEqual("roster", connection.Database);
        Assert.AreEqual("app", connection.UserId);
    }

    [TestMethod]
    public void Normalize_IdentifiesMissingNonSecretField()
    {
        var exception = Assert.ThrowsExactly<InvalidOperationException>(() => new AzureInAppConnectionStringNormalizer().Normalize(
            "Data Source=azure-host;Port=43817;Uid=app;Pwd=secret"));

        StringAssert.Contains(exception.Message, "database");
        Assert.DoesNotContain("secret", exception.Message);
    }

    [TestMethod]
    public void Normalize_UsesPortEmbeddedInHostWhenConnectionStringOmitsPort()
    {
        var connection = new AzureInAppConnectionStringNormalizer().Normalize(
            "Server=127.0.0.1:53381;Database=roster;User ID=app;Password=secret");

        Assert.AreEqual((uint)53381, connection.Port);
        Assert.AreEqual("127.0.0.1", connection.Host);
    }

    [TestMethod]
    public void Normalize_ExplicitConnectionStringPortTakesPrecedenceOverEmbeddedHostPort()
    {
        var connection = new AzureInAppConnectionStringNormalizer().Normalize(
            "Server=127.0.0.1:53381;Port=43817;Database=roster;User ID=app;Password=secret");

        Assert.AreEqual((uint)43817, connection.Port);
        Assert.AreEqual("127.0.0.1:53381", connection.Host);
    }

    [TestMethod]
    public void Normalize_RejectsInvalidConnectionStringPort()
    {
        var exception = Assert.ThrowsExactly<InvalidOperationException>(() => new AzureInAppConnectionStringNormalizer().Normalize(
            "Server=azure-host;Port=not-a-port;Database=roster;User ID=app;Password=secret"));

        StringAssert.Contains(exception.Message, "invalid port");
        Assert.DoesNotContain("secret", exception.Message);
    }

    [TestMethod]
    public void Normalize_ExposesOnlySafeEndpointMetadata()
    {
        var connection = new AzureInAppConnectionStringNormalizer().Normalize(
            "Server=127.0.0.1;Port=53381;Database=roster;User ID=app-user;Password=secret-value");

        Assert.AreEqual("127.0.0.1:53381", connection.SafeEndpoint);
        Assert.DoesNotContain("app-user", connection.SafeEndpoint);
        Assert.DoesNotContain("secret-value", connection.SafeEndpoint);
    }
}