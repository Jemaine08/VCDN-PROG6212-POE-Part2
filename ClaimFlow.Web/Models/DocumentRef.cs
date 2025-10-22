// Reference: Microsoft Learn (2023) C# Properties
// Available at: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/properties
// Assisted in defining calculated properties such as `LecturerName`, `HourlyRate`, etc., for the view model.

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
