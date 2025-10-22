namespace ClaimFlow.Web.Services
{
    public interface IFileEncryptor
    {
        Task<string> EncryptToFileAsync(Stream input, string destFileNameNoExt);
        Task<byte[]> DecryptAsync(string encryptedFileName);
    }
}
