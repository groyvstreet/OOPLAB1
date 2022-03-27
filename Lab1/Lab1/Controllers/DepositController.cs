using Lab1.Models.Data;
using Lab1.Models.DepositModels;
using Lab1.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab1.Controllers
{
    public class DepositController : Controller
    {
        private ApplicationDbContext _context;

        public DepositController(ApplicationDbContext context)
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
        public async Task<IActionResult> Create(CreateDepositModel model)
        {
            if (ModelState.IsValid)
            {
                var client = await _context.Clients
                    .Include(c => c.Deposits)
                    .Include(c => c.Balances)
                    .FirstOrDefaultAsync(c => c.Id == User.Identity.Name);
                Deposit deposit = new Deposit
                {
                    Money = model.Money,
                    Percent = model.Percent,
                    ClientId = client.Id,
                    ClosedTime = model.ClosedTime
                };
                var balance = client.Balances.FirstOrDefault(b => b.Name == model.BalanceName);
                if (balance != null)
                {
                    if(balance.Money < model.Money)
                    {
                        return View();
                    }
                    client.Balances.Remove(balance);
                    balance.Money -= model.Money;
                    client.Balances.Add(balance);
                    _context.Balances.Update(balance);
                }
                _context.Deposits.Add(deposit);
                client.Deposits.Add(deposit);
                _context.Clients.Update(client);
                await _context.SaveChangesAsync();
                return RedirectToAction("Profile", "Client");
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "client")]
        public IActionResult Add(string? depositId)
        {
            ViewBag.DepositId = depositId;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "client")]
        public async Task<IActionResult> Add(AddDepositModel model)
        {
            var client = await _context.Clients.Include(c => c.Deposits)
                .FirstOrDefaultAsync(c => c.Id == User.Identity.Name);
            var deposit = client.Deposits.FirstOrDefault(d => d.Id == model.Id);
            if (deposit.ClosedTime >= DateTime.Now)
            {
                client.Deposits.Remove(deposit);
                deposit.Money += model.Money;
                client.Deposits.Add(deposit);
                _context.Deposits.Update(deposit);
                _context.Clients.Update(client);
                await _context.SaveChangesAsync();
                return RedirectToAction("Profile", "Client");
            }
            return RedirectToAction("Profile", "Client");
        }

        [HttpGet]
        [Authorize(Roles = "client")]
        public async Task<IActionResult> Transfer(string? depositId)
        {
            ViewBag.DepositId = depositId;
            var client = await _context.Clients.Include(c => c.Deposits)
                .FirstOrDefaultAsync(c => c.Id == User.Identity.Name);
            if(client.Deposits.Count == 1)
            {
                return RedirectToAction("Profile", "Client");
            }
            var deposit = client.Deposits.First(d => d.Id == depositId);
            var model = new TransferDepositModel
            {
                Deposits = client.Deposits,
                NowTime = DateTime.Now,
                IdFrom = depositId,
                Money = deposit.Money
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "client")]
        public async Task<IActionResult> Transfer(TransferDepositModel model)
        {
            var client = await _context.Clients.Include(c => c.Deposits)
                .FirstOrDefaultAsync(c => c.Id == User.Identity.Name);
            var depositFrom = client.Deposits.FirstOrDefault(d => d.Id == model.IdFrom);
            var depositTo = client.Deposits.FirstOrDefault(d => d.Id == model.IdTo);
            if (depositTo.ClosedTime >= DateTime.Now)
            {
                depositTo.Money += Math.Round((depositFrom.Percent + 100) * depositFrom.Money / 100, 2);
                _context.Deposits.Remove(depositFrom);
                _context.Deposits.Update(depositTo);
                client.Deposits.Remove(depositFrom);
                client.Deposits.Add(depositTo);
                _context.Clients.Update(client);
                _context.SaveChanges();
                return RedirectToAction("Profile", "Client");
            }
            ViewBag.DepositId = depositFrom.Id;
            model = new TransferDepositModel
            {
                Deposits = client.Deposits,
                NowTime = DateTime.Now,
                IdFrom = depositFrom.Id,
                Money = depositFrom.Money
            };
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "client")]
        public async Task<IActionResult> Get(string? depositId)
        {
            var client = await _context.Clients
                .Include(c => c.Deposits)
                .Include(c => c.Balances)
                .FirstOrDefaultAsync(c => c.Id == User.Identity.Name);
            var deposit = client.Deposits.FirstOrDefault(d => d.Id == depositId);
            double money;
            if (DateTime.Now >= deposit.ClosedTime)
            {
                money = Math.Round((deposit.Percent + 100) * deposit.Money / 100, 2);
            }
            else
            {
                money = Math.Round(((DateTime.Now - deposit.OpenedTime) / (deposit.ClosedTime - deposit.OpenedTime)
                    * (deposit.Percent) + 100) * deposit.Money / 100, 2);
            }
            ViewBag.DepositId = depositId;
            ViewBag.Money = money;
            var model = new GetDepositModel
            {
                Balances = client.Balances,
                Money = money
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "client")]
        public async Task<IActionResult> Get(GetDepositModel model)
        {
            var client = await _context.Clients
                .Include(c => c.Deposits)
                .Include(c => c.Balances)
                .FirstOrDefaultAsync(c => c.Id == User.Identity.Name);
            var deposit = client.Deposits.FirstOrDefault(d => d.Id == model.DepositId);
            _context.Deposits.Remove(deposit);
            client.Deposits.Remove(deposit);
            var balance = client.Balances.FirstOrDefault(d => d.Id == model.BalanceId);
            client.Balances.Remove(balance);
            balance.Money += model.Money;
            _context.Balances.Update(balance);
            client.Balances.Add(balance);
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
            return RedirectToAction("Profile", "Client");
        }
    }
}
