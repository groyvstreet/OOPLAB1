using Lab1.Models.AdminModels;
using Lab1.Models.Data;
using Lab1.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab1.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Profile()
        {
            var admin = await _context.Users.FirstOrDefaultAsync(m => m.Id == User.Identity.Name);
            var bank = await _context.Banks.FirstOrDefaultAsync(b => b.Id == admin.BankId);
            var model = new AdminProfileModel
            {
                Email = admin.Email,
                FirstName = admin.FirstName,
                LastName = admin.LastName,
                Patronymic = admin.Patronymic,
                PhoneNumber = admin.PhoneNumber,
                BankName = bank.Name
            };
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Clients()
        {
            var admin = await _context.Users.FirstOrDefaultAsync(u => u.Id == User.Identity.Name);
            var contextClients = _context.Clients.Where(c => c.BankId == admin.BankId).ToList();
            var model = new ClientsAdminModel
            {
                Clients = contextClients
            };
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Operators()
        {
            var admin = await _context.Users.FirstOrDefaultAsync(u => u.Id == User.Identity.Name);
            var contextOperators = _context.Users.Where(c => c.BankId == admin.BankId &&
                c.RoleName == "operator").ToList();
            var model = new OperatorsAdminModel
            {
                Operators = contextOperators
            };
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Managers()
        {
            var admin = await _context.Users.FirstOrDefaultAsync(u => u.Id == User.Identity.Name);
            var contextManagers = _context.Users.Where(c => c.BankId == admin.BankId &&
                c.RoleName == "manager").ToList();
            var model = new ManagersAdminModel
            {
                Managers = contextManagers
            };
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Specialists()
        {
            var admin = await _context.Users.FirstOrDefaultAsync(u => u.Id == User.Identity.Name);
            var contextSpecialists = _context.Specialists.Where(c => c.BankId == admin.BankId).ToList();
            var model = new SpecialistsAdminModel
            {
                Specialists = contextSpecialists
            };
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> BlockDepositActions(string managerId)
        {
            var actions = _context.BlockDepositActions.Where(a => a.UserId == managerId).ToList();
            var model = new BlockDepositActionsAdminModel
            {
                Actions = actions
            };
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CancelBlockDepositAction(string actionId)
        {
            var action = await _context.BlockDepositActions.FirstOrDefaultAsync(a =>
                a.Id == actionId);
            var deposit = await _context.Deposits.FirstOrDefaultAsync(d => d.Id ==
                action.DepositId);
            var client = await _context.Clients
                .Include(c => c.Deposits)
                .FirstOrDefaultAsync(c => c.Id == deposit.ClientId);
            client.Deposits.Remove(deposit);
            deposit.Blocked = false;
            client.Deposits.Add(deposit);
            action.Canceled = true;
            _context.Deposits.Update(deposit);
            _context.Clients.Update(client);
            _context.BlockDepositActions.Update(action);
            await _context.SaveChangesAsync();
            return RedirectToAction("BlockDepositActions", "Admin", new { managerId = action.UserId });
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> FreezeDepositActions(string managerId)
        {
            var actions = _context.FreezeDepositActions.Where(a => a.UserId == managerId).ToList();
            var model = new FreezeDepositActionsAdminModel
            {
                Actions = actions
            };
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CancelFreezeDepositAction(string actionId)
        {
            var action = await _context.FreezeDepositActions.FirstOrDefaultAsync(a =>
                a.Id == actionId);
            var deposit = await _context.Deposits.FirstOrDefaultAsync(d => d.Id ==
                action.DepositId);
            var client = await _context.Clients
                .Include(c => c.Deposits)
                .FirstOrDefaultAsync(c => c.Id == deposit.ClientId);
            client.Deposits.Remove(deposit);
            deposit.Freezed = false;
            client.Deposits.Add(deposit);
            action.Canceled = true;
            _context.Deposits.Update(deposit);
            _context.Clients.Update(client);
            _context.FreezeDepositActions.Update(action);
            await _context.SaveChangesAsync();
            return RedirectToAction("FreezeDepositActions", "Admin", new { managerId = action.UserId });
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UnblockDepositActions(string managerId)
        {
            var actions = _context.UnblockDepositActions.Where(a => a.UserId == managerId).ToList();
            var model = new UnblockDepositActionsAdminModel
            {
                Actions = actions
            };
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CancelUnblockDepositAction(string actionId)
        {
            var action = await _context.UnblockDepositActions.FirstOrDefaultAsync(a =>
                a.Id == actionId);
            var deposit = await _context.Deposits.FirstOrDefaultAsync(d => d.Id ==
                action.DepositId);
            var client = await _context.Clients
                .Include(c => c.Deposits)
                .FirstOrDefaultAsync(c => c.Id == deposit.ClientId);
            client.Deposits.Remove(deposit);
            deposit.Blocked = true;
            client.Deposits.Add(deposit);
            action.Canceled = true;
            _context.Deposits.Update(deposit);
            _context.Clients.Update(client);
            _context.UnblockDepositActions.Update(action);
            await _context.SaveChangesAsync();
            return RedirectToAction("UnblockDepositActions", "Admin", new { managerId = action.UserId });
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UnFreezeDepositActions(string managerId)
        {
            var actions = _context.UnfreezeDepositActions.Where(a => a.UserId == managerId).ToList();
            var model = new UnfreezeDepositActionsAdminModel
            {
                Actions = actions
            };
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CancelUnfreezeDepositAction(string actionId)
        {
            var action = await _context.UnfreezeDepositActions.FirstOrDefaultAsync(a =>
                a.Id == actionId);
            var deposit = await _context.Deposits.FirstOrDefaultAsync(d => d.Id ==
                action.DepositId);
            var client = await _context.Clients
                .Include(c => c.Deposits)
                .FirstOrDefaultAsync(c => c.Id == deposit.ClientId);
            client.Deposits.Remove(deposit);
            deposit.Freezed = true;
            client.Deposits.Add(deposit);
            action.Canceled = true;
            _context.Deposits.Update(deposit);
            _context.Clients.Update(client);
            _context.UnfreezeDepositActions.Update(action);
            await _context.SaveChangesAsync();
            return RedirectToAction("UnfreezeDepositActions", "Admin", new { managerId = action.UserId });
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateDepositActions(string clientId)
        {
            var actions = _context.CreateDepositActions.Where(a => a.UserId == clientId).ToList();
            var model = new CreateDepositActionsAdminModel
            {
                Actions = actions
            };
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CancelCreateDepositAction(string actionId)
        {
            var action = await _context.CreateDepositActions.FirstOrDefaultAsync(a =>
                a.Id == actionId);
            var deposit = await _context.Deposits.FirstOrDefaultAsync(d => d.Id ==
                action.DepositId);
            var client = await _context.Clients
                .Include(c => c.Deposits)
                .Include(c => c.Balances)
                .FirstOrDefaultAsync(c => c.Id == deposit.ClientId);
            client.Deposits.Remove(deposit);
            action.Canceled = true;
            _context.Deposits.Remove(deposit);
            if(action.BalanceId != null)
            {
                var balance = await _context.Balances.FirstOrDefaultAsync(b => b.Id ==
                    action.BalanceId);
                client.Balances.Remove(balance);
                balance.Money += action.Money;
                client.Balances.Add(balance);
                _context.Balances.Update(balance);
            }
            _context.Clients.Update(client);
            _context.CreateDepositActions.Update(action);
            await _context.SaveChangesAsync();
            return RedirectToAction("CreateDepositActions", "Admin", new { clientId = action.UserId });
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetDepositActions(string clientId)
        {
            var actions = _context.GetDepositActions.Where(a => a.UserId == clientId).ToList();
            var model = new GetDepositActionsAdminModel
            {
                Actions = actions
            };
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CancelGetDepositAction(string actionId)
        {
            var action = await _context.GetDepositActions.FirstOrDefaultAsync(a =>
                a.Id == actionId);
            var client = await _context.Clients
                .Include(c => c.Deposits)
                .Include(c => c.Balances)
                .FirstOrDefaultAsync(c => c.Id == action.UserId);
            var deposit = new Deposit
            {
                Money = action.Money,
                Percent = action.Percent,
                OpenedTime = action.OpenedTime,
                ClosedTime = action.ClosedTime,
                ClientId = action.UserId
            };
            client.Deposits.Add(deposit);
            action.Canceled = true;
            var balance = client.Balances.FirstOrDefault(b => b.Id == action.BalanceId);
            if(balance == null || balance.Money < action.MoneyWithPercent)
            {
                return RedirectToAction("GetDepositActions", "Admin", new { clientId = action.UserId });
            }
            client.Balances.Remove(balance);
            balance.Money -= action.MoneyWithPercent;
            client.Balances.Add(balance);
            _context.Balances.Update(balance);
            _context.Deposits.Add(deposit);
            _context.Clients.Update(client);
            _context.GetDepositActions.Update(action);
            await _context.SaveChangesAsync();
            return RedirectToAction("GetDepositActions", "Admin", new { clientId = action.UserId });
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddDepositActions(string clientId)
        {
            var actions = _context.AddDepositActions.Where(a => a.UserId == clientId).ToList();
            var model = new AddDepositActionsAdminModel
            {
                Actions = actions
            };
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CancelAddDepositAction(string actionId)
        {
            var action = await _context.AddDepositActions.FirstOrDefaultAsync(a =>
                a.Id == actionId);
            var client = await _context.Clients
                .Include(c => c.Deposits)
                .Include(c => c.Balances)
                .FirstOrDefaultAsync(c => c.Id == action.UserId);
            var deposit = client.Deposits.FirstOrDefault(d => d.Id == action.DepositId);
            if (deposit == null)
            {
                return RedirectToAction("AddDepositActions", "Admin", new { clientId = action.UserId });
            }
            action.Canceled = true;
            client.Deposits.Remove(deposit);
            deposit.Money -= action.AddedMoney;
            client.Deposits.Add(deposit);
            _context.Deposits.Update(deposit);
            _context.Clients.Update(client);
            _context.AddDepositActions.Update(action);
            await _context.SaveChangesAsync();
            return RedirectToAction("AddDepositActions", "Admin", new { clientId = action.UserId });
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> TransferDepositActions(string clientId)
        {
            var actions = _context.TransferDepositActions.Where(a => a.UserId == clientId).ToList();
            var model = new TransferDepositActionsAdminModel
            {
                Actions = actions
            };
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CancelTransferDepositAction(string actionId)
        {
            var action = await _context.TransferDepositActions.FirstOrDefaultAsync(a =>
                a.Id == actionId);
            var client = await _context.Clients
                .Include(c => c.Deposits)
                .FirstOrDefaultAsync(c => c.Id == action.UserId);
            var depositTo = client.Deposits.FirstOrDefault(d => d.Id == action.DepositIdTo);
            if (depositTo == null)
            {
                return RedirectToAction("TransferDepositActions", "Admin", new { clientId = action.UserId });
            }
            var depositFrom = new Deposit
            {
                Money = action.Money,
                Percent = action.Percent,
                OpenedTime = action.OpenedTime,
                ClosedTime = action.ClosedTime,
                ClientId = action.UserId
            };
            action.Canceled = true;
            client.Deposits.Remove(depositTo);
            depositTo.Money -= action.TransferMoney;
            client.Deposits.Add(depositTo);
            client.Deposits.Add(depositFrom);
            _context.Deposits.Update(depositTo);
            _context.Clients.Update(client);
            _context.TransferDepositActions.Update(action);
            await _context.SaveChangesAsync();
            return RedirectToAction("TransferDepositActions", "Admin", new { clientId = action.UserId });
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateSalaryActions(string clientId)
        {
            var actions = _context.CreateSalaryActions.Where(a => a.UserId == clientId).ToList();
            var model = new CreateSalaryActionsAdminModel
            {
                Actions = actions
            };
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CancelCreateSalaryAction(string actionId)
        {
            var action = await _context.CreateSalaryActions.FirstOrDefaultAsync(a =>
                a.Id == actionId);
            var client = await _context.Clients
                .Include(c => c.Salary)
                .FirstOrDefaultAsync(c => c.Id == action.UserId);
            if(client.Salary.ApprovedBySpecialist || client.Salary.ApprovedByOperator)
            {
                return RedirectToAction("CreateSalaryActions", "Admin", new { clientId = action.UserId });
            }
            client.Salary = null;
            action.Canceled = true;
            var salaryApproving = await _context.SalaryApprovings.FirstOrDefaultAsync(a =>
                a.ClientId == client.Id);
            _context.SalaryApprovings.Remove(salaryApproving);
            _context.Clients.Update(client);
            _context.CreateSalaryActions.Update(action);
            await _context.SaveChangesAsync();
            return RedirectToAction("CreateSalaryActions", "Admin", new { clientId = action.UserId });
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetSalaryActions(string clientId)
        {
            var actions = _context.GetSalaryActions.Where(a => a.UserId == clientId).ToList();
            var model = new GetSalaryActionsAdminModel
            {
                Actions = actions
            };
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CancelGetSalaryAction(string actionId)
        {
            var action = await _context.GetSalaryActions.FirstOrDefaultAsync(a =>
                a.Id == actionId);
            var client = await _context.Clients
                .Include(c => c.Balances)
                .FirstOrDefaultAsync(c => c.Id == action.UserId);
            action.Canceled = true;
            var balance = client.Balances.FirstOrDefault(b => b.Id == action.BalanceId);
            if (balance == null || balance.Money < action.Money)
            {
                return RedirectToAction("GetSalaryActions", "Admin", new { clientId = action.UserId });
            }
            client.Balances.Remove(balance);
            balance.Money -= action.Money;
            client.Balances.Add(balance);
            _context.Balances.Update(balance);
            _context.Clients.Update(client);
            _context.GetSalaryActions.Update(action);
            await _context.SaveChangesAsync();
            return RedirectToAction("GetSalaryActions", "Admin", new { clientId = action.UserId });
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SalaryApprovingByOperatorActions(string? operatorId)
        {
            var actions = _context.SalaryApprovingByOperatorActions
                .Where(a => a.UserId == operatorId).ToList();
            var model = new SalaryApprovingByOperatorActionsAdminModel
            {
                Actions = actions
            };
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CancelSalaryApprovingByOperatorAction(string? actionId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == User.Identity.Name);
            var action = await _context.SalaryApprovingByOperatorActions
                .FirstOrDefaultAsync(a => a.Id == actionId);
            var client = await _context.Clients
                .Include(c => c.Salary)
                .FirstOrDefaultAsync(c => c.Id == action.ClientId);
            if (action.Canceled)
            {
                if (user.RoleName == "operator")
                {
                    return RedirectToAction("SalaryRejectingByOperatorActions", "Admin", new { operatorId = action.UserId });
                }
                else
                {
                    return RedirectToAction("SalaryRejectingByManagerActions", "Admin", new { managerId = action.UserId });
                }
            }
            client.Salary.ApprovedByOperator = false;
            _context.Clients.Update(client);
            var salaryApproving = new SalaryApproving
            {
                ClientId = action.ClientId
            };
            _context.SalaryApprovings.Add(salaryApproving);
            action.Canceled = true;
            _context.SalaryApprovingByOperatorActions.Update(action);
            await _context.SaveChangesAsync();
            if (user.RoleName == "operator")
            {
                return RedirectToAction("SalaryRejectingByOperatorActions", "Admin", new { operatorId = action.UserId });
            }
            else
            {
                return RedirectToAction("SalaryRejectingByManagerActions", "Admin", new { managerId = action.UserId });
            }
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SalaryRejectingByOperatorActions(string? operatorId)
        {
            var actions = _context.SalaryRejectingByOperatorActions
                .Where(a => a.UserId == operatorId).ToList();
            var model = new SalaryRejectingByOperatorActionsAdminModel
            {
                Actions = actions
            };
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CancelSalaryRejectingByOperatorAction(string? actionId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == User.Identity.Name);
            var action = await _context.SalaryRejectingByOperatorActions
                .FirstOrDefaultAsync(a => a.Id == actionId);
            var client = await _context.Clients
                .Include(c => c.Salary)
                .FirstOrDefaultAsync(c => c.Id == action.ClientId);
            if (action.Canceled)
            {
                if (user.RoleName == "operator")
                {
                    return RedirectToAction("SalaryRejectingByOperatorActions", "Admin", new { operatorId = action.UserId });
                }
                else
                {
                    return RedirectToAction("SalaryRejectingByManagerActions", "Admin", new { managerId = action.UserId });
                }
            }
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id ==
                client.CompanyId);
            client.Salary = new Salary
            {
                Money = company.SalaryMoney,
                ClientId = client.Id
            };
            client.Salary.ApprovedBySpecialist = true;
            _context.Clients.Update(client);
            var salaryApproving = new SalaryApproving
            {
                ClientId = action.ClientId
            };
            _context.SalaryApprovings.Add(salaryApproving);
            action.Canceled = true;
            _context.SalaryRejectingByOperatorActions.Update(action);
            await _context.SaveChangesAsync();
            if (user.RoleName == "operator")
            {
                return RedirectToAction("SalaryRejectingByOperatorActions", "Admin", new { operatorId = action.UserId });
            }
            else
            {
                return RedirectToAction("SalaryRejectingByManagerActions", "Admin", new { managerId = action.UserId });
            }
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SalaryApprovingByManagerActions(string? managerId)
        {
            var actions = _context.SalaryApprovingByOperatorActions
                .Where(a => a.UserId == managerId).ToList();
            var model = new SalaryApprovingByManagerActionsAdminModel
            {
                Actions = actions
            };
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SalaryRejectingByManagerActions(string? managerId)
        {
            var actions = _context.SalaryRejectingByOperatorActions
                .Where(a => a.UserId == managerId).ToList();
            var model = new SalaryRejectingByManagerActionsAdminModel
            {
                Actions = actions
            };
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> OpenBalanceActions(string? clientId)
        {
            var actions = _context.OpenBalanceActions
                .Where(a => a.UserId == clientId).ToList();
            var model = new OpenBalanceActionsAdminModel
            {
                Actions = actions
            };
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CancelOpenBalanceAction(string? actionId)
        {
            var action = await _context.OpenBalanceActions
                .FirstOrDefaultAsync(a => a.Id == actionId);
            if (action.Canceled)
            {
                return RedirectToAction("OpenBalanceActions", "Admin", new { clientId = action.UserId });
            }
            var client = await _context.Clients
                .Include(c => c.Balances)
                .FirstOrDefaultAsync(c => c.Id == action.UserId);
            var balance = client.Balances.FirstOrDefault(b => b.Id == action.BalanceId);
            if (balance == null)
            {
                return RedirectToAction("OpenBalanceActions", "Admin", new { clientId = action.UserId });
            }
            client.Balances.Remove(balance);
            _context.Balances.Remove(balance);
            _context.Clients.Update(client);
            action.Canceled = true;
            _context.OpenBalanceActions.Update(action);
            await _context.SaveChangesAsync();
            return RedirectToAction("OpenBalanceActions", "Admin", new { clientId = action.UserId });
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CloseBalanceActions(string? clientId)
        {
            var actions = _context.CloseBalanceActions
                .Where(a => a.UserId == clientId).ToList();
            var model = new CloseBalanceActionsAdminModel
            {
                Actions = actions
            };
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CancelCloseBalanceAction(string? actionId)
        {
            var action = await _context.CloseBalanceActions
                .FirstOrDefaultAsync(a => a.Id == actionId);
            if (action.Canceled)
            {
                return RedirectToAction("CloseBalanceActions", "Admin", new { clientId = action.UserId });
            }
            var client = await _context.Clients
                .Include(c => c.Balances)
                .FirstOrDefaultAsync(c => c.Id == action.UserId);
            var balance = new Balance
            {
                Id = action.BalanceId,
                Name = action.BalanceName,
                Money = action.Money,
                ClientId = client.Id
            };
            client.Balances.Add(balance);
            _context.Balances.Add(balance);
            _context.Clients.Update(client);
            action.Canceled = true;
            _context.CloseBalanceActions.Update(action);
            await _context.SaveChangesAsync();
            return RedirectToAction("CloseBalanceActions", "Admin", new { clientId = action.UserId });
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddBalanceActions(string? clientId)
        {
            var actions = _context.AddBalanceActions
                .Where(a => a.UserId == clientId).ToList();
            var model = new AddBalanceActionsAdminModel
            {
                Actions = actions
            };
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CancelAddBalanceAction(string? actionId)
        {
            var action = await _context.AddBalanceActions
                .FirstOrDefaultAsync(a => a.Id == actionId);
            if (action.Canceled)
            {
                return RedirectToAction("AddBalanceActions", "Admin", new { clientId = action.UserId });
            }
            var client = await _context.Clients
                .Include(c => c.Balances)
                .FirstOrDefaultAsync(c => c.Id == action.UserId);
            var balance = client.Balances.FirstOrDefault(b => b.Id == action.BalanceId);
            if(balance == null)
            {
                return RedirectToAction("AddBalanceActions", "Admin", new { clientId = action.UserId });
            }
            client.Balances.Remove(balance);
            balance.Money -= action.Money;
            client.Balances.Add(balance);
            _context.Balances.Update(balance);
            _context.Clients.Update(client);
            action.Canceled = true;
            _context.AddBalanceActions.Update(action);
            await _context.SaveChangesAsync();
            return RedirectToAction("AddBalanceActions", "Admin", new { clientId = action.UserId });
        }

        [Authorize(Roles = "admin, operator, manager")]
        public async Task<IActionResult> TransferBalanceActions(string? clientId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id ==
                User.Identity.Name);
            var balanceTransferActions = _context.BalanceTransferActions
                .Where(a => (a.UserId == clientId || a.UserIdTo == clientId) &&
                    a.BankIdFrom == user.BankId).ToList();
            var model = new TransferBalanceActionsAdminModel
            {
                BalanceTransferActions = balanceTransferActions,
                CurrentClientId = clientId
            };
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateInstallmentActions(string? clientId)
        {
            var actions = _context.CreateInstallmentActions
                .Where(a => a.UserId == clientId).ToList();
            var model = new CreateInstallmentActionsAdminModel
            {
                Actions = actions
            };
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CancelCreateInstallmentAction(string? actionId)
        {
            var action = await _context.CreateInstallmentActions.FirstOrDefaultAsync(a =>
                a.Id == actionId);
            var client = await _context.Clients
                .Include(c => c.Installments)
                .Include(c => c.Balances)
                .FirstOrDefaultAsync(c => c.Id == action.UserId);
            var installment = client.Installments.FirstOrDefault(i => i.Id == action.InstallmentId);
            if(installment == null)
            {
                return RedirectToAction("CreateInstallmentActions", "Admin", new { clientId = action.UserId });
            }
            client.Installments.Remove(installment);
            _context.Installments.Remove(installment);
            var balance = client.Balances.FirstOrDefault(b => b.Id == action.BalanceId);
            if(balance == null)
            {
                return RedirectToAction("CreateInstallmentActions", "Admin", new { clientId = action.UserId });
            }
            client.Balances.Remove(balance);
            balance.Money -= action.Money;
            client.Balances.Add(balance);
            _context.Balances.Update(balance);
            _context.Clients.Update(client);
            action.Canceled = true;
            _context.CreateInstallmentActions.Update(action);
            await _context.SaveChangesAsync();
            return RedirectToAction("CreateInstallmentActions", "Admin", new { clientId = action.UserId });
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PayInstallmentActions(string? clientId)
        {
            var actions = _context.PayInstallmentActions
                .Where(a => a.UserId == clientId).ToList();
            var model = new PayInstallmentActionsAdminModel
            {
                Actions = actions
            };
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CancelPayInstallmentAction(string? actionId)
        {
            var action = await _context.PayInstallmentActions.FirstOrDefaultAsync(a =>
                a.Id == actionId);
            var client = await _context.Clients
                .Include(c => c.Installments)
                .Include(c => c.Balances)
                .FirstOrDefaultAsync(c => c.Id == action.UserId);
            var balance = client.Balances.FirstOrDefault(b => b.Id == action.BalanceId);
            if(balance == null)
            {
                return RedirectToAction("PayInstallmentActions", "Admin", new { clientId = action.UserId });
            }
            client.Balances.Remove(balance);
            balance.Money += action.SinglePaymentMoney;
            client.Balances.Add(balance);
            _context.Balances.Update(balance);
            if(action.PayMoney <= action.SinglePaymentMoney)
            {
                var installment = new Installment
                {
                    Id = action.InstallmentId,
                    Money = action.Money,
                    PayMoney = action.SinglePaymentMoney,
                    Months = action.Months,
                    PayedMonths = action.PayedMonths,
                    CreatingTime = action.CreatingTime,
                    PaymentTime = action.PaymentTime,
                    ClientId = client.Id,
                    Approved = true
                };
                client.Installments.Add(installment);
                _context.Installments.Add(installment);
            }
            else
            {
                var installment = client.Installments.FirstOrDefault(i => i.Id == action.InstallmentId);
                if(installment == null)
                {
                    return RedirectToAction("PayInstallmentActions", "Admin", new { clientId = action.UserId });
                }
                client.Installments.Remove(installment);
                installment.PayedMonths -= 1;
                installment.PayMoney += action.SinglePaymentMoney;
                client.Installments.Add(installment);
                _context.Installments.Update(installment);
            }
            _context.Clients.Update(client);
            action.Canceled = true;
            _context.PayInstallmentActions.Update(action);
            await _context.SaveChangesAsync();
            return RedirectToAction("PayInstallmentActions", "Admin", new { clientId = action.UserId });
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateCreditActions(string? clientId)
        {
            var actions = _context.CreateCreditActions
                .Where(a => a.UserId == clientId).ToList();
            var model = new CreateCreditActionsAdminModel
            {
                Actions = actions
            };
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CancelCreateCreditAction(string? actionId)
        {
            var action = await _context.CreateCreditActions.FirstOrDefaultAsync(a =>
                a.Id == actionId);
            var client = await _context.Clients
                .Include(c => c.Credits)
                .Include(c => c.Balances)
                .FirstOrDefaultAsync(c => c.Id == action.UserId);
            var credit = client.Credits.FirstOrDefault(i => i.Id == action.CreditId);
            if (credit == null)
            {
                return RedirectToAction("CreateCreditActions", "Admin", new { clientId = action.UserId });
            }
            client.Credits.Remove(credit);
            _context.Credits.Remove(credit);
            var balance = client.Balances.FirstOrDefault(b => b.Id == action.BalanceId);
            if (balance == null)
            {
                return RedirectToAction("CreateCreditActions", "Admin", new { clientId = action.UserId });
            }
            client.Balances.Remove(balance);
            balance.Money -= action.Money;
            client.Balances.Add(balance);
            _context.Balances.Update(balance);
            _context.Clients.Update(client);
            action.Canceled = true;
            _context.CreateCreditActions.Update(action);
            await _context.SaveChangesAsync();
            return RedirectToAction("CreateCreditActions", "Admin", new { clientId = action.UserId });
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PayCreditActions(string? clientId)
        {
            var actions = _context.PayCreditActions
                .Where(a => a.UserId == clientId).ToList();
            var model = new PayCreditActionsAdminModel
            {
                Actions = actions
            };
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CancelPayCreditAction(string? actionId)
        {
            var action = await _context.PayCreditActions.FirstOrDefaultAsync(a =>
                a.Id == actionId);
            var client = await _context.Clients
                .Include(c => c.Credits)
                .Include(c => c.Balances)
                .FirstOrDefaultAsync(c => c.Id == action.UserId);
            var balance = client.Balances.FirstOrDefault(b => b.Id == action.BalanceId);
            if (balance == null)
            {
                return RedirectToAction("PayCreditActions", "Admin", new { clientId = action.UserId });
            }
            client.Balances.Remove(balance);
            balance.Money += action.SinglePaymentMoney;
            client.Balances.Add(balance);
            _context.Balances.Update(balance);
            if (action.MoneyWithPercent <= action.SinglePaymentMoney)
            {
                var credit = new Credit
                {
                    Id = action.CreditId,
                    Money = action.Money,
                    MoneyWithPercent = action.SinglePaymentMoney,
                    Months = action.Months,
                    PayedMonths = action.PayedMonths,
                    CreatingTime = action.CreatingTime,
                    PaymentTime = action.PaymentTime,
                    ClientId = client.Id,
                    Approved = true
                };
                client.Credits.Add(credit);
                _context.Credits.Add(credit);
            }
            else
            {
                var credit = client.Credits.FirstOrDefault(i => i.Id == action.CreditId);
                if (credit == null)
                {
                    return RedirectToAction("PayCreditActions", "Admin", new { clientId = action.UserId });
                }
                client.Credits.Remove(credit);
                credit.PayedMonths -= 1;
                credit.MoneyWithPercent += action.SinglePaymentMoney;
                client.Credits.Add(credit);
                _context.Credits.Update(credit);
            }
            _context.Clients.Update(client);
            action.Canceled = true;
            _context.PayCreditActions.Update(action);
            await _context.SaveChangesAsync();
            return RedirectToAction("PayCreditActions", "Admin", new { clientId = action.UserId });
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Actions()
        {
            var contextActions = _context.Actions.ToList();
            var actions = new List<Models.Entities.Actions.Action>();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == User.Identity.Name);
            foreach(var action in contextActions)
            {
                var actionUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == action.UserId);
                if(actionUser.BankId == user.BankId)
                {
                    actions.Add(action);
                }
            }
            var model = new ActionsAdminModel
            {
                Actions = actions
            };
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CancelAction(string actionId)
        {
            var action = await _context.Actions.FirstOrDefaultAsync(a => a.Id == actionId);
            switch (action.Type)
            {
                case "AddBalance":
                    return RedirectToAction("CancelAddBalanceAction", "Admin", new { actionId = actionId });
                case "AddDeposit":
                    return RedirectToAction("CancelAddDepositAction", "Admin", new { actionId = actionId });
                case "TransferBalance":
                    return RedirectToAction("CancelClientBalanceTransfer", "Operator", new { actionId = actionId });
                case "BlockDeposit":
                    return RedirectToAction("CancelBlockDepositAction", "Admin", new { actionId = actionId });
                case "CloseBalance":
                    return RedirectToAction("CancelCloseBalanceAction", "Admin", new { actionId = actionId });
                case "CreateCredit":
                    return RedirectToAction("CancelCreateCreditAction", "Admin", new { actionId = actionId });
                case "CreateDeposit":
                    return RedirectToAction("CancelCreateDepositAction", "Admin", new { actionId = actionId });
                case "CreateInstallment":
                    return RedirectToAction("CancelCreateInstallmentAction", "Admin", new { actionId = actionId });
                case "CreateSalary":
                    return RedirectToAction("CancelCreateSalaryAction", "Admin", new { actionId = actionId });
                case "FreezeDeposit":
                    return RedirectToAction("CancelFreezeDepositAction", "Admin", new { actionId = actionId });
                case "GetDeposit":
                    return RedirectToAction("CancelGetDepositAction", "Admin", new { actionId = actionId });
                case "GetSalary":
                    return RedirectToAction("CancelGetSalaryAction", "Admin", new { actionId = actionId });
                case "OpenBalance":
                    return RedirectToAction("CancelOpenBalanceAction", "Admin", new { actionId = actionId });
                case "PayCredit":
                    return RedirectToAction("CancelPayCreditAction", "Admin", new { actionId = actionId });
                case "PayInstallment":
                    return RedirectToAction("CancelPayInstallmentAction", "Admin", new { actionId = actionId });
                case "SalaryApprovingByOperator":
                    return RedirectToAction("CancelSalaryApprovingByOperator", "Admin", new { actionId = actionId });
                case "SalaryApprovingBySpecialist":
                    return RedirectToAction("CancelSalaryApprovingBySpecialist", "Admin", new { actionId = actionId });
                case "SalaryRejectingBySpecialist":
                    return RedirectToAction("CancelSalaryRejectingBySpecialist", "Admin", new { actionId = actionId });
                case "SalaryRejectingByOperator":
                    return RedirectToAction("CancelSalaryRejectingByOperator", "Admin", new { actionId = actionId });
                case "TransferDeposit":
                    return RedirectToAction("CancelTransferDepositAction", "Admin", new { actionId = actionId });
                case "UnblockDeposit":
                    return RedirectToAction("CancelUnblockDepositAction", "Admin", new { actionId = actionId });
                case "UnfreezeDeposit":
                    return RedirectToAction("CancelUnfreezeDepositAction", "Admin", new { actionId = actionId });
            }
            return RedirectToAction("Actions", "Admin");
        }
    }
}
