using AnimalShelter.Constants;
using AnimalShelter.Domain.Repositores;
using AnimalShelter.Domain.UserEntities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AnimalShelter.Infrastructure.Services;

public class JWTService
{
    private readonly IAccountRepository _accountRepository;

    public JWTService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<JwtSecurityToken> GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppConfigurationConstants.JwtSecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: AppConfigurationConstants.JwtIssuer,
            audience: AppConfigurationConstants.JwtAudience,
            claims: await GetClaimsAsync(user),
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return token;
    }

    private async Task<List<Claim>> GetClaimsAsync(User user)
    {
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, user.Username!)
        };

        var role = await _accountRepository.GetUserRole(user.Id);

        claims.Add(new Claim(ClaimTypes.Role, role.Name));
        
        return claims;
    }
}
