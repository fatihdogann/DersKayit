using Microsoft.AspNetCore.Mvc;

namespace DersKayit.Controllers
{
    public class HomeController : Controller
    {
        // Ana sayfa
        public IActionResult Index()
        {
            ViewBag.Title = "Hoş Geldiniz!";
            ViewBag.Subtitle = "Modern ve Şık ASP.NET Core MVC Uygulaması";
            return View();
        }

        // Hakkında sayfası
        public IActionResult About()
        {
            ViewBag.Title = "Hakkımızda";
            ViewBag.Description = "Bu uygulama ASP.NET Core MVC ile sıfırdan kodlanmıştır. Amacımız sade ve etkili bir yapıyla eğitim sunmaktır.";
            return View();
        }

        // İletişim sayfası
        public IActionResult Contact()
        {
            ViewBag.Title = "İletişim";
            return View();
        }
    }
}
