using Lab1.Models.UserModels;
using Lab1.Models.Data;
using Lab1.Models.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Lab1.Controllers
{
    public class UserController : Controller
    {
        private ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult LogIn(string? id)
        {
            ViewBag.Id = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogIn(LogInUserModel model)
        {
            ViewBag.Id = model.BankId;
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email &&
                    u.Password == model.Password &&
                    u.BankId == model.BankId).Result;
                if (user != null)
                {
                    await Authenticate(user);
                    return RedirectToAction("Profile", "User");
                    /*switch (user.RoleName)
                    {
                        case "admin":
                            return RedirectToAction("Index", "Home");
                        case "client":
                            return RedirectToAction("Profile", "Client");
                        case "manager":
                            return RedirectToAction("Profile", "Manager");
                        case "operator":
                            return RedirectToAction("Profile", "Operator");
                        case "specialist":
                            return RedirectToAction("Profile", "Specialist");
                    }*/
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult SignUp(string? id)
        {
            ViewBag.Id = id;
            ViewBag.Roles = _context.Roles;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpUserModel model)
        {
            ViewBag.Id = model.BankId;
            ViewBag.Roles = _context.Roles;
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email &&
                    u.BankId == model.BankId);

                if (user == null)
                {
                    user = new User { 
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Patronymic = model.Patronymic,
                        PhoneNumber = model.PhoneNumber,
                        Email = model.Email,
                        Password = model.Password,
                        RoleName = model.RoleName,
                        BankId = model.BankId,
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    await Authenticate(user);

                    switch (user.RoleName)
                    {
                        case "admin":
                            return RedirectToAction("SignUp", "Admin");
                        case "client":
                            return RedirectToAction("SignUp", "Client");
                        case "manager":
                            return RedirectToAction("SignUp", "Manager");
                        case "operator":
                            return RedirectToAction("SignUp", "Operator");
                        case "specialist":
                            return RedirectToAction("SignUp", "Specialist");
                    }
                }
                else
                    ModelState.AddModelError("", "На указанный электронный адрес уже зарегистрирован пользователь");
            }
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == User.Identity.Name);
            switch (user.RoleName)
            {
                case "admin":
                    return RedirectToAction("Profile", "Admin");
                case "client":
                    var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == User.Identity.Name);
                    if(client == null)
                    {
                        return RedirectToAction("SignUp", "Client");
                    }
                    return RedirectToAction("Profile", "Client");
                case "manager":
                    return RedirectToAction("Profile", "Manager");
                case "operator":
                    return RedirectToAction("Profile", "Operator");
                case "specialist":
                    var specialist = await _context.Specialists.FirstOrDefaultAsync(c => c.Id == User.Identity.Name);
                    if (specialist == null)
                    {
                        return RedirectToAction("SignUp", "Specialist");
                    }
                    return RedirectToAction("Profile", "Specialist");
            }
            return View();
        }

        private async Task Authenticate(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Id),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.RoleName)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            /*if (User.Identity.IsAuthenticated)
            {
                User.AddIdentity(id);
                var identity = User.Identities.FirstOrDefault(i => i.Name == user.Id);
                ClaimsPrincipal.PrimaryIdentitySelector = (ids) => identity;
                await _context.SaveChangesAsync();
            }
            else*/
            //{
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
            //}
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("List", "Bank");
        }
    }
}
