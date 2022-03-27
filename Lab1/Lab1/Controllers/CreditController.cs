using Lab1.Models.CreditModels;
using Lab1.Models.Data;
using Lab1.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab1.Controllers
{
    public class CreditController : Controller
    {
        private ApplicationDbContext _context;

        public CreditController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "client")]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [Authorize(Roles = "client")]
        public async Task<IActionResult> Create(CreateCreditModel model)
        {
            var client = await _context.Clients
                .Include(c => c.Balances)
                .Include(c => c.Credits)
                .FirstOrDefaultAsync(c => c.Id == User.Identity.Name);
            var balance = client.Balances.FirstOrDefault(b => b.Name == model.BalanceName);
            var credit = new Credit
            {
                Money = model.Money,
                Percent = model.Percent + model.Months,
                MoneyWithPercent = Math.Round(model.Money * (100 + client.Percent + model.Months) / 100,
                    2, MidpointRounding.ToPositiveInfinity),
                Months = model.Months,
                //CreatingTime = DateTime.Now,
                //PaymentTime = DateTime.Now.AddMonths(model.Months),
                ClientId = client.Id
            };
            var creditApproving = new CreditApproving
            {
                CreditId = credit.Id,
                BalanceId = balance.Id
            };
           /* client.Balances.Remove(balance);
            balance.Money += credit.Money;
            client.Balances.Add(balance);*/
           _context.CreditApprovings.Add(creditApproving);
            client.Credits.Add(credit);
            _context.Credits.Add(credit);
            //_context.Balances.Update(balance);
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
            return RedirectToAction("Profile", "Client");
        }

        [HttpGet]
        [Authorize(Roles = "client")]
        public async Task<IActionResult> PayAll(string? creditId)
        {
            var client = await _context.Clients
                .Include(c => c.Balances)
                .FirstOrDefaultAsync(c => c.Id == User.Identity.Name);
            ViewBag.CreditId = creditId;
            var model = new PayCreditModel
            {
                Balances = client.Balances
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "client")]
        public async Task<IActionResult> PayAll(PayCreditModel model)
        {
            var client = await _context.Clients
                .Include(c => c.Balances)
                .Include(c => c.Credits)
                .FirstOrDefaultAsync(c => c.Id == User.Identity.Name);
            var credit = client.Credits.FirstOrDefault(c => c.Id == model.CreditId);
            var time = credit.CreatingTime.AddMonths(credit.PayedMonths);
            var nowTime = DateTime.Now;
            if (time < nowTime)
            {
                client.Credits.Remove(credit);
                var fines = (nowTime.Year - time.Year) * 12 + (nowTime.Month - time.Month);
                if (credit.Fines < fines)
                {
                    credit.MoneyWithPercent = Math.Round(credit.MoneyWithPercent * (100 + fines -
                        credit.Fines) / 100, 2, MidpointRounding.ToPositiveInfinity);
                    credit.Fines = fines;
                }
                client.Credits.Add(credit);
                _context.Credits.Update(credit);
                _context.Clients.Update(client);
                await _context.SaveChangesAsync();
            }
            var balance = client.Balances.FirstOrDefault(c => c.Id == model.BalanceId);
            if (balance.Money < credit.MoneyWithPercent)
            {
                ViewBag.CreditId = model.CreditId;
                model = new PayCreditModel
                {
                    Balances = client.Balances
                };
                return View(model);
            }
            _context.Credits.Remove(credit);
            client.Credits.Remove(credit);
            client.Balances.Remove(balance);
            balance.Money -= credit.MoneyWithPercent;
            balance.Money = Math.Round(balance.Money, 2,
                MidpointRounding.ToPositiveInfinity);
            client.Balances.Add(balance);
            _context.Balances.Update(balance);
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
            return RedirectToAction("Profile", "Client");
        }

        [HttpGet]
        [Authorize(Roles = "client")]
        public async Task<IActionResult> Pay(string? creditId)
        {
            var client = await _context.Clients
                .Include(c => c.Balances)
                .FirstOrDefaultAsync(c => c.Id == User.Identity.Name);
            ViewBag.CreditId = creditId;
            var model = new PayCreditModel
            {
                Balances = client.Balances
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "client")]
        public async Task<IActionResult> Pay(PayCreditModel model)
        {
            var client = await _context.Clients
                .Include(c => c.Balances)
                .Include(c => c.Credits)
                .FirstOrDefaultAsync(c => c.Id == User.Identity.Name);
            var credit = client.Credits.FirstOrDefault(c => c.Id == model.CreditId);
            var time = credit.CreatingTime.AddMonths(credit.PayedMonths);
            var nowTime = DateTime.Now;
            if (time < nowTime)
            {
                client.Credits.Remove(credit);
                var fines = (nowTime.Year - time.Year) * 12 + (nowTime.Month - time.Month);
                if (credit.Fines < fines)
                {
                    credit.MoneyWithPercent = Math.Round(credit.MoneyWithPercent * (100 + fines -
                        credit.Fines) / 100, 2, MidpointRounding.ToPositiveInfinity);
                    credit.Fines += fines;
                }
                client.Credits.Add(credit);
                _context.Credits.Update(credit);
                _context.Clients.Update(client);
                await _context.SaveChangesAsync();
            }
            var balance = client.Balances.FirstOrDefault(c => c.Id == model.BalanceId);
            var payMoney = Math.Round(credit.MoneyWithPercent / (credit.Months - credit.PayedMonths), 2,
                MidpointRounding.ToPositiveInfinity);
            if (balance.Money < payMoney)
            {
                ViewBag.CreditId = model.CreditId;
                model = new PayCreditModel
                {
                    Balances = client.Balances
                };
                return View(model);
            }
            client.Balances.Remove(balance);
            balance.Money -= payMoney;
            balance.Money = Math.Round(balance.Money, 2,
                MidpointRounding.ToPositiveInfinity);
            client.Balances.Add(balance);
            _context.Balances.Update(balance);
            client.Credits.Remove(credit);
            credit.MoneyWithPercent -= payMoney;
            if (credit.MoneyWithPercent == 0)
            {
                _context.Credits.Remove(credit);
            }
            else
            {
                credit.MoneyWithPercent = Math.Round(credit.MoneyWithPercent, 2,
                    MidpointRounding.ToPositiveInfinity);
                credit.PayedMonths += 1;
                client.Credits.Add(credit);
                _context.Credits.Update(credit);
            }
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
            return RedirectToAction("Profile", "Client");
        }
    }
}
