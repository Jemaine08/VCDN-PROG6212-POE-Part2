// Reference: Microsoft Learn (2024) AES Class — System.Security.Cryptography.Aes
// Available at: https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.aes
// Used to implement AES encryption and decryption for file uploads in the ClaimsController.

namespace ClaimFlow.Web.Services
{
    public interface IFileEncryptor
    {
        Task<string> EncryptToFileAsync(Stream input, string destFileNameNoExt);
        Task<byte[]> DecryptAsync(string encryptedFileName);
    }
}
