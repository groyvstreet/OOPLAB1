using Lab1.Models.CreditModels;
using Lab1.Models.Data;
using Lab1.Models.Entities;
using Lab1.Models.Entities.Actions;
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
            double modelMoney = double.Parse(model.Money.Replace(".", ","));
            if (modelMoney < 1000)
            {
                ModelState.AddModelError("", "Минимальная сумма - 1000");
                ViewBag.Percent = model.Percent;
                return View(model);
            }
            if (model.Months > 120)
            {
                ModelState.AddModelError("", "Максимальное количество месяцев - 120 (10 лет)");
                ViewBag.Percent = model.Percent;
                return View(model);
            }
            var client = await _context.Clients
                .Include(c => c.Balances)
                .Include(c => c.Credits)
                .FirstOrDefaultAsync(c => c.Id == User.Identity.Name);
            var balance = client.Balances.FirstOrDefault(b => b.Name == model.BalanceName);
            if (balance == null)
            {
                ModelState.AddModelError("", "Указанный счет не найден");
                ViewBag.Percent = model.Percent;
                return View(model);
            }
            var credit = new Credit
            {
                Money = modelMoney,
                Percent = model.Percent + model.Months,
                MoneyWithPercent = Math.Round(modelMoney * (100 + client.Percent + model.Months) / 100,
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
            var payCreditAction = new PayCreditAction
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
                SinglePaymentMoney = credit.MoneyWithPercent,
                Info = $"Клиент {client.Email} выплатил сумму рассрочки в размере {credit.MoneyWithPercent}.",
                Type = "PayInstallment"
            };
            _context.PayCreditActions.Add(payCreditAction);
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
            var payCreditAction = new PayCreditAction
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
                SinglePaymentMoney = payMoney,
                Info = $"Клиент {client.Email} выплатил сумму кредита в размере {payMoney}.",
                Type = "PayCredit"
            };
            _context.PayCreditActions.Add(payCreditAction);
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
