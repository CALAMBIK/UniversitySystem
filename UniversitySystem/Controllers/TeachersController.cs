using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversitySystem.Models;
using System.Linq;
using System.Threading.Tasks;

namespace UniversitySystem.Controllers
{
    public class TeachersController : Controller
    {
        private readonly UniversityContext _context;

        public TeachersController(UniversityContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? departamentId, string searchString)
        {
            var teachers = _context.Teachers
                .Include(t => t.Departament)
                .AsQueryable();

            if (departamentId.HasValue)
            {
                teachers = teachers.Where(t => t.IdDepartament == departamentId);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                string searchLower = searchString.ToLower();
                teachers = teachers.Where(t =>
                    (t.SecondName != null && t.SecondName.ToLower().Contains(searchLower)) ||
                    (t.Name != null && t.Name.ToLower().Contains(searchLower)) ||
                    (t.Patronymic != null && t.Patronymic.ToLower().Contains(searchLower)) ||
                    (t.SecondName + " " + t.Name + " " + t.Patronymic).ToLower().Contains(searchLower));
            }

            ViewBag.Departaments = await _context.Departaments.ToListAsync();
            ViewBag.SelectedDepartamentId = departamentId;
            ViewBag.SearchString = searchString;

            var result = await teachers.ToListAsync();
            return View(result);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                .Include(t => t.Departament)
                .FirstOrDefaultAsync(m => m.IdTeacher == id);

            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        public async Task<IActionResult> Search()
        {
            ViewBag.Departaments = await _context.Departaments.ToListAsync();
            return View();
        }

        [HttpPost]
        public IActionResult Search(int? departamentId, string searchString)
        {
            return RedirectToAction("Index", new { departamentId, searchString });
        }
    }
}