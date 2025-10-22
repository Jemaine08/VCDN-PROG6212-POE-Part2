// Reference: Microsoft Learn (2024) AES Class — System.Security.Cryptography.Aes
// Available at: https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.aes
// Used to implement AES encryption and decryption for file uploads in the ClaimsController.

using Microsoft.Extensions.Options;

namespace ClaimFlow.Web.Services
{
    public interface IFileValidationService
    {
        void Validate(IFormFile? file);
    }

    public class FileValidationService : IFileValidationService
    {
        private readonly long _max;
        private readonly HashSet<string> _allowed;

        public class FileRules
        {
            public long MaxFileSizeBytes { get; set; }
            public string[] AllowedExtensions { get; set; } = Array.Empty<string>();
        }

        public FileValidationService(IOptionsMonitor<FileRules> opts)
        {
            _max = opts.CurrentValue.MaxFileSizeBytes;
            _allowed = new HashSet<string>(opts.CurrentValue.AllowedExtensions.Select(e => e.ToLowerInvariant()));
        }

        public void Validate(IFormFile? file)
        {
            if (file == null) return;
            if (file.Length == 0) throw new InvalidOperationException("Empty file.");
            if (file.Length > _max) throw new InvalidOperationException($"File exceeds limit of {_max} bytes.");
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowed.Contains(ext)) throw new InvalidOperationException($"File type {ext} not allowed.");
        }
    }
}
