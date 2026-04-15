using Loan_Processing_Inzamam.Data;
using Loan_Processing_Inzamam.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Loan_Processing_Inzamam.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AccountCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AccountCategories
        public async Task<IActionResult> Index()
        {
            return View(await _context.AccountCategories.ToListAsync());
        }

        // GET: AccountCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountCategory = await _context.AccountCategories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (accountCategory == null)
            {
                return NotFound();
            }

            return View(accountCategory);
        }

        // GET: AccountCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AccountCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,CategoryName,IsActive")] AccountCategory accountCategory)
        {
            ModelState.Remove("BankAccounts");
            if (ModelState.IsValid)
            {
                _context.Add(accountCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(accountCategory);
        }

        // GET: AccountCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountCategory = await _context.AccountCategories.FindAsync(id);
            if (accountCategory == null)
            {
                return NotFound();
            }
            return View(accountCategory);
        }

        // POST: AccountCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,CategoryName,IsActive")] AccountCategory accountCategory)
        {
            if (id != accountCategory.CategoryId)
            {
                return NotFound();
            }
            ModelState.Remove("BankAccounts");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(accountCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountCategoryExists(accountCategory.CategoryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(accountCategory);
        }

        // GET: AccountCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountCategory = await _context.AccountCategories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (accountCategory == null)
            {
                return NotFound();
            }

            return View(accountCategory);
        }

        // POST: AccountCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var accountCategory = await _context.AccountCategories.FindAsync(id);
            if (accountCategory != null)
            {
                _context.AccountCategories.Remove(accountCategory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountCategoryExists(int id)
        {
            return _context.AccountCategories.Any(e => e.CategoryId == id);
        }
    }
}
