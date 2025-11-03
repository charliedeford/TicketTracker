using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace TicketTracker.Ui.Services.Security;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private const string TokenStorageKey = "authToken";
    private readonly IJSRuntime _js;
    private readonly HttpClient _httpClient;

    public JwtAuthenticationStateProvider(IJSRuntime js, HttpClient httpClient)
    {
        _js = js;
        _httpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _js.InvokeAsync<string?>("localStorage.getItem", TokenStorageKey);

        if (string.IsNullOrWhiteSpace(token))
        {
            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            return new AuthenticationState(anonymous);
        }

        // Attach bearer for future HTTP calls
        if (_httpClient.DefaultRequestHeaders.Authorization == null)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        var identity = CreateIdentityFromJwt(token);
        var user = new ClaimsPrincipal(identity);
        return new AuthenticationState(user);
    }

    public async Task MarkUserAsAuthenticatedAsync(string token)
    {
        await _js.InvokeVoidAsync("localStorage.setItem", TokenStorageKey, token);
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task MarkUserAsLoggedOutAsync()
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", TokenStorageKey);
        _httpClient.DefaultRequestHeaders.Authorization = null;
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()))));
    }

    private static ClaimsIdentity CreateIdentityFromJwt(string token)
    {
        try
        {
            var claims = ParseClaimsFromJwt(token);
            return new ClaimsIdentity(claims, authenticationType: "jwt");
        }
        catch
        {
            return new ClaimsIdentity();
        }
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        // JWT format: header.payload.signature
        var parts = jwt.Split('.');
        if (parts.Length < 2)
            return Array.Empty<Claim>();

        var payload = parts[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes) ?? new();

        var claims = new List<Claim>();
        foreach (var kvp in keyValuePairs)
        {
            if (kvp.Value is JsonElement el && el.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in el.EnumerateArray())
                {
                    claims.Add(new Claim(kvp.Key, item.ToString()));
                }
            }
            else
            {
                claims.Add(new Claim(kvp.Key, kvp.Value?.ToString() ?? string.Empty));
            }
        }

        // Map standard names where helpful
        var mapped = new List<Claim>();
        foreach (var c in claims)
        {
            var type = c.Type switch
            {
                "sub" => ClaimTypes.NameIdentifier,
                "unique_name" => ClaimTypes.Name,
                _ => c.Type
            };
            mapped.Add(new Claim(type, c.Value));
        }

        return mapped;
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}
