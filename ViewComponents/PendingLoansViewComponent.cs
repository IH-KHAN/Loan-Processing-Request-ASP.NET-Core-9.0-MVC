using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Loan_Processing_Inzamam.Data;

namespace Loan_Processing_Inzamam.ViewComponents
{
    public class PendingLoansViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public PendingLoansViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Count all loans with the status "Pending"
            int pendingCount = await _context.LoanRequests.CountAsync(l => l.Status == "Pending");

            return View(pendingCount);
        }
    }
}