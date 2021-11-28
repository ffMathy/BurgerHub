using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BurgerHub.Api.Infrastructure.Security.Auth;

public interface IJwtTokenFactory
{
    string Create(IEnumerable<Claim> claims);
}

public class JwtTokenFactory : IJwtTokenFactory
{
    private readonly IOptionsMonitor<JwtOptions> _jwtOptionsMonitor;

    public JwtTokenFactory(
        IOptionsMonitor<JwtOptions> jwtOptionsMonitor)
    {
        _jwtOptionsMonitor = jwtOptionsMonitor;
    }
        
    public string Create(IEnumerable<Claim> claims)
    {
        var jwtOptions = _jwtOptionsMonitor.CurrentValue;
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims.Distinct()),
            Expires = DateTime.UtcNow.AddMinutes(jwtOptions.ExpirationTimeInMinutes),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(jwtOptions.PrivateKey)),
                SecurityAlgorithms.HmacSha512Signature),
            Audience = jwtOptions.Audience,
            Issuer = jwtOptions.Issuer
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}