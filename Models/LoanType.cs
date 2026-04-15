using System.ComponentModel.DataAnnotations;

namespace Loan_Processing_Inzamam.Models
{
    public class LoanType
    {
        [Key]
        public int LoanTypeId { get; set; }

        [Required]
        [StringLength(100)]
        public string TypeName { get; set; } // e.g., Personal Loan, Home Loan, Auto Loan

        [Required]
        [Range(0, 100)]
        public decimal InterestRate { get; set; } // e.g., 9.5 for 9.5%

        public bool IsActive { get; set; } = true;

        public virtual ICollection<LoanRequest> LoanRequests { get; set; }
    }
}
