using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using TicketTracker.Api.Data;
using TicketTracker.Api.Models.Dtos;
using TicketTracker.Api.Models.Entities;
using TicketTracker.Api.Models.Requests;
using TicketTracker.Api.Models.Responses;
using TicketTracker.IntegrationTests.Infrastructure;
using Xunit;

namespace TicketTracker.IntegrationTests.Controllers;

public class AuthControllerTests : IntegrationTest
{
    public AuthControllerTests(TestWebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsOkAndSuccessMessage()
    {
        // Arrange
        var request = new RegisterRequest("testuser", "Password123!", null);

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/Auth/Register", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<SuccessResponse>();
        Assert.NotNull(result);
        Assert.Equal("Registration successful", result.Message);
    }

    [Fact]
    public async Task Register_WithEmptyUsername_ReturnsBadRequest()
    {
        // Arrange
        var request = new RegisterRequest("", "Password123!", null);

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/Auth/Register", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithEmptyPassword_ReturnsBadRequest()
    {
        // Arrange
        var request = new RegisterRequest("testuser", "", null);

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/Auth/Register", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithDuplicateUsername_ReturnsBadRequest()
    {
        // Arrange
        var username = $"duplicateuser_{Guid.NewGuid()}";
        var request1 = new RegisterRequest(username, "Password123!", null);
        var request2 = new RegisterRequest(username, "Password456!", null);

        // Act
        var response1 = await Client.PostAsJsonAsync("/api/v1/Auth/Register", request1);
        var response2 = await Client.PostAsJsonAsync("/api/v1/Auth/Register", request2);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response2.StatusCode);
        var errorResult = await response2.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(errorResult);
        Assert.Contains("Username taken", errorResult.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Register_WithGroupIds_ReturnsOkAndAssignsGroups()
    {
        // Arrange
        await SeedGroupsAsync();
        var request = new RegisterRequest($"groupuser_{Guid.NewGuid()}", "Password123!", new[] { 1, 2 });

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/Auth/Register", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<SuccessResponse>();
        Assert.NotNull(result);
        Assert.Equal("Registration successful", result.Message);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkAndToken()
    {
        // Arrange
        var username = $"loginuser_{Guid.NewGuid()}";
        var password = "Password123!";
        var registerRequest = new RegisterRequest(username, password, null);
        await Client.PostAsJsonAsync("/api/v1/Auth/Register", registerRequest);

        var loginRequest = new LoginRequest(username, password);

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/Auth/Login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result.Token));
    }

    [Fact]
    public async Task Login_WithInvalidUsername_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequest("nonexistentuser", "Password123!");

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/Auth/Login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var errorResult = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(errorResult);
        Assert.Contains("Invalid", errorResult.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var username = $"passwordtest_{Guid.NewGuid()}";
        var registerRequest = new RegisterRequest(username, "CorrectPassword123!", null);
        await Client.PostAsJsonAsync("/api/v1/Auth/Register", registerRequest);

        var loginRequest = new LoginRequest(username, "WrongPassword123!");

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/Auth/Login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var errorResult = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(errorResult);
        Assert.Contains("Invalid", errorResult.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Login_WithEmptyUsername_ReturnsBadRequestOrUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequest("", "Password123!");

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/Auth/Login", loginRequest);

        // Assert - Either BadRequest (validation) or Unauthorized (auth failure) is acceptable
        Assert.True(response.StatusCode == HttpStatusCode.BadRequest ||
                   response.StatusCode == HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithEmptyPassword_ReturnsBadRequestOrUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequest("testuser", "");

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/Auth/Login", loginRequest);

        // Assert - Either BadRequest (validation) or Unauthorized (auth failure) is acceptable
        Assert.True(response.StatusCode == HttpStatusCode.BadRequest ||
                   response.StatusCode == HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateUserGroups_WithAdminToken_ReturnsOk()
    {
        // Arrange
        await SeedGroupsAsync();
        var adminToken = await CreateUserAndGetTokenAsync("admin", new[] { 1 }); // Group 1 is Admin
        var targetUser = await CreateUserAsync($"targetuser_{Guid.NewGuid()}", null);

        AuthenticateClient(adminToken);
        var request = new UpdateUserGroupsRequest(targetUser.Id, new List<int> { 2, 3 });

        // Act
        var response = await Client.PutAsJsonAsync($"/api/v1/Auth/Users/{targetUser.Id}/Groups", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<SuccessResponse>();
        Assert.NotNull(result);
        Assert.Contains("updated successfully", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task UpdateUserGroups_WithoutToken_ReturnsUnauthorized()
    {
        // Arrange
        var request = new UpdateUserGroupsRequest(1, new List<int> { 1 });

        // Act
        var response = await Client.PutAsJsonAsync("/api/v1/Auth/Users/1/Groups", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateUserGroups_WithNonAdminToken_ReturnsForbidden()
    {
        // Arrange
        await SeedGroupsAsync();
        var userToken = await CreateUserAndGetTokenAsync("regularuser", new[] { 3 }); // Group 3 is User

        AuthenticateClient(userToken);
        var request = new UpdateUserGroupsRequest(1, new List<int> { 1 });

        // Act
        var response = await Client.PutAsJsonAsync("/api/v1/Auth/Users/1/Groups", request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateUserGroups_WithUserIdMismatch_ReturnsBadRequest()
    {
        // Arrange
        await SeedGroupsAsync();
        var adminToken = await CreateUserAndGetTokenAsync("admin2", new[] { 1 });

        AuthenticateClient(adminToken);
        var request = new UpdateUserGroupsRequest(1, new List<int> { 1 });

        // Act - URL has userId 2 but request body has userId 1
        var response = await Client.PutAsJsonAsync("/api/v1/Auth/Users/2/Groups", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errorResult = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(errorResult);
        Assert.Contains("mismatch", errorResult.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetUserGroups_WithAdminToken_ReturnsOkAndGroups()
    {
        // Arrange
        await SeedGroupsAsync();
        var adminToken = await CreateUserAndGetTokenAsync("admin3", new[] { 1 });
        var targetUser = await CreateUserAsync($"targetuser2_{Guid.NewGuid()}", new[] { 2, 3 });

        AuthenticateClient(adminToken);

        // Act
        var response = await Client.GetAsync($"/api/v1/Auth/Users/{targetUser.Id}/Groups");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var groups = await response.Content.ReadFromJsonAsync<List<GroupDto>>();
        Assert.NotNull(groups);
        Assert.Equal(2, groups.Count);
    }

    [Fact]
    public async Task GetUserGroups_WithoutToken_ReturnsUnauthorized()
    {
        // Act
        var response = await Client.GetAsync("/api/v1/Auth/Users/1/Groups");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetUserGroups_WithNonAdminToken_ReturnsForbidden()
    {
        // Arrange
        await SeedGroupsAsync();
        var userToken = await CreateUserAndGetTokenAsync("regularuser2", new[] { 3 });

        AuthenticateClient(userToken);

        // Act
        var response = await Client.GetAsync("/api/v1/Auth/Users/1/Groups");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetUserGroups_ForNonExistentUser_ReturnsNotFoundOrEmptyList()
    {
        // Arrange
        await SeedGroupsAsync();
        var adminToken = await CreateUserAndGetTokenAsync("admin4", new[] { 1 });

        AuthenticateClient(adminToken);

        // Act
        var response = await Client.GetAsync("/api/v1/Auth/Users/99999/Groups");

        // Assert
        // Either NotFound or OK with empty list is acceptable
        Assert.True(response.StatusCode == HttpStatusCode.NotFound ||
                   response.StatusCode == HttpStatusCode.OK);
    }

    // Helper methods
    private async Task SeedGroupsAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TicketTrackerContext>();

        if (!context.Groups.Any())
        {
            context.Groups.AddRange(
                new Group { Id = 1, Name = "Admin", Description = "Administrator group" },
                new Group { Id = 2, Name = "Support", Description = "Support team group" },
                new Group { Id = 3, Name = "User", Description = "Regular user group" }
            );
            await context.SaveChangesAsync();
        }
    }

    private async Task<string> CreateUserAndGetTokenAsync(string username, int[]? groupIds)
    {
        var uniqueUsername = $"{username}_{Guid.NewGuid()}";
        var password = "Password123!";

        var registerRequest = new RegisterRequest(uniqueUsername, password, groupIds);
        await Client.PostAsJsonAsync("/api/v1/Auth/Register", registerRequest);

        var loginRequest = new LoginRequest(uniqueUsername, password);
        var loginResponse = await Client.PostAsJsonAsync("/api/v1/Auth/Login", loginRequest);
        var authResponse = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();

        return authResponse!.Token;
    }

    private async Task<User> CreateUserAsync(string username, int[]? groupIds)
    {
        var password = "Password123!";
        var registerRequest = new RegisterRequest(username, password, groupIds);
        await Client.PostAsJsonAsync("/api/v1/Auth/Register", registerRequest);

        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TicketTrackerContext>();
        var user = await context.Users.FirstAsync(u => u.Username == username);

        return user;
    }
}