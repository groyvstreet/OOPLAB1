using Lab1.Models;
using Lab1.Models.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab1.Models.Entities;
using Lab1.Models.ClientModels;

namespace Lab1.Controllers
{
    public class ClientController : Controller
    {
        private ApplicationDbContext _context;

        public ClientController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "client")]
        public async Task<IActionResult> Profile()
        {
            var client = await _context.Clients
                .Include(c => c.Deposits)
                .Include(c => c.Balances)
                .Include(c => c.Credits)
                .Include(c => c.Installments)
                .Include(c => c.Salary)
                .FirstOrDefaultAsync(c => c.Id == User.Identity.Name);
            foreach (var credit in client.Credits)
            {
                if (credit.Approved)
                {
                    var time = credit.CreatingTime.AddMonths(credit.PayedMonths);
                    var nowTime = DateTime.Now;
                    if (time < nowTime)
                    {
                        var fines = (nowTime.Year - time.Year) * 12 + (nowTime.Month - time.Month);
                        if (credit.Fines < fines)
                        {
                            credit.MoneyWithPercent = Math.Round(credit.MoneyWithPercent * (100 + fines -
                                credit.Fines) / 100, 2, MidpointRounding.ToPositiveInfinity);
                            credit.Fines += fines;
                        }
                        _context.Credits.Update(credit);
                        _context.Clients.Update(client);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id == client.CompanyId);
            var bank = await _context.Banks.FirstOrDefaultAsync(b => b.Id == client.BankId);
            ClientProfileModel model = new ClientProfileModel
            {
                Email = client.Email,
                FirstName = client.FirstName,
                LastName = client.LastName,
                Patronymic = client.Patronymic,
                PhoneNumber = client.PhoneNumber,
                PassportSeries = client.PassportSeries,
                PassportNumber = client.PassportNumber,
                IdentificationNumber = client.IdentificationNumber,
                Deposits = client.Deposits,
                NowTime = DateTime.Now,
                Balances = client.Balances,
                Credits = client.Credits,
                Installments = client.Installments,
                Approved = client.Approved,
                Salary = client.Salary,
                Percent = client.Percent,
                CompanyName = company.LegalName,
                BankName = bank.Name
            };
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "client")]
        public IActionResult SignUp()
        {
            var companies = _context.Companies.ToList();
            var model = new SignUpClientModel
            {
                Companies = companies
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "client")]
        public async Task<IActionResult> SignUp(SignUpClientModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == User.Identity.Name);
            var bank = await _context.Banks.FirstOrDefaultAsync(b => b.Id == user.BankId);
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id == model.CompanyId);
            var client = new Client
            {
                Id = user.Id,
                Email = user.Email,
                Password = user.Password,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Patronymic = user.Patronymic,
                PhoneNumber = user.PhoneNumber,
                RoleName = user.RoleName,
                BankId = user.BankId,
                PassportSeries = model.PassportSeries,
                PassportNumber = model.PassportNumber,
                IdentificationNumber = model.IdentificationNumber,
                Approved = false,
                CompanyId = model.CompanyId,
                Percent = bank.Percent + company.Percent
            };
            var signUpApproving = new SignUpApproving
            {
                ClientId = client.Id
            };
            _context.SignUpApprovings.Add(signUpApproving);
            _context.Users.Remove(user);
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            return RedirectToAction("Profile", "Client");
        }

        [Authorize(Roles = "client")]
        public async Task<IActionResult> CreateSalary()
        {
            var client = await _context.Clients
                .Include(c => c.Salary)
                .FirstOrDefaultAsync(c => c.Id == User.Identity.Name);
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id == client.CompanyId);
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
            await _context.SaveChangesAsync();
            return RedirectToAction("Profile", "Client");
        }

        [Authorize(Roles = "client")]
        public async Task<IActionResult> GetSalary(string balanceId)
        {
            var client = await _context.Clients
                .Include(c => c.Balances)
                .Include(c => c.Salary)
                .FirstOrDefaultAsync(c => c.Id == User.Identity.Name);
            var balance = client.Balances.FirstOrDefault(b => b.Id == balanceId);
            client.Balances.Remove(balance);
            balance.Money += client.Salary.Money;
            client.Balances.Add(balance);
            _context.Balances.Update(balance);
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
            return RedirectToAction("Profile", "Client");
        }
    }
}
