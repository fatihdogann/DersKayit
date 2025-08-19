using Microsoft.AspNetCore.Mvc;
using DersKayit.Data;
using DersKayit.Models;
using System.Linq;
using System.Text;
using System.Text.Unicode;
using System.Text.Encodings.Web;

namespace DersKayit.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Role") == "Admin";
        }

        public IActionResult Applications()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var applications = _context.CourseApplications.ToList();
            return View(applications);
        }

        public IActionResult ExportApplications()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var applications = _context.CourseApplications.ToList();
            var sb = new StringBuilder();
            sb.AppendLine("Ad Soyad,Email,Kurs ID");

            foreach (var app in applications)
            {
                var line = $"{app.Name},{app.Email},{app.CourseId}";
                sb.AppendLine(line);
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", "Basvurular.csv");
        }

        public IActionResult Courses()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var courses = _context.Courses
                .Select(course => new CourseWithCountViewModel
                {
                    Id = course.Id,
                    Title = course.Title,
                    Description = course.Description,
                    Category = course.Category,
                    ApplicationCount = _context.CourseApplications.Count(a => a.CourseId == course.Id)
                })
                .ToList();

            return View(courses);
        }

        public IActionResult Create()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            ViewBag.Categories = GetCategoryList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Course model)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                _context.Courses.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Courses");
            }

            ViewBag.Categories = GetCategoryList();
            return View(model);
        }

        public IActionResult Edit(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var course = _context.Courses.Find(id);
            if (course == null) return NotFound();

            ViewBag.Categories = GetCategoryList();
            return View(course);
        }

        [HttpPost]
        public IActionResult Edit(Course model)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                _context.Courses.Update(model);
                _context.SaveChanges();
                return RedirectToAction("Courses");
            }

            ViewBag.Categories = GetCategoryList();
            return View(model);
        }

        // Güncellenen Delete (GET)
        public IActionResult Delete(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var course = _context.Courses.Find(id);
            if (course == null) return NotFound();

            return View(course);
        }

        // Güncellenen DeleteConfirmed (POST)
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var course = _context.Courses.Find(id);
            if (course == null) return NotFound();

            _context.Courses.Remove(course);
            _context.SaveChanges();
            return RedirectToAction("Courses");
        }

        private List<string> GetCategoryList()
        {
            return _context.Courses
                .Where(c => !string.IsNullOrWhiteSpace(c.Category))
                .Select(c => c.Category!)
                .Distinct()
                .OrderBy(c => c)
                .ToList();
        }
    }

    public class CourseWithCountViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Category { get; set; }
        public int ApplicationCount { get; set; }
    }
}
