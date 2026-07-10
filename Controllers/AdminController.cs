using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Loan_Processing_Inzamam.Data;
using Loan_Processing_Inzamam.ViewModels;
using System.Threading.Tasks;
using System.Linq;

namespace Loan_Processing_Inzamam.Controllers
{
    [Authorize(Roles = "Admin")] // Strictly locks this controller to Admins
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            // 1. Existing Count Metrics
            ViewBag.TotalClients = await _context.Clients.CountAsync();
            ViewBag.TotalEmployees = await _context.Employees.CountAsync();
            ViewBag.TotalBranches = await _context.Branches.CountAsync();
            ViewBag.PendingLoans = await _context.LoanRequests.CountAsync(l => l.Status == "Pending");

            // 2. Total Deposited with the bank
            ViewBag.TotalDeposits = await _context.BankAccounts.AnyAsync() 
                ? await _context.BankAccounts.SumAsync(b => b.Balance) 
                : 0m;

            // 3. Dispersed Loans Aggregations
            var approvedLoans = _context.LoanRequests.Where(l => l.Status == "Approved");

            if (await approvedLoans.AnyAsync())
            {
                ViewBag.TotalLoanDispersed = await approvedLoans.SumAsync(l => l.DesiredLoanAmount);
                ViewBag.MaxLoanDispersed = await approvedLoans.MaxAsync(l => l.DesiredLoanAmount);
                ViewBag.MinLoanDispersed = await approvedLoans.MinAsync(l => l.DesiredLoanAmount);
                ViewBag.AverageLoanDispersed = await approvedLoans.AverageAsync(l => l.DesiredLoanAmount);
            }
            else
            {
                ViewBag.TotalLoanDispersed = 0m;
                ViewBag.MaxLoanDispersed = 0m;
                ViewBag.MinLoanDispersed = 0m;
                ViewBag.AverageLoanDispersed = 0m;
            }

            // 4. Clients Financial Summary List
            var clientSummaries = await _context.Clients
                .Select(c => new ClientSummaryViewModel
                {
                    ClientId = c.ClientId,
                    Name = c.Name,
                    Phone = c.Phone,
                    NIDNumber = c.NIDNumber,
                    IsActive = c.IsActive,
                    TotalDeposited = c.BankAccounts.Any() ? c.BankAccounts.Sum(ba => ba.Balance) : 0m,
                    TotalLoanTaken = c.LoanRequests.Where(lr => lr.Status == "Approved").Any() ? c.LoanRequests.Where(lr => lr.Status == "Approved").Sum(lr => lr.DesiredLoanAmount) : 0m,
                    TotalInstallmentsMade = c.LoanRequests.SelectMany(lr => lr.LoanInstallments).Any() ? c.LoanRequests.SelectMany(lr => lr.LoanInstallments).Sum(li => li.AmountPaid) : 0m
                }).ToListAsync();

            ViewBag.ClientSummaries = clientSummaries;

            return View();
        }
    }
}