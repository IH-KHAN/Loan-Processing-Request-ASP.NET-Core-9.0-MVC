using Microsoft.AspNetCore.Identity;
namespace Loan_Processing_Inzamam.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsProfileComplete { get; set; }
        public virtual Client ClientProfile { get; set; }
        public virtual Employee EmployeeProfile { get; set; }


    }
}
