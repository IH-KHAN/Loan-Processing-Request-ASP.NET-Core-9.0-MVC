using System.ComponentModel.DataAnnotations;

namespace Loan_Processing_Inzamam.ViewModels
{
    public class TransactionViewModel
    {
        [Required(ErrorMessage = "Please select an account.")]
        [Display(Name = "Bank Account")]
        public int AccountId { get; set; }

        [Required(ErrorMessage = "Please select a transaction type.")]
        [Display(Name = "Transaction Type")]
        public string TransactionType { get; set; } // "Deposit" or "Withdraw"

        [Required]
        [Range(10, 1000000, ErrorMessage = "Amount must be between ৳10 and ৳1,000,000.")]
        [Display(Name = "Amount (৳)")]
        public decimal Amount { get; set; }
    }
}