using Lab1.Models.InstallmentModels;
using Lab1.Models.Data;
using Lab1.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab1.Models.Entities.Actions;

namespace Lab1.Controllers
{
    public class InstallmentController : Controller
    {
        private ApplicationDbContext _context;

        public InstallmentController(ApplicationDbContext context)
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
        public async Task<IActionResult> Create(CreateInstallmentModel model)
        {
            double modelMoney = double.Parse(model.Money.Replace(".", ","));
            if (modelMoney < 1000)
            {
                ModelState.AddModelError("", "Минимальная сумма - 1000");
                return View(model);
            }
            if (modelMoney > 1000000000)
            {
                ModelState.AddModelError("", "Максимальная сумма взятия рассрочки - 1 000 000 000");
                return View(model);
            }
            if (model.Months > 120)
            {
                ModelState.AddModelError("", "Максимальное количество месяцев - 120 (10 лет)");
                return View(model);
            }
            var client = await _context.Clients
                .Include(c => c.Balances)
                .Include(c => c.Installments)
                .FirstOrDefaultAsync(c => c.Id == User.Identity.Name);
            var balance = client.Balances.FirstOrDefault(b => b.Name == model.BalanceName);
            if (balance == null)
            {
                ModelState.AddModelError("", "Указанный счет не найден");
                return View(model);
            }
            var installment = new Installment
            {
                Money = modelMoney,
                PayMoney = modelMoney,
                Months = model.Months,
                ClientId = client.Id
            };
            var installmentApproving = new InstallmentApproving
            {
                InstallmentId = installment.Id,
                BalanceId = balance.Id
            };
            _context.InstallmentApprovings.Add(installmentApproving);
            client.Installments.Add(installment);
            _context.Installments.Add(installment);
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
            return RedirectToAction("Profile", "Client");
        }

        [HttpGet]
        [Authorize(Roles = "client")]
        public async Task<IActionResult> PayAll(string? installmentId)
        {
            var client = await _context.Clients
                .Include(c => c.Balances)
                .FirstOrDefaultAsync(c => c.Id == User.Identity.Name);
            ViewBag.InstallmentId = installmentId;
            var model = new PayInstallmentModel
            {
                Balances = client.Balances
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "client")]
        public async Task<IActionResult> PayAll(PayInstallmentModel model)
        {
            var client = await _context.Clients
                .Include(c => c.Balances)
                .Include(c => c.Installments)
                .FirstOrDefaultAsync(c => c.Id == User.Identity.Name);
            var installment = client.Installments.FirstOrDefault(i => i.Id == model.InstallmentId);
            var balance = client.Balances.FirstOrDefault(c => c.Id == model.BalanceId);
            if (balance.Money < installment.PayMoney)
            {
                ViewBag.InstallmentId = model.InstallmentId;
                model = new PayInstallmentModel
                {
                    Balances = client.Balances
                };
                return View(model);
            }
            _context.Installments.Remove(installment);
            client.Installments.Remove(installment);
            client.Balances.Remove(balance);
            balance.Money -= installment.PayMoney;
            balance.Money = Math.Round(balance.Money, 2, MidpointRounding.ToPositiveInfinity);
            client.Balances.Add(balance);
            _context.Balances.Update(balance);
            _context.Clients.Update(client);
            var payInstallmentAction = new PayInstallmentAction
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
                SinglePaymentMoney = installment.PayMoney,
                Info = $"Клиент {client.Email} выплатил сумму рассрочки в размере {installment.PayMoney}.",
                Type = "PayInstallment"
            };
            _context.PayInstallmentActions.Add(payInstallmentAction);
            await _context.SaveChangesAsync();
            return RedirectToAction("Profile", "Client");
        }

        [HttpGet]
        [Authorize(Roles = "client")]
        public async Task<IActionResult> Pay(string? installmentId)
        {
            var client = await _context.Clients
                .Include(c => c.Balances)
                .FirstOrDefaultAsync(c => c.Id == User.Identity.Name);
            ViewBag.InstallmentId = installmentId;
            var model = new PayInstallmentModel
            {
                Balances = client.Balances
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "client")]
        public async Task<IActionResult> Pay(PayInstallmentModel model)
        {
            var client = await _context.Clients
                .Include(c => c.Balances)
                .Include(c => c.Installments)
                .FirstOrDefaultAsync(c => c.Id == User.Identity.Name);
            var installment = client.Installments.FirstOrDefault(i => i.Id == model.InstallmentId);
            var balance = client.Balances.FirstOrDefault(c => c.Id == model.BalanceId);
            var payMoney = Math.Round(installment.PayMoney / (installment.Months -
                installment.PayedMonths), 2, MidpointRounding.ToPositiveInfinity);
            if (balance.Money < payMoney)
            {
                ViewBag.InstallmentId = model.InstallmentId;
                model = new PayInstallmentModel
                {
                    Balances = client.Balances
                };
                return View(model);
            }
            var payInstallmentAction = new PayInstallmentAction
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
                SinglePaymentMoney = payMoney,
                Info = $"Клиент {client.Email} выплатил сумму рассрочки в размере {payMoney}.",
                Type = "PayInstallment"
            };
            _context.PayInstallmentActions.Add(payInstallmentAction);
            client.Balances.Remove(balance);
            balance.Money -= payMoney;
            balance.Money = Math.Round(balance.Money, 2, MidpointRounding.ToPositiveInfinity);
            client.Balances.Add(balance);
            _context.Balances.Update(balance);
            client.Installments.Remove(installment);
            installment.PayMoney -= payMoney;
            if (installment.PayMoney == 0)
            {
                _context.Installments.Remove(installment);
            }
            else
            {
                installment.PayMoney = Math.Round(installment.PayMoney, 2, MidpointRounding.ToPositiveInfinity);
                installment.PayedMonths += 1;
                client.Installments.Add(installment);
                _context.Installments.Update(installment);
            }
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
            return RedirectToAction("Profile", "Client");
        }
    }
}
