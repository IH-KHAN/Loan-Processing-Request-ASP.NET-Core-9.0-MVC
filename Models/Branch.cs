using System.ComponentModel.DataAnnotations;

namespace Loan_Processing_Inzamam.Models
{
    public class Branch
    {
        [Key]
        public int BranchId { get; set; }

        [Required]
        [StringLength(100)]
        public string BranchName { get; set; }

        [Required]
        [StringLength(250)]
        public string Location { get; set; }

        [StringLength(500)]
        public string Features { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }
    }
}
