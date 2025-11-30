using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversitySystem.Data;
using UniversitySystem.Models;
using System.Linq;
using System.Threading.Tasks;

namespace UniversitySystem.Controllers
{
    public class StudentsController : Controller
    {
        private readonly UniversityContext _context;

        public StudentsController(UniversityContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? groupId, int? departamentId, string searchString)
        {
            var students = _context.Students
                .Include(s => s.Group)
                .ThenInclude(g => g.Departament)
                .AsQueryable();

            if (groupId.HasValue)
            {
                students = students.Where(s => s.IdGroup == groupId);
            }

            if (departamentId.HasValue)
            {
                students = students.Where(s => s.Group.IdDepartament == departamentId);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                string searchLower = searchString.ToLower();
                students = students.Where(s =>
                    (s.SecondName != null && s.SecondName.ToLower().Contains(searchLower)) ||
                    (s.Name != null && s.Name.ToLower().Contains(searchLower)) ||
                    (s.Patronymic != null && s.Patronymic.ToLower().Contains(searchLower)) ||
                    (s.SecondName + " " + s.Name + " " + s.Patronymic).ToLower().Contains(searchLower));
            }

            ViewBag.Groups = await _context.StudentGroups.ToListAsync();
            ViewBag.Departaments = await _context.Departaments.ToListAsync();
            ViewBag.SelectedGroupId = groupId;
            ViewBag.SelectedDepartamentId = departamentId;
            ViewBag.SearchString = searchString;

            var result = await students.ToListAsync();
            return View(result);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Group)
                .ThenInclude(g => g.Departament)
                .FirstOrDefaultAsync(m => m.IdStudent == id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        public async Task<IActionResult> Search()
        {
            ViewBag.Groups = await _context.StudentGroups.ToListAsync();
            ViewBag.Departaments = await _context.Departaments.ToListAsync();
            return View();
        }

        [HttpPost]
        public IActionResult Search(int? groupId, int? departamentId, string searchString)
        {
            return RedirectToAction("Index", new { groupId, departamentId, searchString });
        }

        [HttpGet]
        public async Task<IActionResult> GetGroups()
        {
            var groups = await _context.StudentGroups
                .Include(g => g.Departament)
                .Select(g => new {
                    g.IdGroup,
                    g.NumberGroup,
                    DepartamentName = g.Departament.Name
                })
                .OrderBy(g => g.NumberGroup)
                .ToListAsync();
            return Json(groups);
        }

        public async Task<IActionResult> Debug()
        {
            var students = await _context.Students
                .Include(s => s.Group)
                .ThenInclude(g => g.Departament)
                .ToListAsync();

            ViewBag.StudentsCount = students.Count;
            ViewBag.GroupsCount = await _context.StudentGroups.CountAsync();
            ViewBag.DepartamentsCount = await _context.Departaments.CountAsync();

            return View(students);
        }
    }
}