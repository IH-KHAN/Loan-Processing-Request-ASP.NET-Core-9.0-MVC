using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Loan_Processing_Inzamam.ViewModels
{
    public class EmployeeEditViewModel
    {
        public int EmployeeId { get; set; }
        public string ApplicationUserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Designation")]
        public int DesignationId { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        [Display(Name = "Branch Assignment")]
        public int? BranchId { get; set; }

        [StringLength(100)]
        [Display(Name = "Department / Service Category")]
        public string? ServiceCategory { get; set; }

        [Display(Name = "Update Profile Photo (Leave blank to keep current)")]
        public IFormFile? PhotoUpload { get; set; }

        public string? ExistingPhotoPath { get; set; }
        // Add this line at the bottom of EmployeeEditViewModel
        public List<QualificationViewModel> Qualifications { get; set; } = new List<QualificationViewModel>();
    }
}