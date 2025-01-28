using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Encryption
{
    public static class Encrypter
    {
        private const int Keysize = 256;
        private const int DerivationIterations = 1000;

        // Encrypts a plain text using a passphrase
        public static string Encrypt(string plainText, string passPhrase)
        {
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);

                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;

                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                var cipherTextBytes = saltStringBytes.Concat(ivStringBytes).Concat(memoryStream.ToArray()).ToArray();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        // Decrypts a cipher text using a passphrase
        public static string Decrypt(string cipherText, string passPhrase)
        {
            try
            {
                var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
                var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
                var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
                var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).ToArray();

                using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
                {
                    var keyBytes = password.GetBytes(Keysize / 8);

                    using (var symmetricKey = new RijndaelManaged())
                    {
                        symmetricKey.BlockSize = 256;
                        symmetricKey.Mode = CipherMode.CBC;
                        symmetricKey.Padding = PaddingMode.PKCS7;

                        using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                        {
                            using (var memoryStream = new MemoryStream(cipherTextBytes))
                            {
                                using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                                {
                                    var plainTextBytes = new byte[cipherTextBytes.Length];
                                    var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

                                    return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Log or handle the error as needed
                return "Corrupted";
            }
        }

        // Computes SHA256 hash for a given password
        public static string ComputeSHA256(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hashBytes = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        // Decrypts a list of key-value pairs using the specified encryption key
        public static List<KeyValuePair<string, string[]>> DecryptData(List<KeyValuePair<string, string[]>> encryptedData, string encryptionKey)
        {
            if (encryptedData == null || !encryptedData.Any()) return null;

            return encryptedData.Select(data => new KeyValuePair<string, string[]>(Decrypt(data.Key, encryptionKey), data.Value.Select(value => Decrypt(value, encryptionKey)).ToArray())).ToList();
        }

        // Encrypts a list of key-value pairs using the specified encryption key
        public static List<KeyValuePair<string, string[]>> EncryptData(List<KeyValuePair<string, string[]>> decryptedData, string encryptionKey)
        {
            if (decryptedData == null || !decryptedData.Any()) return null;

            return decryptedData.Select(data => new KeyValuePair<string, string[]>(Encrypt(data.Key, encryptionKey), data.Value.Select(value => Encrypt(value, encryptionKey)).ToArray())).ToList();
        }

        // Generates 256 bits of random entropy
        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32];

            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(randomBytes);
            }

            return randomBytes;
        }
    }
}
