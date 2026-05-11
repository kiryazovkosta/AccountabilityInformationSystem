using System.Security.Cryptography;
using AccountabilityInformationSystem.Api.Settings;
using Microsoft.Extensions.Options;

namespace AccountabilityInformationSystem.Api.Shared.Services.Encrypting;

public sealed class EncryptionService(IOptionsSnapshot<EncryptionOptions> options)
{
    private readonly byte[] _masterKey = Convert.FromBase64String(options.Value.Key);
    private const int IvSize = 16;

    public string Encrypt(string plainText)
    {
        try
        {
            using Aes aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = _masterKey;
            aes.IV = RandomNumberGenerator.GetBytes(IvSize);

            using MemoryStream memoryStream = new MemoryStream();
            memoryStream.Write(aes.IV, 0, IvSize);

            using ICryptoTransform encryptor = aes.CreateEncryptor();
            using CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            using StreamWriter writer = new StreamWriter(cryptoStream);

            writer.Write(plainText);
            writer.Flush();
            cryptoStream.FlushFinalBlock();

            return Convert.ToBase64String(memoryStream.ToArray());
        }
        catch ( CryptographicException exception)
        {
            throw new InvalidOperationException($"Encrypt exception", exception);
        }
    }

    public string Decrypt(string cipherText)
    {
        try
        {
            byte[] cipherData = Convert.FromBase64String(cipherText);
            if (cipherData.Length < IvSize )
            {
                throw new ArgumentException("Invalid cipher text format");
            }

            byte[] iv = new byte[IvSize];
            byte[] encryptedData = new byte[cipherData.Length - IvSize];

            Buffer.BlockCopy(cipherData, 0, iv, 0, IvSize);
            Buffer.BlockCopy(cipherData, IvSize, encryptedData, 0, encryptedData.Length);

            using Aes aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = _masterKey;
            aes.IV = iv;

            using MemoryStream memoryStream = new(encryptedData);
            using ICryptoTransform decryptor = aes.CreateDecryptor();
            using CryptoStream cryptoStream = new(memoryStream, decryptor, CryptoStreamMode.Read);
            using StreamReader reader = new(cryptoStream);

            return reader.ReadToEnd();
        }
        catch (CryptographicException exception)
        {
            throw new InvalidOperationException($"Decrypt exception", exception);
        }
    }
}
