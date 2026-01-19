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

            using MemoryStream memeoryStream = new MemoryStream();
            memeoryStream.Write(aes.IV, 0, IvSize);

            using ICryptoTransform encryptor = aes.CreateEncryptor();
            using CryptoStream cryptoStream = new CryptoStream(memeoryStream, encryptor, CryptoStreamMode.Write);
            using StreamWriter writer = new StreamWriter(cryptoStream);

            writer.Write(plainText);

            return Convert.ToBase64String(memeoryStream.ToArray());
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

            using MemoryStream memeoryStream = new();
            using ICryptoTransform decryptor = aes.CreateDecryptor();
            using CryptoStream cryptoStream = new(memeoryStream, decryptor, CryptoStreamMode.Read);
            using StreamReader reader = new(cryptoStream);

            return reader.ReadToEnd();
        }
        catch (CryptographicException exception)
        {
            throw new InvalidOperationException($"Decrypt exception", exception);
        }
    }
}
