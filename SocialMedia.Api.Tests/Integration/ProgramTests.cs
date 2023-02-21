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
    public async Task WebApp_Should_CreateClientSuccessfully()
    {
        // Arrange
        var client = _factory.CreateClient();
        // Act
        var response = await client.GetAsync("");
        // Assert   
        response.EnsureSuccessStatusCode();
        Assert.AreEqual("text/html", 
            response.Content.Headers.ContentType?.ToString());
    }
}