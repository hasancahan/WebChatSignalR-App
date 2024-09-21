using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebChatSignalR.DataAccess;
using WebChatSignalR.Models;

namespace WebChatSignalR.Controllers
{
    public class ChatController : Controller
    {
        private readonly AppDbContext _context;
        public ChatController(AppDbContext context) {  _context = context; }


        public IActionResult Index()
        {
            var user = GetSessionData();

            ViewBag.Username = user.Username;
            ViewBag.FullName = user.FullName;
            ViewBag.UserId = user.UserId;
            ViewBag.Email = user.Email;

            return View();
        }

        public IActionResult VoiceChat()
        {
            var user = GetSessionData();

            ViewBag.Username = user.Username;
            ViewBag.FullName = user.FullName;
            ViewBag.UserId = user.UserId;
            ViewBag.Email = user.Email;
            return View();
        }

        public IActionResult VideoChat()
        {
            var user = GetSessionData();

            ViewBag.Username = user.Username;
            ViewBag.FullName = user.FullName;
            ViewBag.UserId = user.UserId;
            ViewBag.Email = user.Email;
            return View();
        }

        public IActionResult GroupChat()
        {
            var user = GetSessionData();

            ViewBag.Username = user.Username;
            ViewBag.FullName = user.FullName;
            ViewBag.UserId = user.UserId;
            ViewBag.Email = user.Email;
            return View();
        }

        public IActionResult UsersBook()
        {
            var user = GetSessionData();

            ViewBag.Username = user.Username;
            ViewBag.FullName = user.FullName;
            ViewBag.UserId = user.UserId;
            ViewBag.Email = user.Email;


            var users = _context.AppUsers.ToList();
            return View(users);
        }

        public (string FullName, string Username, string UserId, string Email) GetSessionData()
        {
            var fullname = HttpContext.Session.GetString("FullName");
            var username = HttpContext.Session.GetString("Username");
            var userId = HttpContext.Session.GetString("UserId");
            var email = HttpContext.Session.GetString("Email");


            return (fullname, username, userId, email);
        }
















        //Account'a tasınacak...
        public IActionResult Settings()
        {
            var username = HttpContext.Session.GetString("Username");

            if (username != null)
            {
                ViewBag.Username = username;
            }
            return View();
        }
    }
}
