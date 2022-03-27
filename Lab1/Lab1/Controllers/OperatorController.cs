using Lab1.Models.Data;
using Lab1.Models.Entities;
using Lab1.Models.OperatorModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab1.Controllers
{
    public class OperatorController : Controller
    {
        private ApplicationDbContext _context;

        public OperatorController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "operator")]
        public async Task<IActionResult> Profile()
        {
            //var _operator = await _context.Operators.FirstOrDefaultAsync(m => m.Id == User.Identity.Name);
            var _operator = await _context.Users.FirstOrDefaultAsync(m => m.Id == User.Identity.Name);
            var bank = await _context.Banks.FirstOrDefaultAsync(b => b.Id == _operator.BankId);
            var model = new OperatorProfileModel
            {
                Email = _operator.Email,
                FirstName = _operator.FirstName,
                LastName = _operator.LastName,
                Patronymic = _operator.Patronymic,
                PhoneNumber = _operator.PhoneNumber,
                BankName = bank.Name
            };
            return View(model);
        }

        [Authorize(Roles = "operator")]
        public async Task<IActionResult> SignUp()
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == User.Identity.Name);
            var _operator = new Operator
            {
                Id = user.Id,
                Email = user.Email,
                Password = user.Password,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Patronymic = user.Patronymic,
                PhoneNumber = user.PhoneNumber,
                RoleName = user.RoleName,
                BankId = user.BankId
            };
            //_context.Users.Remove(user);
            //_context.Operators.Add(_operator);
            await _context.SaveChangesAsync();
            return RedirectToAction("Profile", "Operator");
        }

        [Authorize(Roles = "operator, manager")]
        public async Task<IActionResult> SalaryApprovings()
        {
            //var _operator = await _context.Operators.FirstOrDefaultAsync(m => m.Id == User.Identity.Name);
            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == User.Identity.Name);
            var salaryApprovings = _context.SalaryApprovings;
            var clients = new List<Client>();
            foreach (var approving in salaryApprovings)
            {
                var client = await _context.Clients
                    .Include(c => c.Salary)
                    .FirstOrDefaultAsync(c => c.Id == approving.ClientId && c.BankId ==
                        user.BankId);
                if (client != null)
                {
                    if (client.Salary.ApprovedBySpecialist)
                    {
                        clients.Add(client);
                    }
                }
            }
            var model = new SalaryApprovingsOperatorModel
            {
                Clients = clients
            };
            return View(model);
        }

        [Authorize(Roles = "operator, manager")]
        public async Task<IActionResult> ApproveSalary(string clientId)
        {
            var client = await _context.Clients
                .Include(c => c.Salary)
                .FirstOrDefaultAsync(c => c.Id == clientId);
            client.Salary.ApprovedByOperator = true;
            _context.Clients.Update(client);
            var salaryApproving = await _context.SalaryApprovings.FirstOrDefaultAsync(s => s.ClientId == clientId);
            _context.SalaryApprovings.Remove(salaryApproving);
            await _context.SaveChangesAsync();
            return RedirectToAction("SalaryApprovings", "Operator");
        }

        [Authorize(Roles = "operator, manager")]
        public async Task<IActionResult> RejectSalary(string clientId)
        {
            var client = await _context.Clients
                .Include(c => c.Salary)
                .FirstOrDefaultAsync(c => c.Id == clientId);
            client.Salary = null;
            _context.Clients.Update(client);
            var salaryApproving = await _context.SalaryApprovings.FirstOrDefaultAsync(s => s.ClientId == clientId);
            _context.SalaryApprovings.Remove(salaryApproving);
            await _context.SaveChangesAsync();
            return RedirectToAction("SalaryApprovings", "Operator");
        }
    }
}
