using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace SocialMedia.Infrastructure.Tests;

[TestClass]
public class FaceBookApiTests
{
    private static readonly HttpClient Client = new HttpClient();
    private readonly string _token;
    private readonly string _cookie;

    public FaceBookApiTests()
    {
        _token = GetFacebookToken()!;
        _cookie = GetFacebookCookie()!;
    }

    [TestMethod]
    public void FacebookToken_ShouldExists()
    {
        var actual = GetFacebookToken();
        Assert.IsFalse(string.IsNullOrEmpty(actual));
    }

    [TestMethod]
    public void FacebookCookie_ShouldExists()
    {
        var actual = GetFacebookCookie();
        Assert.IsFalse(string.IsNullOrEmpty(actual));
    }

    [DataTestMethod]
    [DataRow("https://www.facebook.com/anhchefvn/videos/518436523503472")]
    public void GetLikes_Success(string link)
    {
        // Client.
    }

    private static string? GetFacebookToken()
    {
        var r = GetConfigurationRoot();
        return r.GetSection("FaceBook:Token").Value;
    }
    
    private static string? GetFacebookCookie()
    {
        var r = GetConfigurationRoot();
        return r.GetSection("FaceBook:Cookie").Value;
    }

    private static IConfigurationRoot GetConfigurationRoot()
    {
        var builder = new ConfigurationBuilder().AddUserSecrets(Assembly.GetExecutingAssembly());
        var root = builder.Build();
        return root;
    }
}