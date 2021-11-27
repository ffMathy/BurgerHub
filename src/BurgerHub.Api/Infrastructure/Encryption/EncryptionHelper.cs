using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;

namespace BurgerHub.Api.Infrastructure.Encryption;

public class EncryptionHelper : IEncryptionHelper
{
    private readonly IOptionsMonitor<EncryptionOptions> _encryptionOptionsMonitor;

    private const int KeySize = 256;
    private const int BlockSize = 128;

    public EncryptionHelper(
        IOptionsMonitor<EncryptionOptions> encryptionOptionsMonitor)
    {
        _encryptionOptionsMonitor = encryptionOptionsMonitor;
    }

    public async Task<byte[]> EncryptAsync(string plainText, bool withoutSalt = false)
    {
        var key = _encryptionOptionsMonitor.CurrentValue.Pepper;
        if (key == null)
            throw new InvalidOperationException("Could not find a pepper in the configuration of the application.");

        using var aes = GetAesAlgorithm(key);
        aes.IV = withoutSalt ?
            GetEmptyInitializationVector(aes) :
            GenerateRandomInitializationVector(key);

        using var encryptor = aes.CreateEncryptor(
            aes.Key,
            aes.IV);

        await using var memoryStream = new MemoryStream();
        await memoryStream.WriteAsync(aes.IV);

        await using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
        await using (var streamWriter = new StreamWriter(cryptoStream))
        {
            await streamWriter.WriteAsync(plainText);
        }

        var encrypted = memoryStream.ToArray();
        return encrypted;
    }

    public byte[] Hash(string plainText)
    {
        var key = _encryptionOptionsMonitor.CurrentValue.Pepper;
        if (key == null)
            throw new InvalidOperationException("Could not find a pepper in the configuration of the application.");
        
        var salt = RandomNumberGenerator.GetBytes(BlockSize);
        return KeyDerivation.Pbkdf2(
            plainText + key,
            salt,
            KeyDerivationPrf.HMACSHA512,
            iterationCount: 100000,
            numBytesRequested: KeySize / 8);
    }

    public async Task<string> DecryptAsync(byte[] cipherText)
    {
        var key = _encryptionOptionsMonitor.CurrentValue.Pepper;
        if (key == null)
            throw new InvalidOperationException("Could not find a pepper in the configuration of the application.");

        var dataBytes = ExtractDataBytesFromCipherText(cipherText);

        using var aes = GetAesAlgorithm(key);
        aes.IV = ExtractInitializationVectorFromCipherText(cipherText);

        using var decryptor = aes.CreateDecryptor(
            aes.Key,
            aes.IV);

        await using var memoryStream = new MemoryStream(dataBytes);
        await using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        using var streamReader = new StreamReader(cryptoStream);
        var text = await streamReader.ReadToEndAsync();

        return text;
    }

    private static byte[] GetEmptyInitializationVector(Aes aes)
    {
        return new byte[aes.BlockSize / 8];
    }

    private static byte[] GenerateRandomInitializationVector(string key)
    {
        var aes = GetAesAlgorithm(key);
        aes.GenerateIV();

        return aes.IV;
    }

    private static Aes GetAesAlgorithm(string key)
    {
        var aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.KeySize = KeySize;
        aes.BlockSize = 128;
        aes.Key = GetKeyBytesFromString(key);

        return aes;
    }

    private static byte[] GetKeyBytesFromString(string key)
    {
        return Encoding.UTF8.GetBytes(key);
    }

    private static byte[] ExtractDataBytesFromCipherText(byte[] cipherText)
    {
        var data = new byte[cipherText.Length - 16];
        Array.Copy(cipherText, 16, data, 0, data.Length);
        return data;
    }

    private static byte[] ExtractInitializationVectorFromCipherText(byte[] cipherText)
    {
        var initializationVector = new byte[16];
        Array.Copy(cipherText, 0, initializationVector, 0, initializationVector.Length);
        return initializationVector;
    }
}