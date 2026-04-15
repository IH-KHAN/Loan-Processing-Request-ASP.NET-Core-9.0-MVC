using System.ComponentModel.DataAnnotations;

namespace Loan_Processing_Inzamam.ViewModels
{
    public class LoanProcessViewModel
    {
        public int LoanRequestId { get; set; }

        [Required(ErrorMessage = "You must select a status.")]
        public string Status { get; set; } // "Approved" or "Rejected"

        [Required(ErrorMessage = "Please provide remarks for this decision.")]
        [Display(Name = "Admin / Officer Remarks")]
        public string AdminRemarks { get; set; }
    }
}