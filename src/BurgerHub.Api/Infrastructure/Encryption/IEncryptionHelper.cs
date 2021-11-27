namespace BurgerHub.Api.Infrastructure.Encryption;

public interface IEncryptionHelper
{
    Task<byte[]> EncryptAsync(string plainText, bool withoutSalt = false);
    Task<string> DecryptAsync(byte[] cipherText);
    byte[] Hash(string plainText);
}