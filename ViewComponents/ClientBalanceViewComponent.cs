using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Loan_Processing_Inzamam.Data;
using Loan_Processing_Inzamam.Models;

namespace Loan_Processing_Inzamam.ViewComponents
{
    public class ClientBalanceViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClientBalanceViewComponent(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            decimal totalBalance = 0;

            // Only attempt to calculate if they are logged in and actually a Client
            if (User.Identity.IsAuthenticated && HttpContext.User.IsInRole("Client"))
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user != null)
                {
                    var client = await _context.Clients
                        .Include(c => c.BankAccounts)
                        .FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);

                    if (client != null && client.BankAccounts != null)
                    {
                        totalBalance = client.BankAccounts.Sum(b => b.Balance);
                    }
                }
            }

            return View(totalBalance);
        }
    }
}