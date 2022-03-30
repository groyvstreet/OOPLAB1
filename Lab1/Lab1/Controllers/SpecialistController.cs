using Lab1.Models.Data;
using Lab1.Models.Entities;
using Lab1.Models.Entities.Actions;
using Lab1.Models.SpecialistModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab1.Controllers
{
    public class SpecialistController : Controller
    {
        private ApplicationDbContext _context;

        public SpecialistController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "specialist")]
        public async Task<IActionResult> Profile()
        {
            var specialist = await _context.Specialists
                .Include(s => s.Balances)
                .FirstOrDefaultAsync(s => s.Id == User.Identity.Name);
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id == specialist.CompanyId);
            var bank = await _context.Banks.FirstOrDefaultAsync(b => b.Id == specialist.BankId);
            var model = new SpecialistProfileModel
            {
                Email = specialist.Email,
                FirstName = specialist.FirstName,
                LastName = specialist.LastName,
                Patronymic = specialist.Patronymic,
                PhoneNumber = specialist.PhoneNumber,
                Balances = specialist.Balances,
                CompanyName = company.LegalName,
                BankName = bank.Name
            };
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "specialist")]
        public IActionResult SignUp()
        {
            var companies = _context.Companies.ToList();
            var model = new SignUpSpecialistModel
            {
                Companies = companies
            };
            return View(model);
        }

        [Authorize(Roles = "specialist")]
        public async Task<IActionResult> SignUp(string? companyId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == User.Identity.Name);
            var specialist = new Specialist
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
                CompanyId = companyId
            };
            _context.Users.Remove(user);
            _context.Specialists.Add(specialist);
            await _context.SaveChangesAsync();
            return RedirectToAction("Profile", "Specialist");
        }

        [Authorize(Roles = "specialist")]
        public async Task<IActionResult> SalaryApprovings()
        {
            var specialist = await _context.Specialists.FirstOrDefaultAsync(m => m.Id == User.Identity.Name);
            var salaryApprovings = _context.SalaryApprovings;
            var clients = new List<Client>();
            foreach (var approving in salaryApprovings)
            {
                var client = await _context.Clients
                    .Include(c => c.Salary)
                    .FirstOrDefaultAsync(c => c.Id == approving.ClientId && c.BankId ==
                        specialist.BankId && c.CompanyId == specialist.CompanyId);
                if (client != null)
                {
                    if (client.Salary.ApprovedBySpecialist == false)
                    {
                        clients.Add(client);
                    }
                }
            }
            var model = new SalaryApprovingsSpecialistModel
            {
                Clients = clients
            };
            return View(model);
        }

        [Authorize(Roles = "specialist")]
        public async Task<IActionResult> ApproveSalary(string clientId)
        {
            var client = await _context.Clients
                .Include(c => c.Salary)
                .FirstOrDefaultAsync(c => c.Id == clientId);
            client.Salary.ApprovedBySpecialist = true;
            _context.Clients.Update(client);
            var specialist = await _context.Specialists.FirstOrDefaultAsync(s => s.Id ==
                User.Identity.Name);
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id ==
                specialist.CompanyId);
            var salaryApprovingBySpecialistAction = new SalaryApprovingBySpecialistAction
            {
                SpecialistId = specialist.Id,
                SpecialistEmail = specialist.Email,
                ClientId = client.Id,
                ClientEmail = client.Email,
                CompanyName = company.LegalName,
                Info = $"Специалист {specialist.Email} подтвердил отправку документов клиента {client.Email}."
            };
            _context.SalaryApprovingBySpecialistActions.Add(salaryApprovingBySpecialistAction);
            await _context.SaveChangesAsync();
            return RedirectToAction("SalaryApprovings", "Specialist");
        }

        [Authorize(Roles = "specialist")]
        public async Task<IActionResult> RejectSalary(string clientId)
        {
            var client = await _context.Clients
                .Include(c => c.Salary)
                .FirstOrDefaultAsync(c => c.Id == clientId);
            client.Salary = null;
            _context.Clients.Update(client);
            var salaryApproving = await _context.SalaryApprovings.FirstOrDefaultAsync(s => s.ClientId == clientId);
            _context.SalaryApprovings.Remove(salaryApproving);
            var specialist = await _context.Specialists.FirstOrDefaultAsync(s => s.Id ==
                User.Identity.Name);
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id ==
                specialist.CompanyId);
            var salaryRejectingingBySpecialistAction = new SalaryRejectingBySpecialistAction
            {
                SpecialistId = specialist.Id,
                SpecialistEmail = specialist.Email,
                ClientId = client.Id,
                ClientEmail = client.Email,
                CompanyName = company.LegalName,
                Info = $"Специалист {specialist.Email} отклонил отправку документов клиента {client.Email}."
            };
            _context.SalaryRejectingBySpecialistActions.Add(salaryRejectingingBySpecialistAction);
            await _context.SaveChangesAsync();
            return RedirectToAction("SalaryApprovings", "Specialist");
        }
    }
}
