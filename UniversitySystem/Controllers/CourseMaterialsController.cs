using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversitySystem.Models;
using UniversitySystem.Data;
using UniversitySystem.Services;

namespace UniversitySystem.Controllers
{
    public class CourseMaterialsController : Controller
    {
        private readonly UniversityContext _context;
        private readonly AuthService _authService;

        public CourseMaterialsController(UniversityContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        // Просмотр материалов для студентов
        public async Task<IActionResult> Index(int? disciplineId)
        {
            if (!_authService.IsAuthenticated() || _authService.GetUserRole() != "Student")
                return RedirectToAction("AccessDenied", "Account");

            var userId = _authService.GetUserId();
            var user = await _context.Users
                .Include(u => u.Student)
                .FirstOrDefaultAsync(u => u.IdUser == userId);

            if (user?.Student == null || !user.Student.IdGroup.HasValue)
            {
                TempData["ErrorMessage"] = "Вы не прикреплены к группе!";
                return RedirectToAction("Profile", "Account");
            }

            var materialsQuery = _context.CourseMaterials
                .Include(m => m.Teacher)
                .Include(m => m.Discipline)
                .Where(m => m.IdGroup == user.Student.IdGroup.Value);

            if (disciplineId.HasValue)
            {
                materialsQuery = materialsQuery.Where(m => m.IdDiscipline == disciplineId.Value);
            }

            var materials = await materialsQuery
                .OrderByDescending(m => m.CreatedDate)
                .ToListAsync();

            ViewBag.Disciplines = await _context.CourseMaterials
                .Where(m => m.IdGroup == user.Student.IdGroup.Value)
                .Select(m => m.Discipline)
                .Distinct()
                .ToListAsync();

            ViewBag.SelectedDisciplineId = disciplineId;

            return View(materials);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!_authService.IsAuthenticated() || _authService.GetUserRole() != "Teacher")
                return RedirectToAction("AccessDenied", "Account");

            var userId = _authService.GetUserId();
            var user = await _context.Users
                .Include(u => u.Teacher)
                .FirstOrDefaultAsync(u => u.IdUser == userId);

            if (user?.Teacher == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            // Получаем дисциплины преподавателя с группами
            var teacherDisciplines = await _context.TeacherDisciplines
                .Include(td => td.Discipline)
                .Include(td => td.Group)
                .Where(td => td.IdTeacher == user.Teacher.IdTeacher && td.Group != null)
                .ToListAsync();

            if (!teacherDisciplines.Any())
            {
                TempData["ErrorMessage"] = "У вас нет назначенных дисциплин с группами!";
                return RedirectToAction("Profile", "Account");
            }

            // Получаем УНИКАЛЬНЫЕ группы из teacherDisciplines
            var groups = teacherDisciplines
                .Where(td => td.Group != null)
                .Select(td => td.Group)
                .DistinctBy(g => g.IdGroup) // Используем DistinctBy для .NET 6+
                .ToList();

            // Для .NET 6 и ниже можно использовать GroupBy:
            // var groups = teacherDisciplines
            //     .Where(td => td.Group != null)
            //     .Select(td => td.Group)
            //     .GroupBy(g => g.IdGroup)
            //     .Select(g => g.First())
            //     .ToList();

            ViewBag.TeacherDisciplines = teacherDisciplines;
            ViewBag.Groups = groups; // Теперь это не null

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseMaterial material)
        {
            if (!_authService.IsAuthenticated() || _authService.GetUserRole() != "Teacher")
                return RedirectToAction("AccessDenied", "Account");

            if (ModelState.IsValid)
            {
                var userId = _authService.GetUserId();
                var user = await _context.Users
                    .Include(u => u.Teacher)
                    .FirstOrDefaultAsync(u => u.IdUser == userId);

                if (user?.Teacher == null)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }

                // Проверяем, ведет ли преподаватель эту дисциплину для этой группы
                var isValidDiscipline = await _context.TeacherDisciplines
                    .AnyAsync(td => td.IdTeacher == user.Teacher.IdTeacher
                        && td.IdDiscipline == material.IdDiscipline
                        && (td.IdGroup == null || td.IdGroup == material.IdGroup));

                if (!isValidDiscipline)
                {
                    TempData["ErrorMessage"] = "Вы не ведете эту дисциплину для данной группы!";
                    return RedirectToAction("Create");
                }

                material.IdTeacher = user.Teacher.IdTeacher;
                material.CreatedDate = DateTime.Now;

                _context.CourseMaterials.Add(material);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Материал успешно создан!";
                return RedirectToAction("TeacherMaterials");
            }

            // Если ошибка валидации, возвращаем с данными
            var userIdForView = _authService.GetUserId();
            var userForView = await _context.Users
                .Include(u => u.Teacher)
                .FirstOrDefaultAsync(u => u.IdUser == userIdForView);

            if (userForView?.Teacher == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            // Загружаем данные заново для отображения в форме
            var teacherDisciplines = await _context.TeacherDisciplines
                .Include(td => td.Discipline)
                .Include(td => td.Group)
                .Where(td => td.IdTeacher == userForView.Teacher.IdTeacher && td.Group != null)
                .ToListAsync();

            var groups = teacherDisciplines
                .Where(td => td.Group != null)
                .Select(td => td.Group)
                .DistinctBy(g => g.IdGroup)
                .ToList();

            ViewBag.TeacherDisciplines = teacherDisciplines;
            ViewBag.Groups = groups;

            return View(material);
        }

        // Просмотр материалов преподавателя
        public async Task<IActionResult> TeacherMaterials()
        {
            if (!_authService.IsAuthenticated() || _authService.GetUserRole() != "Teacher")
                return RedirectToAction("AccessDenied", "Account");

            var userId = _authService.GetUserId();
            var user = await _context.Users
                .Include(u => u.Teacher)
                .FirstOrDefaultAsync(u => u.IdUser == userId);

            if (user?.Teacher == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var materials = await _context.CourseMaterials
                .Include(m => m.Group)
                .Include(m => m.Discipline)
                .Where(m => m.IdTeacher == user.Teacher.IdTeacher)
                .OrderByDescending(m => m.CreatedDate)
                .ToListAsync();

            return View(materials);
        }

        // Удаление материала (только для создавшего преподавателя или админа)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!_authService.IsAuthenticated())
                return RedirectToAction("AccessDenied", "Account");

            var material = await _context.CourseMaterials.FindAsync(id);
            if (material == null)
            {
                return NotFound();
            }

            var userRole = _authService.GetUserRole();

            if (userRole == "Admin")
            {
                // Админ может удалять любые материалы
                _context.CourseMaterials.Remove(material);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Материал успешно удален!";
                return RedirectToAction("AdminDashboard", "Home");
            }
            else if (userRole == "Teacher")
            {
                var userId = _authService.GetUserId();
                var user = await _context.Users
                    .Include(u => u.Teacher)
                    .FirstOrDefaultAsync(u => u.IdUser == userId);

                if (user?.Teacher != null && material.IdTeacher == user.Teacher.IdTeacher)
                {
                    // Преподаватель может удалять только свои материалы
                    _context.CourseMaterials.Remove(material);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Материал успешно удален!";
                    return RedirectToAction("TeacherMaterials");
                }
            }

            return RedirectToAction("AccessDenied", "Account");
        }

        public async Task<IActionResult> Details(int id)
        {
            if (!_authService.IsAuthenticated())
                return RedirectToAction("Login", "Account");

            var material = await _context.CourseMaterials
                .Include(m => m.Teacher)
                .ThenInclude(t => t.Departament)
                .Include(m => m.Group)
                .Include(m => m.Discipline)
                .FirstOrDefaultAsync(m => m.IdMaterial == id);

            if (material == null)
            {
                return NotFound();
            }

            // Проверяем доступ
            var userRole = _authService.GetUserRole();

            if (userRole == "Student")
            {
                var user = await _context.Users
                    .Include(u => u.Student)
                    .FirstOrDefaultAsync(u => u.IdUser == _authService.GetUserId());

                if (user?.Student == null || material.IdGroup != user.Student.IdGroup)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
            }
            else if (userRole == "Teacher")
            {
                var user = await _context.Users
                    .Include(u => u.Teacher)
                    .FirstOrDefaultAsync(u => u.IdUser == _authService.GetUserId());

                if (user?.Teacher == null || material.IdTeacher != user.Teacher.IdTeacher)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
            }

            return View(material);
        }
    }
}