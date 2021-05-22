using Kanban.Data;
using Kanban.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Kanban.Controllers
{
    public class AuthController : Controller
    {
        private readonly SqlContext _context;
        public static User LoggedUser { get; set; } = null;

        public AuthController(SqlContext context)
        {
            _context = context;
        }

        [BindProperties]
        public class InputModel
        {
            [Required]
            public string Username { get; set; }
            [Required]
            public string Firstname { get; set; }
            [Required, DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name ="Confirm password")]
            [Required, Compare("Password"), DataType(DataType.Password)]
            public string ConfirmPassword { get; set; }
        }

        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Logout()
        {
            LoggedUser = null;
            return RedirectToAction("Index","Home");
        }


        [HttpPost]
        public IActionResult Login(InputModel model)
        {
            var UserToLogin = _context.User.Include(t => t.Tasks).SingleOrDefault(user => user.Username == model.Username && user.Password == model.Password);
            if (UserToLogin == null)
            {
                ModelState.AddModelError("Username", "Username and password doesn't match");
                return View();
            }
            LoggedUser = new() { Id = UserToLogin.Id, Username = UserToLogin.Username, Firstname= UserToLogin.Firstname, Tasks = UserToLogin.Tasks };
            return RedirectToAction("Index", "Kanban");
            
        }
        [HttpPost]
        public async Task<IActionResult>  Register(InputModel model)
        {
            Console.WriteLine(_context.User);
            Console.WriteLine(model);

            if (ModelState.IsValid)
            {
                if (_context.User.SingleOrDefault(user => user.Username == model.Username) != null)
                {
                    ModelState.AddModelError("Username", "User with this username already exists");
                    return View();
                }
                User UserToDb = new() { Username = model.Username, Firstname = model.Firstname, Password = model.Password };
                _context.User.Add(UserToDb);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");

            }
            return View();
  
        }


        public static bool CheckAuth()
        {
            if (LoggedUser != null)
                return true;
            return false;
        }

    }
}
