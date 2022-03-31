using Lab1.Models.BalanceModels;
using Lab1.Models.Data;
using Lab1.Models.Entities;
using Lab1.Models.Entities.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab1.Controllers
{
    public class BalanceController : Controller
    {
        private ApplicationDbContext _context;

        public BalanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "client, specialist")]
        public IActionResult Open()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "client, specialist")]
        public async Task<IActionResult> Open(OpenBalanceModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == User.Identity.Name);
            var balance = new Balance
            {
                Name = model.Name,
                Money = model.Money,
                ClientId = user.Id
            };
            _context.Balances.Add(balance);
            switch(user.RoleName)
            {
                case "client":
                    var client = await _context.Clients
                        .Include(c => c.Balances)
                        .FirstOrDefaultAsync(c => c.Id == User.Identity.Name);
                    client.Balances.Add(balance);
                    _context.Clients.Update(client);
                    var openBalanceAction = new OpenBalanceAction
                    {
                        UserId = client.Id,
                        UserEmail = client.Email,
                        BalanceId = balance.Id,
                        BalanceName = balance.Name,
                        Info = $"Клиент {client.Email} открыл счет {balance.Name} с денежной суммой в размере {model.Money}."
                    };
                    _context.OpenBalanceActions.Add(openBalanceAction);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Profile", "Client");
                case "specialist":
                    var specialist = await _context.Specialists
                        .Include(c => c.Balances)
                        .FirstOrDefaultAsync(c => c.Id == User.Identity.Name);
                    specialist.Balances.Add(balance);
                    _context.Specialists.Update(specialist);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Profile", "Specialist");
            }
            return View(model);
        }

        [Authorize(Roles = "client, specialist")]
        public async Task<IActionResult> Close(string? balanceId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == User.Identity.Name);
            switch (user.RoleName)
            {
                case "client":
                    var client = await _context.Clients
                        .Include(c => c.Balances)
                        .FirstOrDefaultAsync(c => c.Id == User.Identity.Name);
                    var cBalance = client.Balances.FirstOrDefault(b => b.Id == balanceId);
                    _context.Balances.Remove(cBalance);
                    client.Balances.Remove(cBalance);
                    _context.Clients.Update(client);
                    var cBalanceTransferActions = _context.BalanceTransferActions
                        .Where(a => a.BalanceIdFrom == cBalance.Id || a.BalanceIdTo ==
                            cBalance.Id).ToList();
                    foreach (var balanceTransferAction in cBalanceTransferActions)
                    {
                        if (balanceTransferAction.BalanceIdFrom == cBalance.Id)
                        {
                            balanceTransferAction.BalanceIdFrom = null;
                        }
                        else
                        {
                            balanceTransferAction.BalanceIdTo = null;
                        }
                        _context.BalanceTransferActions.Update(balanceTransferAction);
                    }
                    var closeBalanceAction = new CloseBalanceAction
                    {
                        UserId = client.Id,
                        UserEmail = client.Email,
                        BalanceId = cBalance.Id,
                        BalanceName = cBalance.Name,
                        Money = cBalance.Money,
                        Info = $"Клиент {client.Email} закрыл счет {cBalance.Name} с денежной суммой в размере {cBalance.Money}."
                    };
                    _context.CloseBalanceActions.Add(closeBalanceAction);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Profile", "Client");
                case "specialist":
                    var specialist = await _context.Specialists
                        .Include(c => c.Balances)
                        .FirstOrDefaultAsync(c => c.Id == User.Identity.Name);
                    var sBalance = specialist.Balances.FirstOrDefault(b => b.Id == balanceId);
                    _context.Balances.Remove(sBalance);
                    specialist.Balances.Remove(sBalance);
                    _context.Specialists.Update(specialist);
                    var sBalanceTransferActions = _context.BalanceTransferActions
                        .Where(a => a.BalanceIdFrom == sBalance.Id || a.BalanceIdTo ==
                            sBalance.Id).ToList();
                    foreach (var balanceTransferAction in sBalanceTransferActions)
                    {
                        if (balanceTransferAction.BalanceIdFrom == sBalance.Id)
                        {
                            balanceTransferAction.BalanceIdFrom = null;
                        }
                        else
                        {
                            balanceTransferAction.BalanceIdTo = null;
                        }
                        _context.BalanceTransferActions.Update(balanceTransferAction);
                    }
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Profile", "Specialist");
            }
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "client, specialist")]
        public IActionResult Add(string? balanceId)
        {
            ViewBag.BalanceId = balanceId;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "client, specialist")]
        public async Task<IActionResult> Add(AddBalanceModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == User.Identity.Name);
            switch (user.RoleName)
            {
                case "client":
                    var client = await _context.Clients
                        .Include(c => c.Balances)
                        .FirstOrDefaultAsync(c => c.Id == User.Identity.Name);
                    var cBalance = client.Balances.FirstOrDefault(b => b.Id == model.Id);
                    cBalance.Money += model.Money;
                    _context.Balances.Update(cBalance);
                    client.Balances.Remove(cBalance);
                    client.Balances.Add(cBalance);
                    _context.Clients.Update(client);
                    var addBalanceAction = new AddBalanceAction
                    {
                        UserId = client.Id,
                        UserEmail = client.Email,
                        BalanceId = cBalance.Id,
                        BalanceName = cBalance.Name,
                        Money = model.Money,
                        Info = $"Клиент {client.Email} пополнил счет {cBalance.Name} на денежную сумму в размере {model.Money}."
                    };
                    _context.AddBalanceActions.Add(addBalanceAction);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Profile", "Client");
                case "specialist":
                    var specialist = await _context.Specialists
                        .Include(c => c.Balances)
                        .FirstOrDefaultAsync(c => c.Id == User.Identity.Name);
                    var sBalance = specialist.Balances.FirstOrDefault(b => b.Id == model.Id);
                    sBalance.Money += model.Money;
                    _context.Balances.Update(sBalance);
                    specialist.Balances.Remove(sBalance);
                    specialist.Balances.Add(sBalance);
                    _context.Specialists.Update(specialist);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Profile", "Specialist");
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "client, specialist")]
        public IActionResult Transfer(string? balanceId)
        {
            ViewBag.BalanceId = balanceId;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "client, specialist")]
        public async Task<IActionResult> Transfer(TransferBalanceModel model)
        {
            if (model.Money == 0)
            {
                ModelState.AddModelError("", "Минимальная сумма перевода 0.01");
                return View(model);
            }
            var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == User.Identity.Name);
            var balanceTransferAction = new BalanceTransferAction
            {
                Money = model.Money,
            };
            switch (user.RoleName)
            {
                case "client":
                    var clientFrom = await _context.Clients
                        .Include(c => c.Balances)
                        .FirstOrDefaultAsync(c => c.Id == User.Identity.Name);
                    var cBank = await _context.Banks.FirstOrDefaultAsync(b => b.Name == model.BankNameTo);
                    if (cBank == null)
                    {
                        ModelState.AddModelError("", "Указанный банк отсутствует в системе");
                        return View(model);
                    }
                    var cBankIdTo = cBank.Id;
                    var clientTo = await _context.Clients
                        .Include(c => c.Balances)
                        .FirstOrDefaultAsync(c => c.Email == model.EmailTo && c.BankId == cBankIdTo);
                    if (clientTo == null)
                    {
                        ModelState.AddModelError("", "Счет не найден");
                        return View(model);
                    }
                    var cBalanceFrom = clientFrom.Balances.FirstOrDefault(b => b.Id == model.IdFrom);
                    var cBalanceTo = clientTo.Balances.FirstOrDefault(b => b.Name == model.BalanceNameTo);
                    if (cBalanceTo == null)
                    {
                        ModelState.AddModelError("", "Счет не найден");
                        return View(model);
                    }
                    if (cBalanceFrom.Id == cBalanceTo.Id)
                    {
                        ModelState.AddModelError("", "Невозможная операция");
                        return View(model);
                    }
                    var bankFrom = await _context.Banks.FirstOrDefaultAsync(b => b.Id ==
                        clientFrom.BankId);
                    var bankTo = await _context.Banks.FirstOrDefaultAsync(b => b.Id ==
                        clientTo.BankId);

                    balanceTransferAction.BankIdFrom = bankFrom.Id;
                    balanceTransferAction.BankIdTo = bankTo.Id;
                    balanceTransferAction.BankNameFrom = bankFrom.Name;
                    balanceTransferAction.BankNameTo = bankTo.Name;
                    balanceTransferAction.UserId = clientFrom.Id;
                    balanceTransferAction.UserIdTo = clientTo.Id;
                    balanceTransferAction.UserEmailFrom = clientFrom.Email;
                    balanceTransferAction.UserEmailTo = clientTo.Email;
                    balanceTransferAction.BalanceIdFrom = cBalanceFrom.Id;
                    balanceTransferAction.BalanceIdTo = cBalanceTo.Id;
                    balanceTransferAction.BalanceNameFrom = cBalanceFrom.Name;
                    balanceTransferAction.BalanceNameTo = cBalanceTo.Name;

                    clientFrom.Balances.Remove(cBalanceFrom);
                    cBalanceFrom.Money -= model.Money;
                    clientFrom.Balances.Add(cBalanceFrom);

                    clientTo.Balances.Remove(cBalanceTo);
                    cBalanceTo.Money += model.Money;
                    clientTo.Balances.Add(cBalanceTo);

                    _context.BalanceTransferActions.Add(balanceTransferAction);
                    _context.Balances.Update(cBalanceFrom);
                    _context.Balances.Update(cBalanceTo);
                    _context.Clients.Update(clientFrom);
                    _context.Clients.Update(clientTo);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Profile", "Client");
                case "specialist":
                    var specialistFrom = await _context.Specialists
                        .Include(c => c.Balances)
                        .FirstOrDefaultAsync(c => c.Id == User.Identity.Name);
                    var sBank = await _context.Banks.FirstOrDefaultAsync(b => b.Name == model.BankNameTo);
                    if (sBank == null)
                    {
                        ModelState.AddModelError("", "Указанный банк отсутствует в системе");
                        return View(model);
                    }
                    var sBankIdTo = sBank.Id;
                    var specialistTo = await _context.Specialists
                        .Include(s => s.Balances)
                        .FirstOrDefaultAsync(s => s.Email == model.EmailTo && s.BankId == sBankIdTo
                            && s.CompanyId == specialistFrom.CompanyId);
                    if (specialistTo == null)
                    {
                        ModelState.AddModelError("", "Счет не найден");
                        return View(model);
                    }
                    var sBalanceFrom = specialistFrom.Balances.FirstOrDefault(b => b.Id == model.IdFrom);
                    var sBalanceTo = specialistTo.Balances.FirstOrDefault(b => b.Name == model.BalanceNameTo);
                    if (sBalanceTo == null)
                    {
                        ModelState.AddModelError("", "Счет не найден");
                        return View(model);
                    }
                    if (sBalanceFrom.Id == sBalanceTo.Id)
                    {
                        ModelState.AddModelError("", "Невозможная операция");
                        return View(model);
                    }

                    specialistFrom.Balances.Remove(sBalanceFrom);
                    sBalanceFrom.Money -= model.Money;
                    specialistFrom.Balances.Add(sBalanceFrom);

                    var balanceTransferApproving = new BalanceTransferApproving
                    {
                        Money = model.Money,
                        BalanceIdFrom = sBalanceFrom.Id,
                        BalanceIdTo = sBalanceTo.Id
                    };
                    _context.BalanceTransferApprovings.Add(balanceTransferApproving);
                    _context.Balances.Update(sBalanceFrom);
                    _context.Specialists.Update(specialistFrom);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Profile", "Specialist");
            }
            return View(model);
        }
    }
}
