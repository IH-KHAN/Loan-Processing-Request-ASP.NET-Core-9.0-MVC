using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Loan_Processing_Inzamam.Models
{
    public class Client
    {
        [Key]
        public int ClientId { get; set; }

        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required(ErrorMessage = "Full Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid Phone Number.")]
        [StringLength(20)]
        public string? Phone { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        public string PhotoPath { get; set; }

        [Required]
        [StringLength(20)]
        public string MaritalStatus { get; set; }

        [Required]
        [StringLength(250)]
        public string PresentAddress { get; set; }

        [StringLength(250)]
        public string PermanentAddress { get; set; }

        public bool IsSameAddress { get; set; }

        [Required(ErrorMessage = "NID is required.")]
        [StringLength(17, MinimumLength = 10, ErrorMessage = "NID must be between 10 and 17 digits.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "NID must contain only numbers.")]
        public string NIDNumber { get; set; }

        public bool IsEmployed { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual EmploymentDetail EmploymentDetail { get; set; }
        public virtual ICollection<BankAccount> BankAccounts { get; set; }
        public virtual ICollection<LoanRequest> LoanRequests { get; set; }
    }
    public class EmploymentDetail
    {
        [Key]
        public int EmploymentId { get; set; }

        [Required]
        [StringLength(150)]
        public string EmployerName { get; set; }

        [Required]
        [StringLength(100)]
        public string Occupation { get; set; }

        [Required]
        [Range(0, 50)]
        public int YearsOfExperience { get; set; }

        public int ClientId { get; set; }
        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }
    }
}
