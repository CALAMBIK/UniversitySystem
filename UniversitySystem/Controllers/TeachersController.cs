using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversitySystem.Models;
using UniversitySystem.Services;
using System.Linq;
using System.Threading.Tasks;
using UniversitySystem.Data;

namespace UniversitySystem.Controllers
{
    public class TeachersController : Controller
    {
        private readonly UniversityContext _context;
        private readonly AuthService _authService;

        public TeachersController(UniversityContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<IActionResult> Index(int? departamentId, string searchString)
        {
            // Проверяем авторизацию (только для администраторов)
            if (!_authService.IsAuthenticated() || _authService.GetUserRole() != "Admin")
                return RedirectToAction("AccessDenied", "Account");

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

            // Получаем дисциплины преподавателя
            ViewBag.TeacherDisciplines = await _context.TeacherDisciplines
                .Include(td => td.Discipline)
                .Include(td => td.Group)
                .Where(td => td.IdTeacher == id)
                .ToListAsync();

            // Получаем материалы преподавателя
            ViewBag.CourseMaterials = await _context.CourseMaterials
                .Include(cm => cm.Group)
                .Include(cm => cm.Discipline)
                .Where(cm => cm.IdTeacher == id)
                .OrderByDescending(cm => cm.CreatedDate)
                .Take(10)
                .ToListAsync();

            return View(teacher);
        }

        public async Task<IActionResult> Search()
        {
            if (!_authService.IsAuthenticated() || _authService.GetUserRole() != "Admin")
                return RedirectToAction("AccessDenied", "Account");

            ViewBag.Departaments = await _context.Departaments.ToListAsync();
            return View();
        }

        [HttpPost]
        public IActionResult Search(int? departamentId, string searchString)
        {
            if (!_authService.IsAuthenticated() || _authService.GetUserRole() != "Admin")
                return RedirectToAction("AccessDenied", "Account");

            return RedirectToAction("Index", new { departamentId, searchString });
        }

        [HttpGet]
        public async Task<IActionResult> GetDepartaments()
        {
            var departaments = await _context.Departaments
                .Select(d => new {
                    d.IdDepartament,
                    d.Name
                })
                .OrderBy(d => d.Name)
                .ToListAsync();
            return Json(departaments);
        }

        // Удаление преподавателя (только для администратора)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!_authService.IsAuthenticated() || _authService.GetUserRole() != "Admin")
                return RedirectToAction("AccessDenied", "Account");

            var teacher = await _context.Teachers
                .Include(t => t.Departament)
                .FirstOrDefaultAsync(t => t.IdTeacher == id);

            if (teacher == null)
            {
                TempData["ErrorMessage"] = "Преподаватель не найден!";
                return RedirectToAction("Index");
            }

            try
            {
                // Находим связанного пользователя
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.IdTeacher == id);

                if (user != null)
                {
                    // Удаляем профиль пользователя
                    var profile = await _context.UserProfiles
                        .FirstOrDefaultAsync(p => p.IdUser == user.IdUser);

                    if (profile != null)
                    {
                        _context.UserProfiles.Remove(profile);
                    }

                    // Удаляем учебные материалы преподавателя
                    var materials = await _context.CourseMaterials
                        .Where(m => m.IdTeacher == id)
                        .ToListAsync();

                    if (materials.Any())
                    {
                        _context.CourseMaterials.RemoveRange(materials);
                    }

                    // Удаляем связи преподавателя с дисциплинами
                    var teacherDisciplines = await _context.TeacherDisciplines
                        .Where(td => td.IdTeacher == id)
                        .ToListAsync();

                    if (teacherDisciplines.Any())
                    {
                        _context.TeacherDisciplines.RemoveRange(teacherDisciplines);
                    }

                    // Удаляем пользователя
                    _context.Users.Remove(user);
                }

                // Удаляем преподавателя
                _context.Teachers.Remove(teacher);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Преподаватель {teacher.SecondName} {teacher.Name} успешно удален!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ошибка при удалении: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        // Редактирование преподавателя (только для администратора)
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!_authService.IsAuthenticated() || _authService.GetUserRole() != "Admin")
                return RedirectToAction("AccessDenied", "Account");

            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                .Include(t => t.Departament)
                .FirstOrDefaultAsync(t => t.IdTeacher == id);

            if (teacher == null)
            {
                return NotFound();
            }

            ViewBag.Departaments = await _context.Departaments.ToListAsync();
            ViewBag.Disciplines = await _context.Disciplines.ToListAsync();

            // Получаем текущие дисциплины преподавателя
            ViewBag.SelectedDisciplines = await _context.TeacherDisciplines
                .Where(td => td.IdTeacher == id)
                .Select(td => td.IdDiscipline)
                .ToListAsync();

            return View(teacher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Teacher teacher, List<int> disciplineIds)
        {
            if (!_authService.IsAuthenticated() || _authService.GetUserRole() != "Admin")
                return RedirectToAction("AccessDenied", "Account");

            if (id != teacher.IdTeacher)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingTeacher = await _context.Teachers.FindAsync(id);
                    if (existingTeacher == null)
                    {
                        return NotFound();
                    }

                    // Обновляем данные преподавателя
                    existingTeacher.Name = teacher.Name;
                    existingTeacher.SecondName = teacher.SecondName;
                    existingTeacher.Patronymic = teacher.Patronymic;
                    existingTeacher.PhoneNumber = teacher.PhoneNumber;
                    existingTeacher.IdDepartament = teacher.IdDepartament;

                    _context.Teachers.Update(existingTeacher);

                    // Обновляем связанного пользователя
                    var user = await _context.Users
                        .FirstOrDefaultAsync(u => u.IdTeacher == id);

                    if (user != null)
                    {
                        // Обновляем пароль если изменился
                        if (!string.IsNullOrEmpty(teacher.Password) && teacher.Password != existingTeacher.Password)
                        {
                            existingTeacher.Password = teacher.Password;
                            user.Password = teacher.Password;
                            _context.Users.Update(user);
                        }
                    }

                    // Обновляем дисциплины преподавателя
                    if (disciplineIds != null)
                    {
                        // Удаляем старые связи
                        var oldDisciplines = await _context.TeacherDisciplines
                            .Where(td => td.IdTeacher == id)
                            .ToListAsync();

                        _context.TeacherDisciplines.RemoveRange(oldDisciplines);

                        // Добавляем новые связи
                        foreach (var disciplineId in disciplineIds)
                        {
                            var teacherDiscipline = new TeacherDiscipline
                            {
                                IdTeacher = id,
                                IdDiscipline = disciplineId
                            };
                            _context.TeacherDisciplines.Add(teacherDiscipline);
                        }
                    }

                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Данные преподавателя успешно обновлены!";
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewBag.Departaments = await _context.Departaments.ToListAsync();
            ViewBag.Disciplines = await _context.Disciplines.ToListAsync();

            return View(teacher);
        }

        private bool TeacherExists(int id)
        {
            return _context.Teachers.Any(e => e.IdTeacher == id);
        }
    }
}