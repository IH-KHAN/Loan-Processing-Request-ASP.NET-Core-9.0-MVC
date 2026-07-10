using System.ComponentModel.DataAnnotations;

namespace Loan_Processing_Inzamam.ViewModels
{
    public class PayInstallmentViewModel
    {
        [Required]
        public int LoanRequestId { get; set; }

        [Required(ErrorMessage = "Please select a bank account to pay from.")]
        [Display(Name = "Bank Account")]
        public int BankAccountId { get; set; }

        [Required]
        [Range(1.00, 10000000.00, ErrorMessage = "Payment amount must be at least ৳1.00.")]
        [Display(Name = "Installment Amount (৳)")]
        public decimal Amount { get; set; }
    }
}
