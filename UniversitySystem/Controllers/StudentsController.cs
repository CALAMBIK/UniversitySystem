using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversitySystem.Data;
using UniversitySystem.Models;
using UniversitySystem.Services;
using System.Linq;
using System.Threading.Tasks;

namespace UniversitySystem.Controllers
{
    public class StudentsController : Controller
    {
        private readonly UniversityContext _context;
        private readonly AuthService _authService;

        public StudentsController(UniversityContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<IActionResult> Index(int? groupId, int? departamentId, string searchString)
        {
            // Проверяем авторизацию (только для администраторов)
            if (!_authService.IsAuthenticated() || _authService.GetUserRole() != "Admin")
                return RedirectToAction("AccessDenied", "Account");

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

            // Получаем учебные материалы студента
            if (student.IdGroup.HasValue)
            {
                ViewBag.CourseMaterials = await _context.CourseMaterials
                    .Include(cm => cm.Teacher)
                    .Include(cm => cm.Discipline)
                    .Where(cm => cm.IdGroup == student.IdGroup)
                    .OrderByDescending(cm => cm.CreatedDate)
                    .Take(10)
                    .ToListAsync();
            }

            return View(student);
        }

        public async Task<IActionResult> Search()
        {
            if (!_authService.IsAuthenticated() || _authService.GetUserRole() != "Admin")
                return RedirectToAction("AccessDenied", "Account");

            ViewBag.Groups = await _context.StudentGroups.ToListAsync();
            ViewBag.Departaments = await _context.Departaments.ToListAsync();
            return View();
        }

        [HttpPost]
        public IActionResult Search(int? groupId, int? departamentId, string searchString)
        {
            if (!_authService.IsAuthenticated() || _authService.GetUserRole() != "Admin")
                return RedirectToAction("AccessDenied", "Account");

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
            if (!_authService.IsAuthenticated() || _authService.GetUserRole() != "Admin")
                return RedirectToAction("AccessDenied", "Account");

            var students = await _context.Students
                .Include(s => s.Group)
                .ThenInclude(g => g.Departament)
                .ToListAsync();

            ViewBag.StudentsCount = students.Count;
            ViewBag.GroupsCount = await _context.StudentGroups.CountAsync();
            ViewBag.DepartamentsCount = await _context.Departaments.CountAsync();

            return View(students);
        }

        // Удаление студента (только для администратора)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!_authService.IsAuthenticated() || _authService.GetUserRole() != "Admin")
                return RedirectToAction("AccessDenied", "Account");

            var student = await _context.Students
                .Include(s => s.Group)
                .FirstOrDefaultAsync(s => s.IdStudent == id);

            if (student == null)
            {
                TempData["ErrorMessage"] = "Студент не найден!";
                return RedirectToAction("Index");
            }

            try
            {
                // Находим связанного пользователя
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.IdStudent == id);

                if (user != null)
                {
                    // Удаляем профиль пользователя
                    var profile = await _context.UserProfiles
                        .FirstOrDefaultAsync(p => p.IdUser == user.IdUser);

                    if (profile != null)
                    {
                        _context.UserProfiles.Remove(profile);
                    }

                    // Удаляем пользователя
                    _context.Users.Remove(user);
                }

                // Удаляем студента
                _context.Students.Remove(student);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Студент {student.SecondName} {student.Name} успешно удален!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ошибка при удалении: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        // Редактирование студента (только для администратора)
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!_authService.IsAuthenticated() || _authService.GetUserRole() != "Admin")
                return RedirectToAction("AccessDenied", "Account");

            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Group)
                .FirstOrDefaultAsync(s => s.IdStudent == id);

            if (student == null)
            {
                return NotFound();
            }

            ViewBag.Groups = await _context.StudentGroups
                .Include(g => g.Departament)
                .ToListAsync();
            ViewBag.Departaments = await _context.Departaments.ToListAsync();

            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Student student)
        {
            if (!_authService.IsAuthenticated() || _authService.GetUserRole() != "Admin")
                return RedirectToAction("AccessDenied", "Account");

            if (id != student.IdStudent)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingStudent = await _context.Students.FindAsync(id);
                    if (existingStudent == null)
                    {
                        return NotFound();
                    }

                    // Обновляем данные студента
                    existingStudent.Name = student.Name;
                    existingStudent.SecondName = student.SecondName;
                    existingStudent.Patronymic = student.Patronymic;
                    existingStudent.PhoneNumber = student.PhoneNumber;
                    existingStudent.DateBirthday = student.DateBirthday;
                    existingStudent.IdGroup = student.IdGroup;

                    _context.Students.Update(existingStudent);

                    // Обновляем связанного пользователя
                    var user = await _context.Users
                        .FirstOrDefaultAsync(u => u.IdStudent == id);

                    if (user != null)
                    {
                        // Обновляем пароль если изменился
                        if (!string.IsNullOrEmpty(student.Password) && student.Password != existingStudent.Password)
                        {
                            existingStudent.Password = student.Password;
                            user.Password = student.Password;
                            _context.Users.Update(user);
                        }
                    }

                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Данные студента успешно обновлены!";
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewBag.Groups = await _context.StudentGroups
                .Include(g => g.Departament)
                .ToListAsync();
            ViewBag.Departaments = await _context.Departaments.ToListAsync();

            return View(student);
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.IdStudent == id);
        }
    }
}