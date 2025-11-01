using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using TicketTracker.Api.Data;
using TicketTracker.Api.Models.Entities;
using TicketTracker.Api.Models.Requests;
using TicketTracker.Api.Models.Responses;
using TicketTracker.IntegrationTests.Infrastructure;

namespace TicketTracker.IntegrationTests.Controllers;

public class AuthControllerTests : IntegrationTest
{
    public AuthControllerTests(TestWebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsOk()
    {
        // Arrange
        var request = new RegisterRequest(
            "testuser",
            "Test@123",
            null // or provide an int[] if needed
        );

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123")
        };

        using (var scope = Factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<TicketTrackerContext>();
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }

        var request = new LoginRequest("testuser", "Test@123");

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var token = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(token?.Token);
    }
}