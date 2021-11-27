using Destructurama.Attributed;

namespace BurgerHub.Api.Infrastructure.Encryption;

public class EncryptionOptions
{
    [NotLogged] public string Pepper { get; set; } = null!;
}