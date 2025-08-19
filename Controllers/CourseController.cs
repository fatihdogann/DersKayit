using Microsoft.AspNetCore.Mvc;
using DersKayit.Models;
using DersKayit.Data;
using System.Linq;

namespace DersKayit.Controllers
{
    public class CourseController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CourseController(ApplicationDbContext context) => _context = context;

        //LİSTE 
        public IActionResult List(string? category)
        {
            var courses = string.IsNullOrEmpty(category)
                ? _context.Courses.ToList()
                : _context.Courses.Where(c => c.Category == category).ToList();

            ViewBag.Categories = _context.Courses
                .Select(c => c.Category)
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct()
                .ToList();
            ViewBag.SelectedCategory = category;
            return View(courses);
        }

        /* ---------- BAŞVURU GET ---------- */
        [HttpGet("Course/Apply/{id:int}")]
        public IActionResult Apply(int id)
        {
            if (HttpContext.Session.GetString("Role") != "User")
                return RedirectToAction("Login", "Account");

            ViewBag.CourseId = id;
            return View();
        }

        /* ---------- BAŞVURU POST ---------- */
        [HttpPost("Course/Apply/{id:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult Apply(int id, CourseApplication model)
        {
            if (HttpContext.Session.GetString("Role") != "User")
                return RedirectToAction("Login", "Account");

            // URL'deki id, model’e gelmediyse doldur
            if (model.CourseId == 0) model.CourseId = id;

            if (ModelState.IsValid)
            {
                _context.CourseApplications.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Thanks");
            }

            ViewBag.CourseId = id;
            return View(model);
        }

        /* ---------- TEŞEKKÜR ---------- */
        public IActionResult Thanks()
        {
            if (HttpContext.Session.GetString("Role") != "User")
                return RedirectToAction("Login", "Account");

            return View();
        }

        // DETAY 
        public IActionResult Details(int id)
        {
            if (HttpContext.Session.GetString("Role") != "User")
                return RedirectToAction("Login", "Account");

            var course = _context.Courses.FirstOrDefault(c => c.Id == id);
            return course == null ? NotFound() : View(course);
        }

        // SİL (Admin) 
        // Sadece Admin'in silme işlemi yapabilmesi için POST işlemi
        [HttpGet("Course/Delete/{id:int}")]
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Login", "Account");

            var course = _context.Courses.FirstOrDefault(c => c.Id == id);
            if (course == null) return NotFound();

            return View(course);
        }

        [HttpPost("Course/Delete/{id:int}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Login", "Account");

            var course = _context.Courses.FirstOrDefault(c => c.Id == id);
            if (course == null) return NotFound();

            _context.Courses.Remove(course);
            _context.SaveChanges();
            return RedirectToAction("List");
        }
    }
}
