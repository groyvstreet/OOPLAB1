using Lab1.Models.InstallmentModels;
using Lab1.Models.Data;
using Lab1.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            var client = await _context.Clients
                .Include(c => c.Balances)
                .Include(c => c.Installments)
                .FirstOrDefaultAsync(c => c.Id == User.Identity.Name);
            var balance = client.Balances.FirstOrDefault(b => b.Name == model.BalanceName);
            var installment = new Installment
            {
                Money = model.Money,
                PayMoney = model.Money,
                Months = model.Months,
                //CreatingTime = DateTime.Now,
                //PaymentTime = DateTime.Now.AddMonths(model.Months),
                ClientId = client.Id
            };
            var installmentApproving = new InstallmentApproving
            {
                InstallmentId = installment.Id,
                BalanceId = balance.Id
            };
            /*client.Balances.Remove(balance);
            balance.Money += installment.Money;
            client.Balances.Add(balance);*/
            _context.InstallmentApprovings.Add(installmentApproving);
            client.Installments.Add(installment);
            _context.Installments.Add(installment);
            //_context.Balances.Update(balance);
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
            client.Balances.Remove(balance);
            balance.Money -= payMoney;
            balance.Money = Math.Round(balance.Money, 2,
                MidpointRounding.ToPositiveInfinity);
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
                installment.PayMoney = Math.Round(installment.PayMoney, 2,
                    MidpointRounding.ToPositiveInfinity);
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
