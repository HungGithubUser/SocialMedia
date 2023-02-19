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
    public async Task Nothing()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("");
        
        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        // Assert.Equals("text/html; charset=utf-8", 
        //     response.Content.Headers.ContentType?.ToString());
    }
}