using System.ComponentModel.DataAnnotations;

namespace Loan_Processing_Inzamam.Models
{
    public class AccountCategory
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } // e.g., Savings, Current, Student

        public bool IsActive { get; set; } = true;

        public virtual ICollection<BankAccount> BankAccounts { get; set; }
    }
}
