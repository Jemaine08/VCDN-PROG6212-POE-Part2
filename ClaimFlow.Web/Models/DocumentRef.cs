namespace ClaimFlow.Web.Models
{
    public class DocumentRef
    {
        public string OriginalFileName { get; set; } = "";
        public string EncryptedFileName { get; set; } = "";
        public long SizeBytes { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
