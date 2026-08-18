
using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace API.Tests;

public class ProductControllerTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProductControllerTests(
        WebApplicationFactory<Program> factory)
    {
        var testFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    "Test",
                    options => { });
            });
        });

        _client = testFactory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_AuthenticatedUser_ReturnsOk()
    {
        var response = await _client.GetAsync(
            "/api/v1/Product?pageNumber=1&pageSize=10");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_UserRole_ReturnsForbidden()
    {
        var request = new
        {
            ProductName = "Test Product"
        };

        var json = System.Text.Json.JsonSerializer.Serialize(request);

        using var content = new StringContent(
            json,
            System.Text.Encoding.UTF8,
            "application/json");

        var response = await _client.PostAsync(
            "/api/v1/Product",
            content);

        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }
}
