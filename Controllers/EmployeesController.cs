using Loan_Processing_Inzamam.Data;
using Loan_Processing_Inzamam.Models;
using Loan_Processing_Inzamam.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Loan_Processing_Inzamam.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmployeesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employees
                .Include(e => e.Designation)
                .Include(e => e.Branch)
                .Include(e => e.EducationalQualifications)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            return View(employees);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Designations = new SelectList(_context.Designations.Where(d => d.IsActive), "DesignationId", "Title");
            ViewBag.Branches = new SelectList(_context.Branches, "BranchId", "BranchName");

            return PartialView(new EmployeeCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeCreateViewModel model)
        {
            if (model.IsSameAddress)
            {
                ModelState.Remove("PermanentAddress");
                model.PermanentAddress = model.PresentAddress;
            }

            if (ModelState.IsValid)
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // 1. Create Identity User
                    var user = new ApplicationUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        EmailConfirmed = true,
                        IsProfileComplete = true
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (!result.Succeeded)
                    {
                        var identityErrors = result.Errors.Select(e => e.Description).ToList();
                        return Json(new { success = false, errors = identityErrors });
                    }

                    await _userManager.AddToRoleAsync(user, "Employee");

                    // 2. Handle Photo Upload
                    string uniqueFileName = null;
                    if (model.PhotoUpload != null)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "employees");
                        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                        uniqueFileName = Guid.NewGuid().ToString() + "_" + model.PhotoUpload.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.PhotoUpload.CopyToAsync(fileStream);
                        }
                    }

                    // 3. Call Insert Stored Procedure
                    var employeeIdParam = new SqlParameter("@NewEmployeeId", SqlDbType.Int) { Direction = ParameterDirection.Output };

                    await _context.Database.ExecuteSqlRawAsync(@"
                        EXEC sp_InsertEmployee 
                            @ApplicationUserId, @Name, @DesignationId, @Phone, @DateOfBirth, @PhotoPath, 
                            @MaritalStatus, @PresentAddress, @PermanentAddress, @IsSameAddress, 
                            @NIDNumber, @IsActive, @ServiceCategory, @BranchId, @CreatedAt, 
                            @NewEmployeeId OUTPUT",
                        new SqlParameter("@ApplicationUserId", user.Id),
                        new SqlParameter("@Name", model.Name),
                        new SqlParameter("@DesignationId", model.DesignationId),
                        new SqlParameter("@Phone", model.Phone),
                        new SqlParameter("@DateOfBirth", model.DateOfBirth),
                        new SqlParameter("@PhotoPath", (object)uniqueFileName ?? DBNull.Value),
                        new SqlParameter("@MaritalStatus", model.MaritalStatus),
                        new SqlParameter("@PresentAddress", model.PresentAddress),
                        new SqlParameter("@PermanentAddress", (object)model.PermanentAddress ?? DBNull.Value),
                        new SqlParameter("@IsSameAddress", model.IsSameAddress),
                        new SqlParameter("@NIDNumber", model.NIDNumber),
                        new SqlParameter("@IsActive", true),
                        new SqlParameter("@ServiceCategory", (object)model.ServiceCategory ?? DBNull.Value),
                        new SqlParameter("@BranchId", (object)model.BranchId ?? DBNull.Value),
                        new SqlParameter("@CreatedAt", DateTime.Now),
                        employeeIdParam);

                    int newlyCreatedEmployeeId = (int)employeeIdParam.Value;

                    // 4. Call Qualifications Stored Procedure
                    if (model.Qualifications != null && model.Qualifications.Any(q => q != null && !string.IsNullOrWhiteSpace(q.DegreeName)))
                    {
                        foreach (var qual in model.Qualifications.Where(q => q != null && !string.IsNullOrWhiteSpace(q.DegreeName)))
                        {
                            await _context.Database.ExecuteSqlRawAsync(@"
                                EXEC sp_InsertQualification 
                                    @DegreeName, @InstitutionName, @PassingYear, @GradingScore, @OutOf, @EmployeeId",
                                new SqlParameter("@DegreeName", qual.DegreeName),
                                new SqlParameter("@InstitutionName", qual.InstitutionName),
                                new SqlParameter("@PassingYear", qual.PassingYear),
                                new SqlParameter("@GradingScore", qual.GradingScore),
                                new SqlParameter("@OutOf", qual.OutOf),
                                new SqlParameter("@EmployeeId", newlyCreatedEmployeeId));
                        }
                    }

                    await transaction.CommitAsync();

                    return Json(new { success = true, message = "Employee onboarded successfully!" });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return Json(new { success = false, errors = new[] { "A database error occurred: " + ex.Message } });
                }
            }

            var validationErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return Json(new { success = false, errors = validationErrors });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var employee = await _context.Employees
                .Include(e => e.EducationalQualifications)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null) return NotFound();

            var model = new EmployeeEditViewModel
            {
                EmployeeId = employee.EmployeeId,
                ApplicationUserId = employee.ApplicationUserId,
                Name = employee.Name,
                DesignationId = employee.DesignationId,
                Phone = employee.Phone,
                BranchId = employee.BranchId,
                ServiceCategory = employee.ServiceCategory,
                ExistingPhotoPath = employee.PhotoPath,
                Qualifications = employee.EducationalQualifications.Select(q => new QualificationViewModel
                {
                    DegreeName = q.DegreeName,
                    InstitutionName = q.InstitutionName,
                    PassingYear = q.PassingYear,
                    GradingScore = q.GradingScore,
                    OutOf = q.OutOf
                }).ToList()
            };

            ViewBag.Designations = new SelectList(_context.Designations.Where(d => d.IsActive), "DesignationId", "Title", employee.DesignationId);
            ViewBag.Branches = new SelectList(_context.Branches, "BranchId", "BranchName", employee.BranchId);

            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeeEditViewModel model)
        {
            if (id != model.EmployeeId) return Json(new { success = false, errors = new[] { "Invalid Data: ID mismatch." } });

            if (ModelState.IsValid)
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    string uniqueFileName = model.ExistingPhotoPath;

                    // Handle Photo Upload physically
                    if (model.PhotoUpload != null)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "employees");
                        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                        if (!string.IsNullOrEmpty(model.ExistingPhotoPath))
                        {
                            string oldImagePath = Path.Combine(uploadsFolder, model.ExistingPhotoPath);
                            if (System.IO.File.Exists(oldImagePath)) System.IO.File.Delete(oldImagePath);
                        }

                        uniqueFileName = Guid.NewGuid().ToString() + "_" + model.PhotoUpload.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.PhotoUpload.CopyToAsync(fileStream);
                        }
                    }

                    // Update Employee via Stored Procedure
                    await _context.Database.ExecuteSqlRawAsync(@"
                        EXEC sp_UpdateEmployee 
                            @EmployeeId, @Name, @DesignationId, @Phone, @ServiceCategory, @BranchId, @PhotoPath",
                        new SqlParameter("@EmployeeId", model.EmployeeId),
                        new SqlParameter("@Name", model.Name),
                        new SqlParameter("@DesignationId", model.DesignationId),
                        new SqlParameter("@Phone", model.Phone),
                        new SqlParameter("@ServiceCategory", (object)model.ServiceCategory ?? DBNull.Value),
                        new SqlParameter("@BranchId", (object)model.BranchId ?? DBNull.Value),
                        new SqlParameter("@PhotoPath", (object)uniqueFileName ?? DBNull.Value));

                    // Wipe existing qualifications for this employee via SP
                    await _context.Database.ExecuteSqlRawAsync("EXEC sp_DeleteQualificationsByEmployeeId @EmployeeId",
                        new SqlParameter("@EmployeeId", model.EmployeeId));

                    // Insert the new list of qualifications via SP
                    if (model.Qualifications != null && model.Qualifications.Any(q => q != null && !string.IsNullOrWhiteSpace(q.DegreeName)))
                    {
                        foreach (var qual in model.Qualifications.Where(q => q != null && !string.IsNullOrWhiteSpace(q.DegreeName)))
                        {
                            await _context.Database.ExecuteSqlRawAsync(@"
                                EXEC sp_InsertQualification 
                                    @DegreeName, @InstitutionName, @PassingYear, @GradingScore, @OutOf, @EmployeeId",
                                new SqlParameter("@DegreeName", qual.DegreeName),
                                new SqlParameter("@InstitutionName", qual.InstitutionName),
                                new SqlParameter("@PassingYear", qual.PassingYear),
                                new SqlParameter("@GradingScore", qual.GradingScore),
                                new SqlParameter("@OutOf", qual.OutOf),
                                new SqlParameter("@EmployeeId", model.EmployeeId));
                        }
                    }

                    await transaction.CommitAsync();

                    return Json(new { success = true, message = "Employee profile updated successfully!" });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return Json(new { success = false, errors = new[] { "Failed to update employee: " + ex.Message } });
                }
            }

            var validationErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return Json(new { success = false, errors = validationErrors });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee != null)
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // 1. Delete the actual Login Account (ApplicationUser)
                    var user = await _userManager.FindByIdAsync(employee.ApplicationUserId);
                    if (user != null)
                    {
                        var identityResult = await _userManager.DeleteAsync(user);
                        if (!identityResult.Succeeded) throw new Exception("Failed to delete Identity User.");
                    }

                    // 2. Call Delete Stored Procedure 
                    await _context.Database.ExecuteSqlRawAsync("EXEC sp_DeleteEmployee @EmployeeId",
                        new SqlParameter("@EmployeeId", id));

                    // 3. Delete physical photo from server
                    if (!string.IsNullOrEmpty(employee.PhotoPath))
                    {
                        string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "employees", employee.PhotoPath);
                        if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                    }

                    await transaction.CommitAsync();
                    TempData["SuccessMessage"] = "Employee permanently deleted.";
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = "Failed to delete employee. " + ex.Message;
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Employee not found.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}