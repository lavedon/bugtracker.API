using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.Common;

using BugTracker.Data;
using BugTracker.Models;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BugTracker.Tests;

public class NoAuthTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public NoAuthTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    // Test endpoints we should not be able to access unless signed in
    [Theory]
    [InlineData("/login")]
    [InlineData("/createproject")]
    [InlineData("/createticket")]
    [InlineData("/deleteproject/testProject1")]
    [InlineData("/updateproject/testProject1")]

    public async void Get_EndPointsReturnForbidden(string url)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(url);

        // Assert that the response was a forbidden
        Assert.Throws<HttpRequestException>(() => response.EnsureSuccessStatusCode());
        Assert.Contains("MethodNotAllowed", response.StatusCode.ToString());

    }

    [Theory]
    [InlineData("/getallusers")]
    [InlineData("/getallprojects")]
    [InlineData("/getprojectbyid/5")]
    [InlineData("/getprojectbyname/testProject1")]
    public async void Get_EndPointsReturnNotAuthorized(string url)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(url);

        // Assert that the response was a forbidden
        Assert.Throws<HttpRequestException>(() => response.EnsureSuccessStatusCode());
        Assert.Contains("Unauthorized", response.StatusCode.ToString());

    }
}

// Create a custom WebApplicationFactory<T> that removes the default database and uses a custom in memory database
public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbContextOptions<AppDbContext>));
            
            services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDb");
            });

            var sp = services.BuildServiceProvider();
            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AppDbContext>();
                var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();
                try {

                db.Database.EnsureCreated();
                db.Users.Add(new User{
                    Id = 1,
                    Username = "testUser1",
                    Password = "testPassword1",
                    Role = "Admin"
                });

                db.SaveChanges();
                } 
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred seeding the database. Error: {message}", ex.Message);
                }
            }
        });
    }
}
  // Test that user can login via the /login endpoint with valid credentials
 //   [Fact(DisplayName = "User can Login")]
 public class LoginTests : IClassFixture<CustomWebApplicationFactory<Program>>
 {
     private readonly CustomWebApplicationFactory<Program> _factory;

     public LoginTests(CustomWebApplicationFactory<Program> factory)
     {
         _factory = factory;
     }

    [Fact(DisplayName = "Test the Login endpoint")]
    public async void UserCanLogin()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = client.PostAsync("/login", new StringContent(
            "{\"username\": \"testUser1\", \"password\": \"testPassword1\"}", Encoding.UTF8, "application/json")).Result;

        // Assert
        Assert.True(response.IsSuccessStatusCode);
    }
 }

// Test user login with an in-memory database
public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    // @TODO mock the database with a test user so we can test login
    private readonly WebApplicationFactory<Program> _factory;

    public IntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async void WebApplicationTestPlatform_ShouldWork()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/");

        // Assert
        response.EnsureSuccessStatusCode();
        var stringResponse = await response.Content.ReadAsStringAsync();
        Assert.Contains("Bug Tracker", stringResponse);
    }

}