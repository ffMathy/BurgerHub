namespace BurgerHub.Api.Infrastructure.Encryption;

public interface IAesEncryptionHelper
{
    Task<byte[]> EncryptAsync(string plainText, bool withoutSalt = false);
    Task<string> DecryptAsync(byte[] cipherText);
}