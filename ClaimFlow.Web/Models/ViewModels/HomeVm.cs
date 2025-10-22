// Reference: Microsoft Learn (2023) C# Properties
// Available at: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/properties
// Assisted in defining calculated properties such as `LecturerName`, `HourlyRate`, etc., for the view model.

using ClaimFlow.Web.Models;

namespace ClaimFlow.Web.Models.ViewModels
{
    public class HomeVm
    {
        public int Total { get; set; }
        public int Submitted { get; set; }
        public int Verified { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }

        public List<Claim> Recent { get; set; } = new();
    }
}
