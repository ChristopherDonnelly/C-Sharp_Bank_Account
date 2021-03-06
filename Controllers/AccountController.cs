using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bank_Accounts.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Bank_Accounts.Controllers
{
    public class AccountController : Controller
    {
        private BankAccountsContext _context;

        public AccountController(BankAccountsContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("Account")]
        public IActionResult Account()
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            string UserName = HttpContext.Session.GetString("UserName");
            
            ViewData["Username"] = UserName;

            if(UserId!=null){

                ViewData["UserId"] = (int)UserId;

                User user = _context.User.Include(u => u.Transactions).SingleOrDefault(u => u.UserId == UserId);

                user.Transactions = user.Transactions.OrderByDescending(t => t.CreatedAt).ToList();

                AccountBundle accountBundle = new AccountBundle{ 
                    user = user, 
                    transaction = new Transaction{
                        UserId = user.UserId,
                        User = user,
                        Amount = 0
                    }
                };

                return View(accountBundle);
            }else{
                return RedirectToAction("Login", "User");
            }
        }

        [HttpPost]
        [Route("Transact")]
        public IActionResult Transact(Transaction transaction)
        {
            
            User user = _context.User.Include(u => u.Transactions).SingleOrDefault(u => u.UserId == transaction.UserId);
            user.Transactions = user.Transactions.OrderByDescending(t => t.CreatedAt).ToList();

            user.Balance += transaction.Amount;
            
            TryValidateModel(user);

            if(ModelState.IsValid)
            {
                _context.Transaction.Add(transaction);
                _context.SaveChanges();

                return RedirectToAction("Account");
            }

            TempData["error"] = "You do not have enough funds to make the Withdrawal!";

            return RedirectToAction("Account");
        }

    }
}
