using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Loan_Processing_Inzamam.Models
{
    public class BankAccount
    {
        [Key]
        public int AccountId { get; set; }

        public int ClientId { get; set; }
        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }

        [Required]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual AccountCategory AccountCategory { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Balance { get; set; }
        [Required]
        [StringLength(20)]
        public string AccountNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
