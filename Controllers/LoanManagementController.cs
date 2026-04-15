using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Loan_Processing_Inzamam.Data;
using Loan_Processing_Inzamam.Models;
using Loan_Processing_Inzamam.ViewModels;

namespace Loan_Processing_Inzamam.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class LoanManagementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public LoanManagementController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /LoanManagement/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var loans = await _context.LoanRequests
                .Include(l => l.Client)
                .Include(l => l.LoanType)
                .OrderByDescending(l => l.RequestDate)
                .ToListAsync();

            return View(loans);
        }

        // GET: /LoanManagement/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var loan = await _context.LoanRequests
                .Include(l => l.Client)
                .Include(l => l.LoanType)
                .Include(l => l.Guarantors)
                .Include(l => l.ApprovedByEmployee)
                .FirstOrDefaultAsync(l => l.LoanRequestId == id);

            if (loan == null) return NotFound();

            // Store the loan details in ViewBag to display on the page
            ViewBag.LoanDetails = loan;

            // Prepare the view model for the form at the bottom
            var model = new LoanProcessViewModel
            {
                LoanRequestId = loan.LoanRequestId
            };

            return View(model);
        }

        // POST: /LoanManagement/ProcessLoan
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessLoan(LoanProcessViewModel model)
        {
            if (ModelState.IsValid)
            {
                var loan = await _context.LoanRequests.FindAsync(model.LoanRequestId);
                if (loan == null) return NotFound();

                // Find the employee making this decision
                var user = await _userManager.GetUserAsync(User);
                var employee = await _context.Employees.FirstOrDefaultAsync(e => e.ApplicationUserId == user.Id);

                // Update the loan record
                loan.Status = model.Status;
                loan.AdminRemarks = model.AdminRemarks;
                loan.ActionDate = DateTime.Now;

                // If an actual employee processed it, link their ID. (Admins might not have employee profiles)
                if (employee != null)
                {
                    loan.ApprovedByEmployeeId = employee.EmployeeId;
                }

                _context.Update(loan);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // If validation fails, reload the page data
            var reloadLoan = await _context.LoanRequests
                .Include(l => l.Client)
                .Include(l => l.LoanType)
                .Include(l => l.Guarantors)
                .FirstOrDefaultAsync(l => l.LoanRequestId == model.LoanRequestId);

            ViewBag.LoanDetails = reloadLoan;
            return View("Details", model);
        }
    }
}