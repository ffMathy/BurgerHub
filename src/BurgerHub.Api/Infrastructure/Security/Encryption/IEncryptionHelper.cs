namespace BurgerHub.Api.Infrastructure.Security.Encryption;

public interface IEncryptionHelper
{
    Task<string> EncryptAsync(string plainText, bool withoutInitializationVector = false);
    Task<string> DecryptAsync(string cipherText);
}