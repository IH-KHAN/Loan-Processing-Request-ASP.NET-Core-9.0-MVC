using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Loan_Processing_Inzamam.ViewModels
{
    public class ClientEditViewModel
    {
        public int ClientId { get; set; }
        public string ApplicationUserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public string Phone { get; set; }


        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "NID Number")]
        public string NIDNumber { get; set; }

        [Required]
        [Display(Name = "Present Address")]
        public string PresentAddress { get; set; }

        [Display(Name = "Permanent Address")]
        public string? PermanentAddress { get; set; }

        [Display(Name = "Update Profile Photo (Leave blank to keep current)")]
        public IFormFile? PhotoUpload { get; set; }

        public string? ExistingPhotoPath { get; set; }
    }
}