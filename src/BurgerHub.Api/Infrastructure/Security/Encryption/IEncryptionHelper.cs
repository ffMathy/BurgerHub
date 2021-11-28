namespace BurgerHub.Api.Infrastructure.Security.Encryption;

public interface IEncryptionHelper
{
    Task<string> EncryptAsync(string plainText, bool withoutSalt = false);
    Task<string> DecryptAsync(string cipherText);
}