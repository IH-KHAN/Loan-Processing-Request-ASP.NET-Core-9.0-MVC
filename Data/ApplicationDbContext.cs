using Loan_Processing_Inzamam.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Loan_Processing_Inzamam.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions <ApplicationDbContext> options): base(options)
        {
            
        }
        public DbSet<Client> Clients { get; set; }
        public DbSet<EmploymentDetail> EmploymentDetails { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EducationalQualification> EducationalQualifications { get; set; }
        public DbSet<Designation> Designations { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<LoanRequest> LoanRequests { get; set; }
        public DbSet<Guarantor> Guarantors { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<LoanType> LoanTypes { get; set; }
        public DbSet<AccountCategory> AccountCategories { get; set; }
        public DbSet<LoanInstallment> LoanInstallments { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

  
            builder.Entity<Client>().Property(c => c.ClientId).UseIdentityColumn(100000, 1);
            builder.Entity<Employee>().Property(e => e.EmployeeId).UseIdentityColumn(200000, 1);
            builder.Entity<LoanRequest>().Property(l => l.LoanRequestId).UseIdentityColumn(300000, 1);
            builder.Entity<BankAccount>().Property(b => b.AccountId).UseIdentityColumn(400000, 1);
            builder.Entity<Branch>().Property(b => b.BranchId).UseIdentityColumn(500000, 1);
            builder.Entity<Designation>().Property(d => d.DesignationId).UseIdentityColumn(600000, 1);
            builder.Entity<LoanType>().Property(l => l.LoanTypeId).UseIdentityColumn(700000, 1);
            builder.Entity<AccountCategory>().Property(a => a.CategoryId).UseIdentityColumn(800000, 1);


            builder.Entity<LoanRequest>().Property(l => l.DesiredLoanAmount).HasPrecision(18, 2);
            builder.Entity<LoanRequest>().Property(l => l.AnnualIncome).HasPrecision(18, 2);
            builder.Entity<BankAccount>().Property(b => b.Balance).HasPrecision(18, 2);
            builder.Entity<LoanType>().Property(l => l.InterestRate).HasPrecision(5, 2);
            builder.Entity<LoanInstallment>().Property(li => li.AmountPaid).HasPrecision(18, 2);


            builder.Entity<Client>()
                .HasOne(c => c.ApplicationUser)
                .WithOne(a => a.ClientProfile)
                .HasForeignKey<Client>(c => c.ApplicationUserId);
            builder.Entity<Employee>()
                .HasOne(e => e.ApplicationUser)
                .WithOne(a => a.EmployeeProfile)
                .HasForeignKey<Employee>(e => e.ApplicationUserId);

            
        }
        
    }
    
}
