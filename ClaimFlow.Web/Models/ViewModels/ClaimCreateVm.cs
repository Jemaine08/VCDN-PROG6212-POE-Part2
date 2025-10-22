using System.ComponentModel.DataAnnotations;

namespace ClaimFlow.Web.Models.ViewModels
{
    public class ClaimCreateVm
    {
        [Required, StringLength(60)]
        public string LecturerName { get; set; } = "";

        [Range(0.5, 300)]
        public decimal HoursWorked { get; set; }

        [Range(50, 2000)]
        public decimal HourlyRate { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
        public IFormFile? Upload { get; set; }
    }
}
