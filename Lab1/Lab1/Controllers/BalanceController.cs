using Lab1.Models.BalanceModels;
using Lab1.Models.Data;
using Lab1.Models.Entities;
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
            var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == User.Identity.Name);
            switch (user.RoleName)
            {
                case "client":
                    var clientFrom = await _context.Clients
                        .Include(c => c.Balances)
                        .FirstOrDefaultAsync(c => c.Id == User.Identity.Name);
                    var cBank = await _context.Banks.FirstOrDefaultAsync(b => b.Name == model.BankNameTo);
                    var cBankIdTo = cBank.Id;
                    var clientTo = await _context.Clients
                        .Include(c => c.Balances)
                        .FirstOrDefaultAsync(c => c.Email == model.EmailTo && c.BankId == cBankIdTo);
                    var cBalanceFrom = clientFrom.Balances.FirstOrDefault(b => b.Id == model.IdFrom);
                    var cBalanceTo = clientTo.Balances.FirstOrDefault(b => b.Name == model.BalanceNameTo);

                    clientFrom.Balances.Remove(cBalanceFrom);
                    cBalanceFrom.Money -= model.Money;
                    clientFrom.Balances.Add(cBalanceFrom);

                    clientTo.Balances.Remove(cBalanceTo);
                    cBalanceTo.Money += model.Money;
                    clientTo.Balances.Add(cBalanceTo);

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
                    var sBankIdTo = sBank.Id;
                    var specialistTo = await _context.Specialists
                        .Include(s => s.Balances)
                        .FirstOrDefaultAsync(s => s.Email == model.EmailTo && s.BankId == sBankIdTo
                            && s.CompanyId == specialistFrom.CompanyId);
                    var sBalanceFrom = specialistFrom.Balances.FirstOrDefault(b => b.Id == model.IdFrom);
                    var sBalanceTo = specialistTo.Balances.FirstOrDefault(b => b.Name == model.BalanceNameTo);

                    specialistFrom.Balances.Remove(sBalanceFrom);
                    sBalanceFrom.Money -= model.Money;
                    specialistFrom.Balances.Add(sBalanceFrom);

                    specialistTo.Balances.Remove(sBalanceTo);
                    sBalanceTo.Money += model.Money;
                    specialistTo.Balances.Add(sBalanceTo);

                    _context.Balances.Update(sBalanceFrom);
                    _context.Balances.Update(sBalanceTo);
                    _context.Specialists.Update(specialistFrom);
                    _context.Specialists.Update(specialistTo);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Profile", "Specialist");
            }
            return View(model);
        }
    }
}
