using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebChatSignalR.DataAccess;
using WebChatSignalR.Models;

namespace WebChatSignalR.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public AccountController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(AppUsers users)
        {
            try
            {
                var user = _appDbContext.AppUsers.SingleOrDefault(a => a.Username == users.Username && a.Password == users.Password);
                if (user != null)
                {
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetString("Email", user.Email);
                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                    HttpContext.Session.SetString("FullName", user.FullName);

                    return RedirectToAction("Index", "Chat");
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
            }


            return View(users);
        }







        [HttpGet]
        public IActionResult Register() 
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(AppUsers appUsers)
        {
            if (appUsers != null) 
            {
                var users = _appDbContext.AppUsers.SingleOrDefault(m => m.Username == appUsers.Username);
                if (users != null) 
                {
                    return View(appUsers);
                }

                var newuser = new AppUsers
                {
                    FullName = appUsers.FullName,
                    Username = appUsers.Username,
                    Password = appUsers.Password,
                    Email = appUsers.Email,
                    FirstRegisterDate = appUsers.FirstRegisterDate
                };

                _appDbContext.AppUsers.Add(newuser);
                await _appDbContext.SaveChangesAsync();

                return RedirectToAction("Login");
            }
            return View(appUsers);
        }





        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Username");
            return RedirectToAction("Login", "Account");
        }




        public IActionResult Profile()
        {
            var username = HttpContext.Session.GetString("Username");
            var email = HttpContext.Session.GetString("Email");

            if (username != null)
            {
                // Profil sayfasına kullanıcı bilgilerini gönder
                ViewBag.Username = username;
                ViewBag.Email = email;
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }
    }
}
