using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bank_Accounts.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Bank_Accounts.Controllers
{
    public class UserController : Controller
    {
        private BankAccountsContext _context;

        public UserController(BankAccountsContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("")]
        [Route("Login")]
        public IActionResult Login()
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            string UserName = HttpContext.Session.GetString("UserName");

            if(UserId!=null){
                return RedirectToAction("Account", "Account");
            }else{
                return View("Login");
            }
        }

        [HttpGet]
        [Route("Logoff")]
        public IActionResult Logoff()
        {
            HttpContext.Session.Clear();
            return View("Login");
        }

        [HttpGet]
        [Route("Register")]
        public IActionResult Register()
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            string UserName = HttpContext.Session.GetString("UserName");

            if(UserId!=null){
                return RedirectToAction("Account", "Account");
            }else{
                return View("Register");
            }
        }

        [HttpPost]
        [Route("Register")]
        public IActionResult Register(RegisterViewModel model)
        {

            if(ModelState.IsValid)
            {
                User user = _context.User.SingleOrDefault(u => u.Email == model.Email);

                model.Unique = (user==null)?0:1;

                TryValidateModel(model);

                if(!ModelState.IsValid){
                    return View("Register");
                }else{
                    User newUser = model.createUser();

                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    newUser.Password = Hasher.HashPassword(newUser, newUser.Password);

					_context.User.Add(newUser);
                	_context.SaveChanges();
				
                    HttpContext.Session.SetInt32("UserId", (int)newUser.UserId);
                    HttpContext.Session.SetString("UserName", (string)newUser.FirstName);

                    return RedirectToAction("Account", "Account");
                }
            }

            return View("Register");
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login(LoginViewModel model)
        {
			User user = _context.User.SingleOrDefault(u => u.Email == model.Email);
            
            model.Found = (user==null)?1:0;

            if(model.Found == 0){
                var Hasher = new PasswordHasher<User>();

                model.PasswordConfirmation = (Hasher.VerifyHashedPassword(user, user.Password, model.Password) != 0)?0:1;
            }

            TryValidateModel(model);

            if(ModelState.IsValid)
            {
                HttpContext.Session.SetInt32("UserId", (int)user.UserId);
                HttpContext.Session.SetString("UserName", (string)user.FirstName);
                return RedirectToAction("Account", "Account");
            }

            return View("Login");
        }

    }
}
