using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Loan_Processing_Inzamam.Models;
using Loan_Processing_Inzamam.ViewModels;

namespace Loan_Processing_Inzamam.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserManagementController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: /UserManagement/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var userRoles = new List<UserRoleViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles.Add(new UserRoleViewModel
                {
                    UserId = user.Id,
                    Email = user.Email,
                    CurrentRoles = roles
                });
            }

            return View(userRoles);
        }

        // GET: /UserManagement/EditRole/5
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.ToList();

            // Pass the available roles to a dropdown
            ViewBag.Roles = new SelectList(allRoles, "Name", "Name");

            var model = new UserRoleViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                CurrentRoles = userRoles
            };

            return View(model);
        }

        
        // POST: /UserManagement/EditRole/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(UserRoleViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();

            // Force .NET to ignore these missing fields from the HTML form
            ModelState.Remove("Email");
            ModelState.Remove("CurrentRoles");

            if (ModelState.IsValid)
            {
                // Remove all existing roles from the user
                var currentRoles = await _userManager.GetRolesAsync(user);

                if (currentRoles.Any())
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    if (!removeResult.Succeeded)
                    {
                        ModelState.AddModelError("", "Failed to remove old roles");
                        return View(model);
                    }
                }

                // Add the new selected role
                var addResult = await _userManager.AddToRoleAsync(user, model.SelectedRole);

                if (addResult.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in addResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            // Repopulate dropdown if it fails
            var allRoles = _roleManager.Roles.ToList();
            ViewBag.Roles = new SelectList(allRoles, "Name", "Name");
            model.CurrentRoles = await _userManager.GetRolesAsync(user);
            model.Email = user.Email; // Restore the email so the view doesn't break

            return View(model);
        }
    }
}