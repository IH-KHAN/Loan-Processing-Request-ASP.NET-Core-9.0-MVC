using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Loan_Processing_Inzamam.ViewModels
{
    public class LoanRequestViewModel
    {
        [Required]
        [Display(Name = "Loan Type")]
        public int LoanTypeId { get; set; }

        [Required]
        [Range(1000, 50000000, ErrorMessage = "Loan amount must be between 1,000 and 50,000,000.")]
        [Display(Name = "Desired Loan Amount")]
        public decimal DesiredLoanAmount { get; set; }

        [Required]
        [Range(0, 100000000, ErrorMessage = "Invalid Annual Income.")]
        [Display(Name = "Annual Income")]
        public decimal AnnualIncome { get; set; }

        [Required]
        [Range(6, 360, ErrorMessage = "Loan term must be between 6 and 360 months.")]
        [Display(Name = "Duration (Months)")]
        public int LoanTermMonths { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Purpose of Loan")]
        public string LoanPurpose { get; set; }

        [Required(ErrorMessage = "Please upload your TIN Certificate.")]
        [Display(Name = "TIN Certificate Document")]
        public IFormFile TINFileUpload { get; set; }

        // Dynamic list of Guarantors
        public List<GuarantorViewModel> Guarantors { get; set; } = new List<GuarantorViewModel>();
    }

    public class GuarantorViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Relationship is required")]
        [StringLength(50)]
        public string Relationship { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(250)]
        public string ContactAddress { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [Phone]
        [StringLength(20)]
        public string Phone { get; set; }
    }
}