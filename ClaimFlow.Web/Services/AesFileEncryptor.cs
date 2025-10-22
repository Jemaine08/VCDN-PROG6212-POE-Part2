// Reference: Microsoft Learn (2024) AES Class — System.Security.Cryptography.Aes
// Available at: https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.aes
// Used to implement AES encryption and decryption for file uploads in the ClaimsController.

using System.Security.Cryptography;
using Microsoft.Extensions.Options;

namespace ClaimFlow.Web.Services
{
    public class AesFileEncryptor : IFileEncryptor
    {
        private readonly string _folder;
        private readonly byte[] _key;

        public class StorageCryptoOptions
        {
            public string UploadsFolder { get; set; } = "wwwroot/uploads";
            public string AesKeyBase64 { get; set; } = "";
        }

        public AesFileEncryptor(IOptionsMonitor<StorageCryptoOptions> opts)
        {
            _folder = opts.CurrentValue.UploadsFolder;
            if (!Directory.Exists(_folder)) Directory.CreateDirectory(_folder);
            _key = Convert.FromBase64String(opts.CurrentValue.AesKeyBase64);
        }

        public async Task<string> EncryptToFileAsync(Stream input, string destFileNameNoExt)
        {
            var file = Path.Combine(_folder, $"{destFileNameNoExt}.bin");
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.GenerateIV();
            await using var fs = new FileStream(file, FileMode.Create, FileAccess.Write);
            await fs.WriteAsync(aes.IV);
            await using var crypto = new CryptoStream(fs, aes.CreateEncryptor(), CryptoStreamMode.Write);
            await input.CopyToAsync(crypto);
            await crypto.FlushAsync();
            return Path.GetFileName(file);
        }

        public async Task<byte[]> DecryptAsync(string encryptedFileName)
        {
            var file = Path.Combine(_folder, encryptedFileName);
            await using var fs = new FileStream(file, FileMode.Open, FileAccess.Read);
            var iv = new byte[16];
            _ = await fs.ReadAsync(iv);
            using var aes = Aes.Create();
            aes.Key = _key; aes.IV = iv;
            await using var crypto = new CryptoStream(fs, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using var ms = new MemoryStream();
            await crypto.CopyToAsync(ms);
            return ms.ToArray();
        }
    }
}
