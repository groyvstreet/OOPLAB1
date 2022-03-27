using Lab1.Models.Data;
using Lab1.Models.Entities;
using Lab1.Models.ManagerModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab1.Controllers
{
    public class ManagerController : Controller
    {
        private ApplicationDbContext _context;

        public ManagerController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "manager")]
        public async Task<IActionResult> Profile()
        {
            //var manager = await _context.Managers.FirstOrDefaultAsync(m => m.Id == User.Identity.Name);
            var manager = await _context.Users.FirstOrDefaultAsync(m => m.Id == User.Identity.Name);
            var bank = await _context.Banks.FirstOrDefaultAsync(b => b.Id == manager.BankId);
            var model = new ManagerProfileModel
            {
                Email = manager.Email,
                FirstName = manager.FirstName,
                LastName = manager.LastName,
                Patronymic = manager.Patronymic,
                PhoneNumber = manager.PhoneNumber,
                BankName = bank.Name
            };
            return View(model);
        }

        [Authorize(Roles = "manager")]
        public async Task<IActionResult> SignUp()
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == User.Identity.Name);
            var manager = new Manager
            {
                Id = user.Id,
                Email = user.Email,
                Password = user.Password,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Patronymic = user.Patronymic,
                PhoneNumber = user.PhoneNumber,
                RoleName = user.RoleName,
                BankId = user.BankId
            };
            //_context.Users.Remove(user);
            //_context.Managers.Add(manager);
            await _context.SaveChangesAsync();
            return RedirectToAction("Profile", "Manager");
        }

        [Authorize(Roles = "manager")]
        public async Task<IActionResult> SignUpApprovings()
        {
            //var manager = await _context.Managers.FirstOrDefaultAsync(m => m.Id == User.Identity.Name);
            var manager = await _context.Users.FirstOrDefaultAsync(m => m.Id == User.Identity.Name);
            var signUpApprovings = _context.SignUpApprovings;
            var clients = new List<Client>();
            foreach(var approving in signUpApprovings)
            {
                var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == approving.ClientId
                    && c.BankId == manager.BankId);
                if (client != null)
                {
                    clients.Add(client);
                }
            }
            SignUpApprovingsManagerModel model = new SignUpApprovingsManagerModel
            {
                Clients = clients
            };
            return View(model);
        }

        [Authorize(Roles = "manager")]
        public async Task<IActionResult> ApproveSignUp(string clientId)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id.Equals(clientId));
            client.Approved = true;
            _context.Clients.Update(client);
            var signUpApproving = await _context.SignUpApprovings.FirstOrDefaultAsync(s =>
                s.ClientId == clientId);
            _context.SignUpApprovings.Remove(signUpApproving);
            await _context.SaveChangesAsync();
            return RedirectToAction("SignUpApprovings", "Manager");
        }

        [Authorize(Roles = "manager")]
        public async Task<IActionResult> RejectSignUp(string clientId)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id.Equals(clientId));
            _context.Clients.Remove(client);
            var signUpapproving = await _context.SignUpApprovings.FirstOrDefaultAsync(s =>
                s.ClientId == clientId);
            _context.SignUpApprovings.Remove(signUpapproving);
            await _context.SaveChangesAsync();
            return RedirectToAction("SignUpApprovings", "Manager");
        }

        [Authorize(Roles = "manager")]
        public async Task<IActionResult> CreditApprovings()
        {
            //var manager = await _context.Managers.FirstOrDefaultAsync(m => m.Id == User.Identity.Name);
            var manager = await _context.Users.FirstOrDefaultAsync(m => m.Id == User.Identity.Name);
            var creditApprovings = _context.CreditApprovings;
            var credits = new List<Credit>();
            foreach (var approving in creditApprovings)
            {
                var credit = await _context.Credits.FirstOrDefaultAsync(c => c.Id == approving.CreditId);
                var client = await _context.Clients.FirstOrDefaultAsync(c => c.BankId ==
                    manager.BankId && c.Id == credit.ClientId);
                if (client != null)
                {
                    credits.Add(credit);
                }
            }
            CreditApprovingManagerModel model = new CreditApprovingManagerModel
            {
                Credits = credits
            };
            return View(model);
        }

        [Authorize(Roles = "manager")]
        public async Task<IActionResult> ApproveCredit(string creditId)
        {
            var credit = await _context.Credits.FirstOrDefaultAsync(c => c.Id.Equals(creditId));
            var creditApproving = await _context.CreditApprovings.FirstOrDefaultAsync(c => c.CreditId ==
                creditId);
            var client = await _context.Clients
                .Include(c => c.Credits)
                .Include(c => c.Balances)
                .FirstOrDefaultAsync(c => c.Id == credit.ClientId);
            var balance = await _context.Balances.FirstOrDefaultAsync(b => b.Id ==
                creditApproving.BalanceId);
            client.Credits.Remove(credit);
            client.Balances.Remove(balance);
            credit.Approved = true;
            credit.CreatingTime = DateTime.Now;
            credit.PaymentTime = credit.CreatingTime.AddMonths(credit.Months);
            balance.Money += credit.Money;
            client.Credits.Add(credit);
            client.Balances.Add(balance);
            _context.Clients.Update(client);
            _context.Credits.Update(credit);
            _context.Balances.Update(balance);
            _context.CreditApprovings.Remove(creditApproving);
            await _context.SaveChangesAsync();
            return RedirectToAction("CreditApprovings", "Manager");
        }

        [Authorize(Roles = "manager")]
        public async Task<IActionResult> RejectCredit(string creditId)
        {
            var credit = await _context.Credits.FirstOrDefaultAsync(c => c.Id.Equals(creditId));
            var creditApproving = await _context.CreditApprovings.FirstOrDefaultAsync(c => c.CreditId ==
                creditId);
            var client = await _context.Clients
                .Include(c => c.Credits)
                .FirstOrDefaultAsync(c => c.Id == credit.ClientId);
            _context.Credits.Remove(credit);
            _context.CreditApprovings.Remove(creditApproving);
            client.Credits.Remove(credit);
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
            return RedirectToAction("CreditApprovings", "Manager");
        }

        [Authorize(Roles = "manager")]
        public async Task<IActionResult> InstallmentApprovings()
        {
            //var manager = await _context.Managers.FirstOrDefaultAsync(m => m.Id == User.Identity.Name);
            var manager = await _context.Users.FirstOrDefaultAsync(m => m.Id == User.Identity.Name);
            var installmentApprovings = _context.InstallmentApprovings;
            var installments = new List<Installment>();
            foreach (var approving in installmentApprovings)
            {
                var installment = await _context.Installments.FirstOrDefaultAsync(i => i.Id ==
                    approving.InstallmentId);
                var client = await _context.Clients.FirstOrDefaultAsync(c => c.BankId ==
                    manager.BankId && c.Id == installment.ClientId);
                if (client != null)
                {
                    installments.Add(installment);
                }
            }
            var model = new InstallmentApprovingManagerModel
            {
                Installments = installments
            };
            return View(model);
        }

        [Authorize(Roles = "manager")]
        public async Task<IActionResult> ApproveInstallment(string installmentId)
        {
            var installment = await _context.Installments.FirstOrDefaultAsync(c =>
                c.Id.Equals(installmentId));
            var installmentApproving = await _context.InstallmentApprovings.FirstOrDefaultAsync(c =>
                c.InstallmentId == installmentId);
            var client = await _context.Clients
                .Include(c => c.Credits)
                .Include(c => c.Balances)
                .FirstOrDefaultAsync(c => c.Id == installment.ClientId);
            var balance = await _context.Balances.FirstOrDefaultAsync(b => b.Id ==
                installmentApproving.BalanceId);
            client.Installments.Remove(installment);
            client.Balances.Remove(balance);
            installment.Approved = true;
            installment.CreatingTime = DateTime.Now;
            installment.PaymentTime = installment.CreatingTime.AddMonths(installment.Months);
            balance.Money += installment.Money;
            client.Installments.Add(installment);
            client.Balances.Add(balance);
            _context.Clients.Update(client);
            _context.Installments.Update(installment);
            _context.Balances.Update(balance);
            _context.InstallmentApprovings.Remove(installmentApproving);
            await _context.SaveChangesAsync();
            return RedirectToAction("InstallmentApprovings", "Manager");
        }

        [Authorize(Roles = "manager")]
        public async Task<IActionResult> RejectInstallment(string installmentId)
        {
            var installment = await _context.Installments.FirstOrDefaultAsync(i => i.Id.Equals(installmentId));
            var installmentApproving = await _context.InstallmentApprovings.FirstOrDefaultAsync(i =>
                i.InstallmentId == installmentId);
            var client = await _context.Clients
                .Include(c => c.Installments)
                .FirstOrDefaultAsync(c => c.Id == installment.ClientId);
            _context.Installments.Remove(installment);
            _context.InstallmentApprovings.Remove(installmentApproving);
            client.Installments.Remove(installment);
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
            return RedirectToAction("InstallmentApprovings", "Manager");
        }

        [Authorize(Roles = "manager")]
        public async Task<IActionResult> Deposits()
        {
            //var manager = await _context.Managers.FirstOrDefaultAsync(m => m.Id == User.Identity.Name);
            var manager = await _context.Users.FirstOrDefaultAsync(m => m.Id == User.Identity.Name);
            var clients = _context.Clients
                .Include(c => c.Deposits)
                .Where(c => c.BankId == manager.BankId && c.Deposits.Any()).ToList();
            var model = new DepositsManagerModel
            {
                Clients = clients
            };
            return View(model);
        }

        [Authorize(Roles = "manager")]
        public async Task<IActionResult> Block(string? depositId)
        {
            var deposit = await _context.Deposits.FirstOrDefaultAsync(d => d.Id == depositId);
            var client = await _context.Clients
                .Include(c => c.Deposits)
                .FirstOrDefaultAsync(c => c.Id == deposit.ClientId);
            client.Deposits.Remove(deposit);
            deposit.Blocked = true;
            client.Deposits.Add(deposit);
            _context.Clients.Update(client);
            _context.Deposits.Update(deposit);
            await _context.SaveChangesAsync();
            return RedirectToAction("Deposits", "Manager");
        }

        [Authorize(Roles = "manager")]
        public async Task<IActionResult> Unblock(string? depositId)
        {
            var deposit = await _context.Deposits.FirstOrDefaultAsync(d => d.Id == depositId);
            var client = await _context.Clients
                .Include(c => c.Deposits)
                .FirstOrDefaultAsync(c => c.Id == deposit.ClientId);
            client.Deposits.Remove(deposit);
            deposit.Blocked = false;
            client.Deposits.Add(deposit);
            _context.Clients.Update(client);
            _context.Deposits.Update(deposit);
            await _context.SaveChangesAsync();
            return RedirectToAction("Deposits", "Manager");
        }

        [Authorize(Roles = "manager")]
        public async Task<IActionResult> Freeze(string? depositId)
        {
            var deposit = await _context.Deposits.FirstOrDefaultAsync(d => d.Id == depositId);
            var client = await _context.Clients
                .Include(c => c.Deposits)
                .FirstOrDefaultAsync(c => c.Id == deposit.ClientId);
            client.Deposits.Remove(deposit);
            deposit.Freezed = true;
            client.Deposits.Add(deposit);
            _context.Clients.Update(client);
            _context.Deposits.Update(deposit);
            await _context.SaveChangesAsync();
            return RedirectToAction("Deposits", "Manager");
        }

        [Authorize(Roles = "manager")]
        public async Task<IActionResult> Unfreeze(string? depositId)
        {
            var deposit = await _context.Deposits.FirstOrDefaultAsync(d => d.Id == depositId);
            var client = await _context.Clients
                .Include(c => c.Deposits)
                .FirstOrDefaultAsync(c => c.Id == deposit.ClientId);
            client.Deposits.Remove(deposit);
            deposit.Freezed = false;
            client.Deposits.Add(deposit);
            _context.Clients.Update(client);
            _context.Deposits.Update(deposit);
            await _context.SaveChangesAsync();
            return RedirectToAction("Deposits", "Manager");
        }
    }
}
