using Microsoft.AspNetCore.Http;

namespace ClaimFlow.Tests.TestHelpers
{
    public class FormFileStub : IFormFile
    {
        public FormFileStub(string fileName, byte[] bytes, string contentType = "application/octet-stream")
        { FileName = fileName; _bytes = bytes; ContentType = contentType; Length = bytes.LongLength; }
        private readonly byte[] _bytes;
        public string ContentType { get; }
        public string ContentDisposition => "";
        public IHeaderDictionary Headers => new HeaderDictionary();
        public long Length { get; }
        public string Name => "Upload";
        public string FileName { get; }
        public void CopyTo(Stream target) => new MemoryStream(_bytes).CopyTo(target);
        public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
            => new MemoryStream(_bytes).CopyToAsync(target, cancellationToken);
        public Stream OpenReadStream() => new MemoryStream(_bytes);
    }
}
