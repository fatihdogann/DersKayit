using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using DersKayit.Data;
using DersKayit.Models;

namespace DersKayit.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Login ekranı
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("Username") != null)
            {
                // Kullanıcı zaten giriş yaptıysa role göre yönlendir
                var role = HttpContext.Session.GetString("Role");
                if (role == "Admin")
                    return RedirectToAction("Courses", "Admin");
                return RedirectToAction("List", "Course");
            }

            return View();
        }

        // POST: Giriş işlemi
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                if (!string.IsNullOrEmpty(user.Username))
                    HttpContext.Session.SetString("Username", user.Username);

                if (!string.IsNullOrEmpty(user.Role))
                    HttpContext.Session.SetString("Role", user.Role);

                if (user.Role == "Admin")
                    return RedirectToAction("Courses", "Admin");

                return RedirectToAction("List", "Course");
            }

            ViewBag.Message = "Hatalı kullanıcı adı veya şifre.";
            return View();
        }

        // Kullanıcı çıkışı
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // GET: Kayıt ekranı
        [HttpGet]
        public IActionResult Register()
        {
            if (HttpContext.Session.GetString("Username") != null)
            {
                // Giriş yaptıysa kayıt ekranına girmesin
                var role = HttpContext.Session.GetString("Role");
                if (role == "Admin")
                    return RedirectToAction("Courses", "Admin");
                return RedirectToAction("List", "Course");
            }

            return View();
        }

        // POST: Kayıt işlemi
        [HttpPost]
        public IActionResult Register(string username, string password)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Username == username);
            if (existingUser != null)
            {
                ViewBag.Message = "Bu kullanıcı adı zaten alınmış.";
                return View();
            }

            var newUser = new User
            {
                Username = username,
                Password = password,
                Role = "User"
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            ViewBag.Message = "Kayıt başarılı. Giriş yapabilirsiniz.";
            return RedirectToAction("Login");
        }
    }
}
