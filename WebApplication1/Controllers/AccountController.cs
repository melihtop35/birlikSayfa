using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using sayfaASP.Models;
using WebApplication1.Filter;
using WebApplication1.Models;
using static SkiaSharp.HarfBuzz.SKShaper;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext _context;
        public AccountController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("Id").HasValue)
            {
                return Redirect("Home");
            }
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("Index");
        }
        public IActionResult Login(string No, string email)
        {
            var user = _context.Users.FirstOrDefault(w => w.taxNo.Equals(No) && w.Email.Equals(email));
            var userTcNo = _context.Users.FirstOrDefault(w => w.tcNo.Equals(No) && w.Email.Equals(email));
            if (user != null)
            {
                HttpContext.Session.SetInt32("Id", user.Id);
                HttpContext.Session.SetString("VergiNo", user.taxNo);
                return Redirect("/Home");
            }
            else if (userTcNo != null)
            {
                HttpContext.Session.SetInt32("Id", userTcNo.Id);
                HttpContext.Session.SetString("TcNo", userTcNo.tcNo);
                return Redirect("/Home");
            }
            else
            {
                return Redirect("Index");
            }
        }
    }
}
