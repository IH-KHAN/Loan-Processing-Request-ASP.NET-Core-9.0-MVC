using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Loan_Processing_Inzamam.ViewModels
{
    public class EmployeeCreateViewModel
    {
        // --- Identity Login Info ---
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // --- Employee Profile Info ---
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Designation")]
        public int DesignationId { get; set; }

        [Required]
        [Phone]
        [StringLength(20)]
        public string Phone { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Profile Photo")]
        public IFormFile? PhotoUpload { get; set; }

        [Required]
        [StringLength(20)]
        public string MaritalStatus { get; set; }

        [Required]
        [StringLength(250)]
        public string PresentAddress { get; set; }

        [StringLength(250)]
        public string? PermanentAddress { get; set; }

        public bool IsSameAddress { get; set; }

        [Required]
        [StringLength(17, MinimumLength = 10)]
        public string NIDNumber { get; set; }

        [StringLength(100)]
        [Display(Name = "Department / Service Category")]
        public string? ServiceCategory { get; set; }

        [Display(Name = "Branch Assignment")]
        public int? BranchId { get; set; }

        // --- Dynamic Educational Qualifications ---
        public List<QualificationViewModel> Qualifications { get; set; } = new List<QualificationViewModel>();
    }

    public class QualificationViewModel
    {
        [Required(ErrorMessage = "Degree Name is required")]
        [StringLength(100)]
        public string DegreeName { get; set; }

        [Required(ErrorMessage = "Institution is required")]
        [StringLength(150)]
        public string InstitutionName { get; set; }

        [Required]
        [Range(1950, 2100)]
        public int PassingYear { get; set; }

        [Required]
        [Range(0.0, 5.0)]
        public double GradingScore { get; set; }

        [Required]
        [Range(4.0, 5.0)]
        public double OutOf { get; set; } = 4.0;
    }
}