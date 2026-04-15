using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Loan_Processing_Inzamam.Data;
using Loan_Processing_Inzamam.Models;
using Loan_Processing_Inzamam.ViewModels;

namespace Loan_Processing_Inzamam.Controllers
{
    [Authorize(Roles = "Client")]
    public class BankAccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BankAccountController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /BankAccount/Create
        [HttpGet]
        public IActionResult Create()
        {
            // Load only active account categories into the dropdown
            ViewBag.Categories = new SelectList(_context.AccountCategories.Where(c => c.IsActive), "CategoryId", "CategoryName");
            return View(new BankAccountCreateViewModel());
        }

        // POST: /BankAccount/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BankAccountCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. Identify the logged-in user and find their Client Profile
                var user = await _userManager.GetUserAsync(User);
                var client = await _context.Clients.FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);

                if (client == null) return RedirectToAction("CreateProfile", "Client");

                // 2. Auto-generate a random 10-digit account number (starting with 100 for SecureBank)
                Random random = new Random();
                string generatedAccountNumber = "100" + random.Next(1000000, 9999999).ToString();

                // 3. Create the Bank Account entity
                var bankAccount = new BankAccount
                {
                    ClientId = client.ClientId,
                    CategoryId = model.CategoryId,
                    Balance = model.InitialDeposit,
                    AccountNumber = generatedAccountNumber,
                    CreatedAt = DateTime.Now
                };

                // 4. Save to Database
                _context.BankAccounts.Add(bankAccount);
                await _context.SaveChangesAsync();

                // 5. Redirect back to Dashboard
                return RedirectToAction("Dashboard", "Client");
            }

            // If validation fails, reload dropdown
            ViewBag.Categories = new SelectList(_context.AccountCategories.Where(c => c.IsActive), "CategoryId", "CategoryName", model.CategoryId);
            return View(model);
        }
    }
}