using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Loan_Processing_Inzamam.Data;
using Loan_Processing_Inzamam.Models;
using Loan_Processing_Inzamam.ViewModels;

namespace Loan_Processing_Inzamam.Controllers
{
    [Authorize(Roles = "Client")]
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TransactionController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> MakeTransaction()
        {
            var user = await _userManager.GetUserAsync(User);
            var client = await _context.Clients
                .Include(c => c.BankAccounts)
                .FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);

            // If they don't have any accounts, send them to the account creation page
            if (client == null || !client.BankAccounts.Any())
            {
                return RedirectToAction("Create", "BankAccount");
            }

            // Create a dropdown list of their specific accounts showing the Account Number
            ViewBag.Accounts = new SelectList(client.BankAccounts, "AccountId", "AccountNumber");
            return View(new TransactionViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeTransaction(TransactionViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var client = await _context.Clients
                .Include(c => c.BankAccounts)
                .FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);

            if (client == null) return NotFound();

            if (ModelState.IsValid)
            {
                // Find the specific account the user selected
                var account = client.BankAccounts.FirstOrDefault(a => a.AccountId == model.AccountId);

                if (account == null) return NotFound();

                // Validation: Check for insufficient funds during a withdrawal
                if (model.TransactionType == "Withdraw" && account.Balance < model.Amount)
                {
                    ModelState.AddModelError("Amount", "Insufficient funds for this withdrawal. Your current balance is ৳" + account.Balance.ToString("N2"));
                }
                else
                {
                    // Process the transaction
                    if (model.TransactionType == "Deposit")
                    {
                        account.Balance += model.Amount;
                    }
                    else if (model.TransactionType == "Withdraw")
                    {
                        account.Balance -= model.Amount;
                    }

                    _context.Update(account);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Dashboard", "Client");
                }
            }

            // If validation fails, reload the dropdown
            ViewBag.Accounts = new SelectList(client.BankAccounts, "AccountId", "AccountNumber", model.AccountId);
            return View(model);
        }
    }
}