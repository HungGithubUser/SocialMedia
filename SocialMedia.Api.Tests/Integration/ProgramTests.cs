using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace SocialMedia.Api.Tests.Integration;

[TestClass]
public class ProgramTests
{
    private static WebApplicationFactory<Program> _factory = null!;

    [ClassInitialize]
    public static void ClassInit(TestContext testContext)
    {
        _factory = new WebApplicationFactory<Program>();
    }

    [TestMethod]
    public void WebApp_Should_CreateClientSuccessfully()
    {
        // Arrange
        _factory.CreateClient();
    }
}