using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Loan_Processing_Inzamam.Data;
using Loan_Processing_Inzamam.Models;
using Loan_Processing_Inzamam.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using System;

namespace Loan_Processing_Inzamam.Controllers
{
    [Authorize] // Ensures only logged-in users can access this controller
    public class ClientController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ClientController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment; // Used to get the path to wwwroot for saving photos
        }

        // GET: /Client/CreateProfile
        [HttpGet]
        public async Task<IActionResult> CreateProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            // If they already completed their profile, push them to the dashboard
            if (user.IsProfileComplete)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new ClientProfileViewModel());
        }

        // POST: /Client/CreateProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProfile(ClientProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // If employment is checked, manually validate the employment fields
            if (model.IsEmployed)
            {
                if (string.IsNullOrWhiteSpace(model.EmployerName))
                    ModelState.AddModelError("EmployerName", "Employer Name is required if employed.");
                if (string.IsNullOrWhiteSpace(model.Occupation))
                    ModelState.AddModelError("Occupation", "Occupation is required if employed.");
                if (!model.YearsOfExperience.HasValue)
                    ModelState.AddModelError("YearsOfExperience", "Years of Experience is required if employed.");
            }

            if (ModelState.IsValid)
            {
                string uniqueFileName = null;

                // 1. Handle the Profile Photo Upload
                if (model.PhotoUpload != null)
                {
                    // Ensure the folder exists
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "clients");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.PhotoUpload.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.PhotoUpload.CopyToAsync(fileStream);
                    }
                }

                // 2. Map ViewModel to the Client Model
                var newClient = new Client
                {
                    ApplicationUserId = user.Id,
                    Name = model.Name,
                    Phone = model.Phone,
                    DateOfBirth = model.DateOfBirth,
                    MaritalStatus = model.MaritalStatus,
                    PresentAddress = model.PresentAddress,
                    IsSameAddress = model.IsSameAddress,
                    // If IsSameAddress is true, copy the present address over
                    PermanentAddress = model.IsSameAddress ? model.PresentAddress : model.PermanentAddress,
                    NIDNumber = model.NIDNumber,
                    IsEmployed = model.IsEmployed,
                    PhotoPath = uniqueFileName,
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                // 3. Handle Employment Details
                if (model.IsEmployed)
                {
                    newClient.EmploymentDetail = new EmploymentDetail
                    {
                        EmployerName = model.EmployerName,
                        Occupation = model.Occupation,
                        YearsOfExperience = model.YearsOfExperience.Value
                    };
                }

                // 4. Save to Database
                _context.Clients.Add(newClient);

                // 5. Update the User flag
                user.IsProfileComplete = true;
                await _userManager.UpdateAsync(user);

                await _context.SaveChangesAsync();

                // Redirect to the dashboard once complete
                return RedirectToAction("Index", "Home");
            }

            // If ModelState is invalid, return the form with validation errors
            return View(model);
        }
        // GET: /Client/Dashboard
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // Fetch the Client's profile, including their accounts and loans
            var clientProfile = await _context.Clients
                .Include(c => c.BankAccounts)
                .Include(c => c.LoanRequests)
                    .ThenInclude(l => l.LoanType) // Includes the specific type of loan requested
                .Include(c => c.LoanRequests)
                    .ThenInclude(l => l.LoanInstallments)
                .FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);

            // Safety net: If they somehow bypassed the Action Filter and don't have a profile
            if (clientProfile == null)
            {
                return RedirectToAction("CreateProfile");
            }

            return View(clientProfile);
        }
        // GET: /Client/EditProfile
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var client = await _context.Clients.FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);
            if (client == null) return RedirectToAction(nameof(CreateProfile));

            var model = new ClientEditViewModel
            {
                ClientId = client.ClientId,
                ApplicationUserId = client.ApplicationUserId,
                Name = client.Name,
                DateOfBirth = client.DateOfBirth,
                NIDNumber = client.NIDNumber,
                PresentAddress = client.PresentAddress,
                PermanentAddress = client.PermanentAddress,
                ExistingPhotoPath = client.PhotoPath
            };

            return View(model);
        }

        // POST: /Client/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(ClientEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var client = await _context.Clients.FindAsync(model.ClientId);
                if (client == null) return NotFound();

                // Security check: ensure the logged-in user actually owns this profile
                var user = await _userManager.GetUserAsync(User);
                if (client.ApplicationUserId != user.Id) return Unauthorized();

                // Update core fields
                client.Name = model.Name;
                client.Phone = model.Phone;
                client.DateOfBirth = model.DateOfBirth;
                client.NIDNumber = model.NIDNumber;
                client.PresentAddress = model.PresentAddress;
                client.PermanentAddress = model.PermanentAddress;

                // Handle photo update if a new one is uploaded
                if (model.PhotoUpload != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "clients");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    // Delete old photo to save space
                    if (!string.IsNullOrEmpty(client.PhotoPath))
                    {
                        string oldImagePath = Path.Combine(uploadsFolder, client.PhotoPath);
                        if (System.IO.File.Exists(oldImagePath)) System.IO.File.Delete(oldImagePath);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.PhotoUpload.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.PhotoUpload.CopyToAsync(fileStream);
                    }
                    client.PhotoPath = uniqueFileName;
                }

                _context.Update(client);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Dashboard));
            }

            return View(model);
        }
    }
}