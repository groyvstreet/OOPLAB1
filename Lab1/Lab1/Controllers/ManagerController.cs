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
        public async Task<IActionResult> SignUpApprovings()
        {
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
            if (balance == null)
            {
                return RedirectToAction("CreditApprovings", "Manager");
            }
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
            var createCreditAction = new CreateCreditAction
            {
                UserId = client.Id,
                UserEmail = client.Email,
                CreditId = credit.Id,
                Money = credit.Money,
                MoneyWithPercent = credit.MoneyWithPercent,
                Months = credit.Months,
                PayedMonths = credit.PayedMonths,
                Fines = credit.Fines,
                CreatingTime = credit.CreatingTime,
                PaymentTime = credit.PaymentTime,
                BalanceId = balance.Id,
                BalanceName = balance.Name,
                Info = $"Клиент {client.Email} взял кредит на сумму {credit.Money} на {credit.Months} месяцев на счет {balance.Name}.",
                Type = "CreateCredit"
            };
            _context.CreateCreditActions.Add(createCreditAction);
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
            if (balance == null)
            {
                return RedirectToAction("InstallmentApprovings", "Manager");
            }
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
            var createInstallmentAction = new CreateInstallmentAction
            {
                UserId = client.Id,
                UserEmail = client.Email,
                InstallmentId = installment.Id,
                Money = installment.Money,
                PayMoney = installment.PayMoney,
                Months = installment.Months,
                PayedMonths = installment.PayedMonths,
                CreatingTime = installment.CreatingTime,
                PaymentTime = installment.PaymentTime,
                BalanceId = balance.Id,
                BalanceName = balance.Name,
                Info = $"Клиент {client.Email} взял рассрочку на сумму {installment.Money} на {installment.Months} месяцев на счет {balance.Name}.",
                Type = "CreateInstallment"
            };
            _context.CreateInstallmentActions.Add(createInstallmentAction);
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
            var manager = await _context.Users.FirstOrDefaultAsync(m => m.Id == User.Identity.Name);
            var blockDepositAction = new BlockDepositAction
            {
                UserId = manager.Id,
                UserEmail = manager.Email,
                DepositId = deposit.Id,
                ClientEmail = client.Email,
                Money = deposit.Money,
                Percent = deposit.Percent,
                Info = $"Менеджер {manager.Email} заблокировал вклад клиента {client.Email}.",
                Type = "BlockDeposit"
            };
            _context.BlockDepositActions.Add(blockDepositAction);
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
            var manager = await _context.Users.FirstOrDefaultAsync(m => m.Id == User.Identity.Name);
            var unblockDepositAction = new UnblockDepositAction
            {
                UserId = manager.Id,
                UserEmail = manager.Email,
                DepositId = deposit.Id,
                ClientEmail = client.Email,
                Money = deposit.Money,
                Percent = deposit.Percent,
                Info = $"Менеджер {manager.Email} разблокировал вклад клиента {client.Email}.",
                Type = "UnblockDeposit"
            };
            _context.UnblockDepositActions.Add(unblockDepositAction);
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
            var manager = await _context.Users.FirstOrDefaultAsync(m => m.Id == User.Identity.Name);
            var freezeDepositAction = new FreezeDepositAction
            {
                UserId = manager.Id,
                UserEmail = manager.Email,
                DepositId = deposit.Id,
                ClientEmail = client.Email,
                Money = deposit.Money,
                Percent = deposit.Percent,
                Info = $"Менеджер {manager.Email} заморозил вклад клиента {client.Email}.",
                Type = "FreezeDeposit"
            };
            _context.FreezeDepositActions.Add(freezeDepositAction);
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
            var manager = await _context.Users.FirstOrDefaultAsync(m => m.Id == User.Identity.Name);
            var UnfreezeDepositAction = new UnfreezeDepositAction
            {
                UserId = manager.Id,
                UserEmail = manager.Email,
                DepositId = deposit.Id,
                ClientEmail = client.Email,
                Money = deposit.Money,
                Percent = deposit.Percent,
                Info = $"Менеджер {manager.Email} разморозил вклад клиента {client.Email}.",
                Type = "UnfreezeDeposit"
            };
            _context.UnfreezeDepositActions.Add(UnfreezeDepositAction);
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
                if(balanceFrom == null || balanceTo == null)
                {
                    continue;
                }
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
                UserId = specialistFrom.Id,
                UserIdTo = specialistTo.Id,
                UserEmail = specialistFrom.Email,
                UserEmailTo = specialistTo.Email,
                BalanceIdFrom = balanceFrom.Id,
                BalanceIdTo = balanceTo.Id,
                BalanceNameFrom = balanceFrom.Name,
                BalanceNameTo = balanceTo.Name,
                Info = $"Специалист {specialistFrom.Email} перевел со счета {balanceFrom.Name} сумму в размере {approving.Money} специалисту {specialistTo.Email} на счет {balanceTo.Name}",
                Type = "SpecialistTransferBalance"
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
                .Where(a => (a.UserId == specialistId || a.UserIdTo == specialistId) &&
                    a.BankIdFrom == user.BankId).ToList();
            var model = new SpecialistBalanceTransferActionManagerModel
            {
                BalanceTransferActions = balanceTransferActions,
                CurrentSpecialistId = specialistId
            };
            return View(model);
        }

        [Authorize(Roles = "admin, manager")]
        public async Task<IActionResult> CancelSpecialistBalanceTransferAction(string? actionId)
        {
            var action = await _context.BalanceTransferActions
                .FirstOrDefaultAsync(a => a.Id == actionId);
            if (action.Canceled == true)
            {
                return RedirectToAction("SpecialistBalanceTransferActions", "Manager", new { specialistId = action.UserId });
            }
            var specialistFrom = await _context.Specialists
                .Include(c => c.Balances)
                .FirstOrDefaultAsync(c => c.Id == action.UserId);
            var specialistTo = await _context.Specialists
                .Include(c => c.Balances)
                .FirstOrDefaultAsync(c => c.Id == action.UserIdTo);
            var balanceFrom = await _context.Balances.FirstOrDefaultAsync(b => b.Id ==
                action.BalanceIdFrom);
            var balanceTo = await _context.Balances.FirstOrDefaultAsync(b => b.Id ==
                action.BalanceIdTo);
            if (balanceFrom == null || balanceTo == null)
            {
                return RedirectToAction("SpecialistBalanceTransferActions", "Manager", new { specialistId = action.UserId });
            }
            if (balanceTo.Money < action.Money)
            {
                return RedirectToAction("SpecialistBalanceTransferActions", "Manager", new { specialistId = action.UserId });
            }

            specialistFrom.Balances.Remove(balanceFrom);
            balanceFrom.Money += action.Money;
            specialistFrom.Balances.Add(balanceFrom);

            specialistTo.Balances.Remove(balanceTo);
            balanceTo.Money -= action.Money;
            specialistTo.Balances.Add(balanceTo);

            action.Canceled = true;
            action.CancelTime = DateTime.Now;
            _context.BalanceTransferActions.Update(action);
            _context.Balances.Update(balanceFrom);
            _context.Balances.Update(balanceTo);
            _context.Specialists.Update(specialistFrom);
            _context.Specialists.Update(specialistTo);
            await _context.SaveChangesAsync();
            return RedirectToAction("SpecialistBalanceTransferActions", "Manager", new { specialistId = action.UserId });
        }

        [Authorize(Roles = "admin, manager")]
        public async Task<IActionResult> SalaryApprovingBySpecialistActions(string? specialistId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id ==
                User.Identity.Name);
            var actions = _context.SalaryApprovingBySpecialistActions
                .Where(a => a.UserId == specialistId).ToList();
            var model = new SalaryApprovingBySpecialistActionsManagerModel
            {
                Actions = actions
            };
            return View(model);
        }

        [Authorize(Roles = "admin, manager")]
        public async Task<IActionResult> CancelSalaryApprovingBySpecialistAction(string? actionId)
        {
            var action = await _context.SalaryApprovingBySpecialistActions
                .FirstOrDefaultAsync(a => a.Id == actionId);
            var client = await _context.Clients
                .Include(c => c.Salary)
                .FirstOrDefaultAsync(c => c.Id == action.ClientId);
            if (client.Salary == null || client.Salary.ApprovedByOperator || action.Canceled)
            {
                return RedirectToAction("SalaryApprovingBySpecialistActions", "Manager", new { specialistId = action.UserId });
            }
            client.Salary.ApprovedBySpecialist = false;
            _context.Clients.Update(client);
            action.Canceled = true;
            action.CancelTime = DateTime.Now;
            _context.SalaryApprovingBySpecialistActions.Update(action);
            await _context.SaveChangesAsync();
            return RedirectToAction("SalaryApprovingBySpecialistActions", "Manager", new { specialistId = action.UserId });
        }

        [Authorize(Roles = "admin, manager")]
        public async Task<IActionResult> SalaryRejectingBySpecialistActions(string? specialistId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id ==
                User.Identity.Name);
            var actions = _context.SalaryRejectingBySpecialistActions
                .Where(a => a.UserId == specialistId).ToList();
            var model = new SalaryRejectingBySpecialistActionsManagerModel
            {
                Actions = actions
            };
            return View(model);
        }

        [Authorize(Roles = "admin, manager")]
        public async Task<IActionResult> CancelSalaryRejectingBySpecialistAction(string? actionId)
        {
            var action = await _context.SalaryRejectingBySpecialistActions
                .FirstOrDefaultAsync(a => a.Id == actionId);
            var client = await _context.Clients
                .Include(c => c.Salary)
                .FirstOrDefaultAsync(c => c.Id == action.ClientId);
            if (action.Canceled || client.Salary != null)
            {
                return RedirectToAction("SalaryApprovingBySpecialistActions", "Manager", new { specialistId = action.UserId });
            }
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id ==
                client.CompanyId);
            client.Salary = new Salary
            {
                Money = company.SalaryMoney,
                ClientId = client.Id
            };
            _context.Clients.Update(client);
            var salaryApproving = new SalaryApproving
            {
                ClientId = client.Id
            };
            _context.SalaryApprovings.Add(salaryApproving);
            action.Canceled = true;
            action.CancelTime = DateTime.Now;
            _context.SalaryRejectingBySpecialistActions.Update(action);
            await _context.SaveChangesAsync();
            return RedirectToAction("SalaryRejectingBySpecialistActions", "Manager", new { specialistId = action.UserId });
        }
    }
}
