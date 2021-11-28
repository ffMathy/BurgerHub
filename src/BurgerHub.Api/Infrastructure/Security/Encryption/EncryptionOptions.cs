using Destructurama.Attributed;

namespace BurgerHub.Api.Infrastructure.Security.Encryption;

public class EncryptionOptions
{
    [NotLogged] public string Pepper { get; set; } = null!;
}