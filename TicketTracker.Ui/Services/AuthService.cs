using System.Net.Http.Json;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Authorization;
using TicketTracker.Ui.Services.Security;
using TicketTracker.Ui.Models;

namespace TicketTracker.Ui.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
    private bool _isAuthenticated;
    private readonly JwtAuthenticationStateProvider _authProvider;

    public AuthService(HttpClient httpClient, IJSRuntime jsRuntime, AuthenticationStateProvider authProvider)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
        _authProvider = (JwtAuthenticationStateProvider)authProvider;
    }

    public bool IsAuthenticated => _isAuthenticated;

    public async Task<bool> LoginAsync(LoginRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("auth/login", request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                if (result?.Token != null)
                {
                    await _authProvider.MarkUserAsAuthenticatedAsync(result.Token);
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.Token);
                    _isAuthenticated = true;
                    return true;
                }
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("auth/register", request);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        await _authProvider.MarkUserAsLoggedOutAsync();
        _httpClient.DefaultRequestHeaders.Authorization = null;
        _isAuthenticated = false;
    }
}

public record AuthResponse(string Token);