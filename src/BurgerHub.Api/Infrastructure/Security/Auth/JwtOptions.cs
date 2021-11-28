using Destructurama.Attributed;

namespace BurgerHub.Api.Infrastructure.Security.Auth;

public class JwtOptions
{
    [NotLogged] public string PrivateKey { get; set; } = null!;
    [NotLogged] public string Audience { get; set; } = null!;
    [NotLogged] public string Issuer { get; set; } = null!;
    
    public int ExpirationTimeInMinutes { get; set; }
}