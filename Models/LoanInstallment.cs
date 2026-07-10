using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Loan_Processing_Inzamam.Models
{
    public class LoanInstallment
    {
        [Key]
        public int InstallmentId { get; set; }

        [Required]
        public int LoanRequestId { get; set; }
        [ForeignKey("LoanRequestId")]
        public virtual LoanRequest LoanRequest { get; set; }

        [Required]
        [Range(0.01, 100000000.00)]
        [DataType(DataType.Currency)]
        public decimal AmountPaid { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } // "Bank Account" or "Cash"

        public int? BankAccountId { get; set; }
        [ForeignKey("BankAccountId")]
        public virtual BankAccount BankAccount { get; set; }
    }
}
