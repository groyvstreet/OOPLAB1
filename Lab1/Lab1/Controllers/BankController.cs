﻿using Lab1.Models;
using Lab1.Models.BankModels;
using Lab1.Models.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Lab1.Controllers
{
    public class BankController : Controller
    {
        private ApplicationDbContext _context;

        public BankController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Banks()
        {
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (User.Identity.IsAuthenticated)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == User.Identity.Name);
                return RedirectToAction("Profile", "Bank");
            }
            return View(_context.Banks.ToList());
        }

        public async Task<IActionResult> Profile(string? bankId)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == User.Identity.Name);
                bankId = user.BankId;
            }
            ViewBag.BankId = bankId;
            var bank = await _context.Banks.FirstOrDefaultAsync(b => b.Id == bankId);
            var model = new BankProfileModel
            {
                Name = bank.Name,
                Percent = bank.Percent,
                BIC = bank.BIC,
                Authenticated = User.Identity.IsAuthenticated
            };
            /*foreach (var identity in User.Identities)
            {
                if (identity.IsAuthenticated)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == identity.Name &&
                        u.BankId == bankId);
                    if (user != null)
                    {
                        ClaimsPrincipal.PrimaryIdentitySelector = (ids) => identity;
                        return RedirectToAction("Profile", "User");
                    }
                }
            }
            ClaimsPrincipal.PrimaryIdentitySelector = null;*/
            return View(model);
        }
    }
}