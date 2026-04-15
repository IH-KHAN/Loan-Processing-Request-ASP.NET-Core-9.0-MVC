using System.ComponentModel.DataAnnotations;

namespace Loan_Processing_Inzamam.ViewModels
{
    public class BankAccountCreateViewModel
    {
        [Required(ErrorMessage = "Please select an account type.")]
        [Display(Name = "Account Type")]
        public int CategoryId { get; set; }

        [Required]
        [Range(500, 10000000, ErrorMessage = "A minimum initial deposit of ৳500 is required to open an account.")]
        [Display(Name = "Initial Deposit Amount")]
        public decimal InitialDeposit { get; set; }
    }
}