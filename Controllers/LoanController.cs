using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Loan_Processing_Inzamam.Data;
using Loan_Processing_Inzamam.Models;
using Loan_Processing_Inzamam.ViewModels;

namespace Loan_Processing_Inzamam.Controllers
{
    [Authorize(Roles = "Client")]
    public class LoanController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LoanController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult RequestLoan()
        {
            // Populate the dropdown with active loan types
            ViewBag.LoanTypes = new SelectList(_context.LoanTypes.Where(l => l.IsActive), "LoanTypeId", "TypeName");
            return View(new LoanRequestViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestLoan(LoanRequestViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. Identify the logged-in user and find their Client Profile
                var user = await _userManager.GetUserAsync(User);
                var client = await _context.Clients.FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);

                if (client == null)
                {
                    return RedirectToAction("CreateProfile", "Client"); // Safety net
                }

                // 2. Handle the TIN Document Upload
                string uniqueFileName = null;
                if (model.TINFileUpload != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "documents", "tin_certificates");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.TINFileUpload.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.TINFileUpload.CopyToAsync(fileStream);
                    }
                }

                // 3. Map Data to LoanRequest Entity
                var loanRequest = new LoanRequest
                {
                    ClientId = client.ClientId,
                    LoanTypeId = model.LoanTypeId,
                    DesiredLoanAmount = model.DesiredLoanAmount,
                    AnnualIncome = model.AnnualIncome,
                    LoanTermMonths = model.LoanTermMonths,
                    LoanPurpose = model.LoanPurpose,
                    TINFilePath = uniqueFileName,
                    RequestDate = DateTime.Now,
                    Status = "Pending",
                    AdminRemarks = ""
                };

                // 4. Map the Guarantors
                if (model.Guarantors != null && model.Guarantors.Any())
                {
                    loanRequest.Guarantors = model.Guarantors.Select(g => new Guarantor
                    {
                        Name = g.Name,
                        Relationship = g.Relationship,
                        ContactAddress = g.ContactAddress,
                        Phone = g.Phone
                    }).ToList();
                }

                // 5. Save to Database
                _context.LoanRequests.Add(loanRequest);
                await _context.SaveChangesAsync();

                // Redirect back to their dashboard so they can see it in the table
                return RedirectToAction("Dashboard", "Client");
            }

            // If validation fails, reload the dropdown and return to form
            ViewBag.LoanTypes = new SelectList(_context.LoanTypes.Where(l => l.IsActive), "LoanTypeId", "TypeName", model.LoanTypeId);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> PayInstallment(int loanRequestId)
        {
            var user = await _userManager.GetUserAsync(User);
            var client = await _context.Clients
                .Include(c => c.BankAccounts)
                .FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);

            if (client == null) return NotFound();

            var loanRequest = await _context.LoanRequests
                .Include(l => l.LoanType)
                .Include(l => l.LoanInstallments)
                .FirstOrDefaultAsync(l => l.LoanRequestId == loanRequestId && l.ClientId == client.ClientId);

            if (loanRequest == null) return NotFound();
            if (loanRequest.Status != "Approved") return BadRequest("Installments can only be paid on approved loans.");

            if (!client.BankAccounts.Any())
            {
                TempData["ErrorMessage"] = "You need a bank account with sufficient funds to pay an installment.";
                return RedirectToAction("Dashboard", "Client");
            }

            // Calculate total paid so far
            decimal totalPaid = loanRequest.LoanInstallments.Sum(i => i.AmountPaid);
            decimal remainingBalance = loanRequest.DesiredLoanAmount - totalPaid;

            ViewBag.LoanRequest = loanRequest;
            ViewBag.TotalPaid = totalPaid;
            ViewBag.RemainingBalance = remainingBalance;
            ViewBag.BankAccounts = new SelectList(client.BankAccounts.Select(a => new {
                AccountId = a.AccountId,
                DisplayText = $"{a.AccountNumber} (Balance: ৳{a.Balance:N2})"
            }), "AccountId", "DisplayText");

            var model = new PayInstallmentViewModel
            {
                LoanRequestId = loanRequestId,
                Amount = Math.Min(remainingBalance, 1000m)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayInstallment(PayInstallmentViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var client = await _context.Clients
                .Include(c => c.BankAccounts)
                .FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);

            if (client == null) return NotFound();

            var loanRequest = await _context.LoanRequests
                .Include(l => l.LoanInstallments)
                .FirstOrDefaultAsync(l => l.LoanRequestId == model.LoanRequestId && l.ClientId == client.ClientId);

            if (loanRequest == null) return NotFound();
            if (loanRequest.Status != "Approved") return BadRequest("Installments can only be paid on approved loans.");

            var account = client.BankAccounts.FirstOrDefault(a => a.AccountId == model.BankAccountId);
            if (account == null)
            {
                ModelState.AddModelError("BankAccountId", "Selected bank account was not found.");
            }
            else if (account.Balance < model.Amount)
            {
                ModelState.AddModelError("Amount", $"Insufficient funds. Selected account balance is ৳{account.Balance:N2}");
            }

            decimal totalPaid = loanRequest.LoanInstallments.Sum(i => i.AmountPaid);
            decimal remainingBalance = loanRequest.DesiredLoanAmount - totalPaid;

            if (model.Amount > remainingBalance)
            {
                ModelState.AddModelError("Amount", $"Payment amount (৳{model.Amount:N2}) exceeds the remaining loan balance (৳{remainingBalance:N2}).");
            }

            if (ModelState.IsValid)
            {
                // Deduct from bank account
                account.Balance -= model.Amount;
                _context.BankAccounts.Update(account);

                // Create loan installment
                var installment = new LoanInstallment
                {
                    LoanRequestId = loanRequest.LoanRequestId,
                    AmountPaid = model.Amount,
                    PaymentDate = DateTime.Now,
                    PaymentMethod = "Bank Account",
                    BankAccountId = account.AccountId
                };
                _context.LoanInstallments.Add(installment);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Successfully paid installment of ৳{model.Amount:N2} from account #{account.AccountNumber}.";
                return RedirectToAction("Dashboard", "Client");
            }

            // Reload ViewBag data if model is invalid
            ViewBag.LoanRequest = loanRequest;
            ViewBag.TotalPaid = totalPaid;
            ViewBag.RemainingBalance = remainingBalance;
            ViewBag.BankAccounts = new SelectList(client.BankAccounts.Select(a => new {
                AccountId = a.AccountId,
                DisplayText = $"{a.AccountNumber} (Balance: ৳{a.Balance:N2})"
            }), "AccountId", "DisplayText", model.BankAccountId);

            return View(model);
        }
    }
}