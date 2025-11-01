using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TicketTracker.Api.Models.Entities;
using TicketTracker.Api.Models.Responses;

namespace TicketTracker.Api.Services;

public class AuthService(IUserRepository userRepository, IConfiguration config) : IAuthService
{
    public async Task<RegisterResponse> RegisterAsync(string username, string password, int[]? groupIds, CancellationToken cancellationToken = default)
    {
        if (await userRepository.UsernameExistsAsync(username))
        {
            return new RegisterResponse(false, "Username taken");
        }

        var user = new User
        {
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            
        };

        await userRepository.CreateAsync(user, cancellationToken);
        return new RegisterResponse(true, null);
    }

    public async Task<LoginResponse> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByUsernameAsync(username, cancellationToken);
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return new LoginResponse(false, null, "Invalid username or password");
        }

        var token = GenerateToken(user);
        return new LoginResponse(true, token, null);
    }

    private string GenerateToken(User user)
    {
        var jwt = config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwt["Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Username)
        };

        foreach (var group in user.Groups)
        {
            claims.Add(new Claim("group", group.Name));
        }

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<LoginResponse> UpdateUserGroupsAsync(int userId, List<int> groupIds, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return new LoginResponse(false, null, "User not found");
        }

        await userRepository.UpdateUserGroupsAsync(userId, groupIds, cancellationToken);
        return new LoginResponse(true, null, null);
    }

    public async Task<List<Group>> GetUserGroupsAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return new List<Group>();
        }

        return user.Groups.ToList();
    }
}