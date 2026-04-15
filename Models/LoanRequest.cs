using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Loan_Processing_Inzamam.Models
{
    public class LoanRequest
    {
        [Key]
        public int LoanRequestId { get; set; }

        public int ClientId { get; set; }
        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }
        [Required]
        public DateTime RequestDate { get; set; } = DateTime.Now;

        [Required]
        [Range(1000, 50000000, ErrorMessage = "Loan amount must be between 1,000 and 50,000,000.")]
        [DataType(DataType.Currency)]
        public decimal DesiredLoanAmount { get; set; }

        [Required]
        [Range(0, 100000000, ErrorMessage = "Invalid Annual Income.")]
        [DataType(DataType.Currency)]
        public decimal AnnualIncome { get; set; }

        public string TINFilePath { get; set; }

        [Required]
        [Range(6, 360, ErrorMessage = "Loan term must be between 6 and 360 months.")]
        public int LoanTermMonths { get; set; }

        [Required]
        public int LoanTypeId { get; set; }
        [ForeignKey("LoanTypeId")]
        public virtual LoanType LoanType { get; set; }

        [Required]
        [StringLength(500)]
        public string LoanPurpose { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Pending";
        public int? ApprovedByEmployeeId { get; set; }
        [ForeignKey("ApprovedByEmployeeId")]
        public virtual Employee ApprovedByEmployee { get; set; }

        public DateTime? ActionDate { get; set; }

        public string AdminRemarks { get; set; }


        public virtual ICollection<Guarantor> Guarantors { get; set; }
    }
    public class Guarantor
    {
        [Key]
        public int GuarantorId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Relationship { get; set; }

        [Required]
        [StringLength(250)]
        public string ContactAddress { get; set; }

        [Required]
        [Phone]
        [StringLength(20)]
        public string Phone { get; set; }

        public int LoanRequestId { get; set; }
        [ForeignKey("LoanRequestId")]
        public virtual LoanRequest LoanRequest { get; set; }
    }
}
