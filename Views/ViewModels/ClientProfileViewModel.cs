using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Loan_Processing_Inzamam.ViewModels
{
    public class ClientProfileViewModel
    {
        // --- Client Master Information ---
        [Required(ErrorMessage = "Full Name is required.")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        // IFormFile handles the actual file upload from the browser
        [Display(Name = "Profile Photo")]
        public IFormFile PhotoUpload { get; set; }

        [Required]
        [StringLength(20)]
        public string MaritalStatus { get; set; }

        [Required]
        [StringLength(250)]
        public string PresentAddress { get; set; }

        [StringLength(250)]
        public string PermanentAddress { get; set; }

        [Display(Name = "Permanent address is same as present address")]
        public bool IsSameAddress { get; set; }

        [Required(ErrorMessage = "NID is required.")]
        [StringLength(17, MinimumLength = 10)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "NID must contain only numbers.")]
        public string NIDNumber { get; set; }

        [Display(Name = "Currently Employed?")]
        public bool IsEmployed { get; set; }

        // --- Employment Detail Information ---
        // These are not marked [Required] here because they are only needed if IsEmployed is true.
        // We will validate this in the controller.

        [StringLength(150)]
        public string EmployerName { get; set; }

        [StringLength(100)]
        public string Occupation { get; set; }

        [Range(0, 50)]
        public int? YearsOfExperience { get; set; }
    }
}
