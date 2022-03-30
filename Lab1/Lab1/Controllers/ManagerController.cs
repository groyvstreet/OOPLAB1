using Lab1.Models.Data;
using Lab1.Models.Entities;
using Lab1.Models.Entities.Actions;
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
            foreach (var approving in signUpApprovings)
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

        [Authorize(Roles = "manager")]
        public async Task<IActionResult> BalanceTransferApprovings()
        {
            var balanceTransferApprovings = new List<BalanceTransferApprovingModel>();
            var contextApprovings = _context.BalanceTransferApprovings.ToList();
            foreach (var approving in contextApprovings)
            {
                var balanceFrom = await _context.Balances.FirstOrDefaultAsync(b => b.Id == approving.BalanceIdFrom);
                var balanceTo = await _context.Balances.FirstOrDefaultAsync(b => b.Id == approving.BalanceIdTo);
                var specialistFrom = await _context.Specialists.FirstOrDefaultAsync(s =>
                    s.Id == balanceFrom.ClientId);
                var specialistTo = await _context.Specialists.FirstOrDefaultAsync(s =>
                    s.Id == balanceTo.ClientId);
                var balanceTransferApprovingModel = new BalanceTransferApprovingModel
                {
                    ApprovingId = approving.Id,
                    Money = approving.Money,
                    SpecialistFrom = specialistFrom,
                    SpecialistTo = specialistTo
                };
                balanceTransferApprovings.Add(balanceTransferApprovingModel);
            }
            var model = new BalanceTransferApprovingManagerModel
            {
                BalanceTransferApprovings = balanceTransferApprovings
            };
            return View(model);
        }

        [Authorize(Roles = "manager")]
        public async Task<IActionResult> ApproveBalanceTransfer(string? approvingId)
        {
            var approving = await _context.BalanceTransferApprovings.FirstOrDefaultAsync(a =>
                a.Id == approvingId);
            var balanceFrom = await _context.Balances.FirstOrDefaultAsync(s =>
                s.Id == approving.BalanceIdFrom);
            var balanceTo = await _context.Balances.FirstOrDefaultAsync(s =>
                s.Id == approving.BalanceIdTo);
            var specialistFrom = await _context.Specialists
                .Include(s => s.Balances)
                .FirstOrDefaultAsync(s => s.Id == balanceFrom.ClientId);
            var specialistTo = await _context.Specialists
                .Include(s => s.Balances)
                .FirstOrDefaultAsync(s => s.Id == balanceTo.ClientId);
            var bankFrom = await _context.Banks.FirstOrDefaultAsync(b => b.Id == specialistFrom.BankId);
            var bankTo = await _context.Banks.FirstOrDefaultAsync(b => b.Id == specialistTo.BankId);
            /*specialistFrom.Balances.Remove(balanceFrom);
            balanceFrom.Money -= approving.Money;
            specialistFrom.Balances.Add(balanceFrom);*/

            specialistTo.Balances.Remove(balanceTo);
            balanceTo.Money += approving.Money;
            specialistTo.Balances.Add(balanceTo);

            var balanceTransferAction = new BalanceTransferAction
            {
                Money = approving.Money,
                BankIdFrom = specialistFrom.BankId,
                BankIdTo = specialistTo.BankId,
                BankNameFrom = bankFrom.Name,
                BankNameTo = bankTo.Name,
                UserIdFrom = specialistFrom.Id,
                UserIdTo = specialistTo.Id,
                UserEmailFrom = specialistFrom.Email,
                UserEmailTo = specialistTo.Email,
                BalanceIdFrom = balanceFrom.Id,
                BalanceIdTo = balanceTo.Id,
                BalanceNameFrom = balanceFrom.Name,
                BalanceNameTo = balanceTo.Name
            };

            _context.BalanceTransferActions.Add(balanceTransferAction);
            _context.Specialists.Update(specialistFrom);
            _context.Specialists.Update(specialistTo);
            _context.Balances.Update(balanceFrom);
            _context.Balances.Update(balanceTo);
            _context.BalanceTransferApprovings.Remove(approving);
            await _context.SaveChangesAsync();
            return RedirectToAction("BalanceTransferApprovings", "Manager");
        }

        [Authorize(Roles = "manager")]
        public async Task<IActionResult> RejectBalanceTransfer(string? approvingId)
        {
            var approving = await _context.BalanceTransferApprovings.FirstOrDefaultAsync(a =>
                a.Id == approvingId);
            var balanceFrom = await _context.Balances.FirstOrDefaultAsync(s =>
                s.Id == approving.BalanceIdFrom);
            var specialistFrom = await _context.Specialists
                .Include(s => s.Balances)
                .FirstOrDefaultAsync(s => s.Id == balanceFrom.ClientId);

            specialistFrom.Balances.Remove(balanceFrom);
            balanceFrom.Money += approving.Money;
            specialistFrom.Balances.Add(balanceFrom);

            _context.Balances.Update(balanceFrom);
            _context.Specialists.Update(specialistFrom);
            _context.BalanceTransferApprovings.Remove(approving);
            await _context.SaveChangesAsync();
            return RedirectToAction("BalanceTransferApprovings", "Manager");
        }

        [Authorize(Roles = "admin, manager")]
        public async Task<IActionResult> Specialists()
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == User.Identity.Name);
            var contextSpecialists = _context.Specialists.Where(c => c.BankId == user.BankId).ToList();
            var model = new SpecialistsManagerModel
            {
                Specialists = contextSpecialists
            };
            return View(model);
        }

        [Authorize(Roles = "admin, manager")]
        public async Task<IActionResult> SpecialistBalanceTransferActions(string? specialistId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id ==
                User.Identity.Name);
            var balanceTransferActions = _context.BalanceTransferActions
                .Where(a => (a.UserIdFrom == specialistId || a.UserIdTo == specialistId) &&
                    a.BankIdFrom == user.BankId).ToList();
            var model = new SpecialistBalanceTransferActionManagerModel
            {
                BalanceTransferActions = balanceTransferActions,
                CurrentSpecialistId = specialistId
            };
            return View(model);
        }

        [Authorize(Roles = "admin, manager")]
        public async Task<IActionResult> CancelSpecialistBalanceTransfer(string? actionId)
        {
            var action = await _context.BalanceTransferActions
                .FirstOrDefaultAsync(a => a.Id == actionId);
            if (action.BalanceIdFrom == null || action.BalanceIdTo == null || action.Canceled == true)
            {
                return RedirectToAction("SpecialistBalanceTransferActions", "Manager", new { specialistId = action.UserIdFrom });
            }
            var specialistFrom = await _context.Specialists
                .Include(c => c.Balances)
                .FirstOrDefaultAsync(c => c.Id == action.UserIdFrom);
            var specialistTo = await _context.Specialists
                .Include(c => c.Balances)
                .FirstOrDefaultAsync(c => c.Id == action.UserIdTo);
            var balanceFrom = await _context.Balances.FirstOrDefaultAsync(b => b.Id ==
                action.BalanceIdFrom);
            var balanceTo = await _context.Balances.FirstOrDefaultAsync(b => b.Id ==
                action.BalanceIdTo);
            if (balanceTo.Money < action.Money)
            {
                return RedirectToAction("SpecialistBalanceTransferActions", "Manager", new { specialistId = action.UserIdFrom });
            }

            specialistFrom.Balances.Remove(balanceFrom);
            balanceFrom.Money += action.Money;
            specialistFrom.Balances.Add(balanceFrom);

            specialistTo.Balances.Remove(balanceTo);
            balanceTo.Money -= action.Money;
            specialistTo.Balances.Add(balanceTo);

            action.Canceled = true;
            _context.BalanceTransferActions.Update(action);
            _context.Balances.Update(balanceFrom);
            _context.Balances.Update(balanceTo);
            _context.Specialists.Update(specialistFrom);
            _context.Specialists.Update(specialistTo);
            await _context.SaveChangesAsync();
            return RedirectToAction("SpecialistBalanceTransferActions", "Manager", new { specialistId = action.UserIdFrom });
        }
    }
}
