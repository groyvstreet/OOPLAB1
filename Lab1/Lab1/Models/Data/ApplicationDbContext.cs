using Lab1.Models.Entities;
using Lab1.Models.Entities.Actions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Lab1.Models.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Specialist> Specialists { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Deposit> Deposits { get; set; }
        public DbSet<Balance> Balances { get; set; }
        public DbSet<Credit> Credits { get; set; }
        public DbSet<Installment> Installments { get; set; }
        public DbSet<SignUpApproving> SignUpApprovings { get; set; }
        public DbSet<CreditApproving> CreditApprovings { get; set; }
        public DbSet<InstallmentApproving> InstallmentApprovings { get; set; }
        public DbSet<SalaryApproving> SalaryApprovings { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<BalanceTransferApproving> BalanceTransferApprovings { get; set; }
        public DbSet<BalanceTransferAction> BalanceTransferActions { get; set; }
        public DbSet<SalaryApprovingBySpecialistAction> SalaryApprovingBySpecialistActions { get; set; }
        public DbSet<SalaryRejectingBySpecialistAction> SalaryRejectingBySpecialistActions { get; set; }
        public DbSet<BlockDepositAction> BlockDepositActions { get; set; }
        public DbSet<FreezeDepositAction> FreezeDepositActions { get; set; }
        public DbSet<UnblockDepositAction> UnblockDepositActions { get; set; }
        public DbSet<UnfreezeDepositAction> UnfreezeDepositActions { get; set; }
        public DbSet<CreateDepositAction> CreateDepositActions { get; set; }
        public DbSet<GetDepositAction> GetDepositActions { get; set; }
        public DbSet<AddDepositAction> AddDepositActions { get; set; }
        public DbSet<TransferDepositAction> TransferDepositActions { get; set; }
        public DbSet<CreateSalaryAction> CreateSalaryActions { get; set; }
        public DbSet<GetSalaryAction> GetSalaryActions { get; set; }
        public DbSet<SalaryApprovingByOperatorAction> SalaryApprovingByOperatorActions { get; set; }
        public DbSet<SalaryRejectingByOperatorAction> SalaryRejectingByOperatorActions { get; set; }
        public DbSet<OpenBalanceAction> OpenBalanceActions { get; set; }
        public DbSet<CloseBalanceAction> CloseBalanceActions { get; set; }
        public DbSet<AddBalanceAction> AddBalanceActions { get; set; }
        public DbSet<CreateInstallmentAction> CreateInstallmentActions { get; set;}
        public DbSet<PayInstallmentAction> PayInstallmentActions { get; set;}
        public DbSet<CreateCreditAction> CreateCreditActions { get; set; }
        public DbSet<PayCreditAction> PayCreditActions { get; set; }
        public DbSet<Entities.Actions.Action> Actions { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Role adminRole = new Role { Id = "1", Name = "admin" };
            Role clientRole = new Role { Id = "2", Name = "client" };
            Role managerRole = new Role { Id = "3", Name = "manager" };
            Role operatorRole = new Role { Id = "4", Name = "operator" };
            Role specialistRole = new Role { Id = "5", Name = "specialist" };

            modelBuilder.Entity<Role>().HasData(adminRole, clientRole, managerRole, operatorRole, specialistRole);
            base.OnModelCreating(modelBuilder);
        }
    }
}
