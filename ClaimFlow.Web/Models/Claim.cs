using System.ComponentModel.DataAnnotations;

namespace ClaimFlow.Web.Models
{
    public class Claim
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Lecturer identifiers kept simple (no auth in Part 2)
        [Required, StringLength(60)]
        public string LecturerName { get; set; } = "";

        [Range(0.5, 300)]
        public decimal HoursWorked { get; set; }

        [Range(50, 2000)]
        public decimal HourlyRate { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public decimal Total => Math.Round(HoursWorked * HourlyRate, 2);

        public ClaimStatus Status { get; set; } = ClaimStatus.Submitted;

        // Review trail
        public string? CoordinatorName { get; set; }
        public DateTime? VerifiedAt { get; set; }

        public string? ManagerName { get; set; }
        public Guid? ManagerId { get; set; }   // <-- per feedback: store manager ID
        public DateTime? ApprovedOrRejectedAt { get; set; }

        public List<DocumentRef> Documents { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
