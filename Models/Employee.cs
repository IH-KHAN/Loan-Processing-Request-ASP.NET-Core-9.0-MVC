using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Loan_Processing_Inzamam.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public int DesignationId { get; set; }
        [ForeignKey("DesignationId")]
        public virtual Designation Designation { get; set; }

        [Required]
        [Phone]
        [StringLength(20)]
        public string Phone { get; set; }

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

        [Required]
        [StringLength(17, MinimumLength = 10)]
        public string NIDNumber { get; set; }

        public bool IsActive { get; set; } = true;

        // The specific banking department they work in
        [StringLength(100)]
        public string ServiceCategory { get; set; }

        public int? BranchId { get; set; }
        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<EducationalQualification> EducationalQualifications { get; set; }
    }
    public class EducationalQualification
    {
        [Key]
        public int QualificationId { get; set; }

        [Required]
        [StringLength(100)]
        public string DegreeName { get; set; }

        [Required]
        [StringLength(150)]
        public string InstitutionName { get; set; }

        [Required]
        [Range(1950, 2100)]
        public int PassingYear { get; set; }

        [Required]
        [Range(0.0, 5.0, ErrorMessage = "Score must be between 0.0 and 5.0")]
        public double GradingScore { get; set; }

        [Required]
        [Range(4.0, 5.0)]
        public double OutOf { get; set; }

        public int EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }

    }
    public class Designation
    {
        [Key]
        public int DesignationId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } // e.g., Principal Officer, Management Trainee

        public bool IsActive { get; set; } = true; // For Admin Soft Delete

        // Navigation property: One designation can belong to many employees
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
