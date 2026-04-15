using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Loan_Processing_Inzamam.ViewModels
{
    public class UserRoleViewModel
    {
        public string UserId { get; set; }

        public string? Email { get; set; }

        [Display(Name = "Current Roles")]
        public IList<string>? CurrentRoles { get; set; }

        [Required]
        [Display(Name = "Assign New Role")]
        public string SelectedRole { get; set; }
    }
}