using System.Net;
using System.Reflection;
using System.Web;
using Microsoft.Extensions.Configuration;

namespace SocialMedia.Infrastructure.Tests;

[TestClass]
public class FaceBookApiTests
{
    private const string VideoLinkType1 = "https://www.facebook.com/anhchefvn/videos/518436523503472";
    private const string VideoLinkType2 = "https://www.facebook.com/watch/?v=518436523503472";
    private const string FacebookGraph = "https://graph.facebook.com/v16.0/";
    private const string CookieDelimiter = ";";


    [TestMethod]
    public void FacebookToken_ShouldExists()
    {
        var actual = GetFacebookToken();
        Assert.IsFalse(string.IsNullOrEmpty(actual));
    }

    [TestMethod]
    public void FacebookCookie_ShouldExists()
    {
        var actual = GetFacebookCookies();
        Assert.IsFalse(string.IsNullOrEmpty(actual));
    }

    [DataTestMethod]
    [DataRow(VideoLinkType1)]
    [DataRow(VideoLinkType2)]
    public void GetFacebookVideoId_Success(string link)
    {
        var id = GetFacebookVideoId(link);
        Assert.AreEqual("518436523503472", id);
    }

    [DataTestMethod]
    [DataRow(VideoLinkType1)]
    [DataRow(VideoLinkType2)]
    public async Task GetFacebookLike_Success(string link)
    {
        var baseAddress = new Uri(FacebookGraph+GetFacebookVideoId(link)+$"?access_token={GetFacebookToken()}");
        var cookieContainer = new CookieContainer();
        using var handler = new HttpClientHandler { CookieContainer = cookieContainer };
        using var client = new HttpClient(handler) { BaseAddress = baseAddress };
        foreach (var cookie in GetFacebookCookies().Split(CookieDelimiter))
        {
            cookieContainer.SetCookies(baseAddress, cookie);
        }

        var result = await client.GetAsync(baseAddress);
        result.EnsureSuccessStatusCode();
    }


    private static string GetFacebookVideoId(string fullLink)
    {
        var uri = new Uri(fullLink);
        var query = uri.Query;
        return (string.IsNullOrEmpty(query)
                   ? uri.Segments.Last()
                   : HttpUtility.ParseQueryString(query)["v"])
               ?? throw new InvalidOperationException();
    }

    private static string GetFacebookToken()
    {
        var r = GetConfigurationRoot();
        return r.GetSection("FaceBook:Token").Value ?? throw new SecretUnsetException();
    }

    private static string GetFacebookCookies()
    {
        var r = GetConfigurationRoot();
        return r.GetSection("FaceBook:Cookie").Value ?? throw new SecretUnsetException();
    }

    private static IConfigurationRoot GetConfigurationRoot()
    {
        var builder = new ConfigurationBuilder()
            .AddUserSecrets(Assembly.GetExecutingAssembly());
        var root = builder.Build();
        return root;
    }
}

internal class SecretUnsetException : Exception
{
}