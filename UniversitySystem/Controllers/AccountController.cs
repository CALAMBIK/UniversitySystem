using UniversitySystem.Data;
using Microsoft.AspNetCore.Mvc;
using UniversitySystem.Services;
using UniversitySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace UniversitySystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthService _authService;
        private readonly UniversityContext _context;

        public AccountController(AuthService authService, UniversityContext context)
        {
            _authService = authService;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (_authService.IsAuthenticated())
            {
                return RedirectToAction("Profile", "Account");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string login, string password, string returnUrl = null)
        {
            var user = await _authService.Authenticate(login, password);

            if (user != null)
            {
                TempData["SuccessMessage"] = $"Добро пожаловать, {_authService.GetUserName()}!";

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Profile", "Account");
            }

            TempData["ErrorMessage"] = "Неверный логин или пароль!";
            return View();
        }

        public IActionResult Logout()
        {
            _authService.Logout();
            TempData["SuccessMessage"] = "Вы успешно вышли из системы!";
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Profile()
        {
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = _authService.GetUserId();
            var userRole = _authService.GetUserRole();

            ViewBag.UserName = _authService.GetUserName();
            ViewBag.UserRole = userRole;

            var user = await _context.Users
                .Include(u => u.Student)
                    .ThenInclude(s => s.Group)
                    .ThenInclude(g => g.Departament)
                .Include(u => u.Teacher)
                    .ThenInclude(t => t.Departament)
                .FirstOrDefaultAsync(u => u.IdUser == userId);

            var userProfile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.IdUser == userId);

            if (user != null)
            {
                if (userRole == "Student" && user.IdStudent.HasValue)
                {
                    ViewBag.UserData = user.Student;

                    if (user.Student.IdGroup.HasValue)
                    {
                        ViewBag.CourseMaterials = await _context.CourseMaterials
                            .Include(cm => cm.Teacher)
                            .Include(cm => cm.Discipline)
                            .Where(cm => cm.IdGroup == user.Student.IdGroup)
                            .OrderByDescending(cm => cm.CreatedDate)
                            .Take(5)
                            .ToListAsync();

                        ViewBag.Disciplines = await _context.CourseMaterials
                            .Where(cm => cm.IdGroup == user.Student.IdGroup)
                            .Select(cm => cm.Discipline)
                            .Distinct()
                            .ToListAsync();
                    }
                }
                else if (userRole == "Teacher" && user.IdTeacher.HasValue)
                {
                    ViewBag.UserData = user.Teacher;

                    ViewBag.CourseMaterials = await _context.CourseMaterials
                        .Include(cm => cm.Group)
                        .Include(cm => cm.Discipline)
                        .Where(cm => cm.IdTeacher == user.IdTeacher)
                        .OrderByDescending(cm => cm.CreatedDate)
                        .Take(5)
                        .ToListAsync();

                    ViewBag.TeacherDisciplines = await _context.TeacherDisciplines
                        .Include(td => td.Discipline)
                        .Include(td => td.Group)
                        .Where(td => td.IdTeacher == user.IdTeacher)
                        .ToListAsync();
                }
                else if (userRole == "Admin")
                {
                    ViewBag.StudentsCount = await _context.Students.CountAsync();
                    ViewBag.TeachersCount = await _context.Teachers.CountAsync();
                    ViewBag.GroupsCount = await _context.StudentGroups.CountAsync();
                    ViewBag.MaterialsCount = await _context.CourseMaterials.CountAsync();
                }

                ViewBag.UserProfile = userProfile;
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            if (!_authService.IsAuthenticated())
                return RedirectToAction("Login", "Account");

            var currentUserId = _authService.GetUserId();
            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.IdUser == currentUserId);

            if (profile == null)
            {
                // Создаем профиль, если его нет
                profile = new UserProfile { IdUser = currentUserId };
                _context.UserProfiles.Add(profile);
                await _context.SaveChangesAsync();
            }

            // Получаем данные пользователя для отображения текущей информации
            var user = await _context.Users
                .Include(u => u.Student)
                .Include(u => u.Teacher)
                .FirstOrDefaultAsync(u => u.IdUser == currentUserId);

            ViewBag.User = user;
            ViewBag.UserRole = _authService.GetUserRole();

            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(UserProfile profile, string name, string secondName, string patronymic, string phoneNumber)
        {
            if (!_authService.IsAuthenticated())
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                try
                {
                    var currentUserId = _authService.GetUserId();
                    var userRole = _authService.GetUserRole();

                    // Обновляем профиль
                    profile.IdUser = currentUserId;
                    profile.UpdatedDate = DateTime.Now;

                    _context.UserProfiles.Update(profile);
                    await _context.SaveChangesAsync();

                    // Обновляем данные студента или преподавателя
                    var user = await _context.Users
                        .Include(u => u.Student)
                        .Include(u => u.Teacher)
                        .FirstOrDefaultAsync(u => u.IdUser == currentUserId);

                    if (userRole == "Student" && user?.Student != null)
                    {
                        user.Student.Name = name;
                        user.Student.SecondName = secondName;
                        user.Student.Patronymic = patronymic;
                        user.Student.PhoneNumber = phoneNumber;
                        _context.Students.Update(user.Student);
                    }
                    else if (userRole == "Teacher" && user?.Teacher != null)
                    {
                        user.Teacher.Name = name;
                        user.Teacher.SecondName = secondName;
                        user.Teacher.Patronymic = patronymic;
                        user.Teacher.PhoneNumber = phoneNumber;
                        _context.Teachers.Update(user.Teacher);
                    }

                    await _context.SaveChangesAsync();

                    // Обновляем имя в сессии
                    string newUserName = userRole switch
                    {
                        "Student" when user?.Student != null => $"{user.Student.SecondName} {user.Student.Name}",
                        "Teacher" when user?.Teacher != null => $"{user.Teacher.SecondName} {user.Teacher.Name}",
                        _ => _authService.GetUserName()
                    };

                    HttpContext.Session.SetString("UserName", newUserName);

                    TempData["SuccessMessage"] = "Профиль успешно обновлен!";
                    return RedirectToAction("Profile");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Ошибка при обновлении профиля: {ex.Message}";
                }
            }

            // Если есть ошибки, возвращаем обратно с данными
            var userIdForView = _authService.GetUserId();
            var userForView = await _context.Users
                .Include(u => u.Student)
                .Include(u => u.Teacher)
                .FirstOrDefaultAsync(u => u.IdUser == userIdForView);

            ViewBag.User = userForView;
            ViewBag.UserRole = _authService.GetUserRole();

            return View(profile);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (!_authService.IsAuthenticated())
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (!_authService.IsAuthenticated())
                return RedirectToAction("Login", "Account");

            if (newPassword != confirmPassword)
            {
                TempData["ErrorMessage"] = "Новые пароли не совпадают!";
                return View();
            }

            if (newPassword.Length < 6)
            {
                TempData["ErrorMessage"] = "Новый пароль должен содержать минимум 6 символов!";
                return View();
            }

            var currentUserId = _authService.GetUserId();
            var user = await _context.Users.FindAsync(currentUserId);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Пользователь не найден!";
                return RedirectToAction("Login", "Account");
            }

            // Проверяем текущий пароль
            if (user.Password != currentPassword)
            {
                TempData["ErrorMessage"] = "Текущий пароль указан неверно!";
                return View();
            }

            try
            {
                // Обновляем пароль
                user.Password = newPassword;
                _context.Users.Update(user);

                // Также обновляем пароль в соответствующей таблице (Student или Teacher)
                var userRole = _authService.GetUserRole();
                if (userRole == "Student" && user.IdStudent.HasValue)
                {
                    var student = await _context.Students.FindAsync(user.IdStudent.Value);
                    if (student != null)
                    {
                        student.Password = newPassword;
                        _context.Students.Update(student);
                    }
                }
                else if (userRole == "Teacher" && user.IdTeacher.HasValue)
                {
                    var teacher = await _context.Teachers.FindAsync(user.IdTeacher.Value);
                    if (teacher != null)
                    {
                        teacher.Password = newPassword;
                        _context.Teachers.Update(teacher);
                    }
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Пароль успешно изменен!";
                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ошибка при изменении пароля: {ex.Message}";
                return View();
            }
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult DeleteAccount()
        {
            if (!_authService.IsAuthenticated())
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount(string confirmPassword)
        {
            if (!_authService.IsAuthenticated())
                return RedirectToAction("Login", "Account");

            var currentUserId = _authService.GetUserId();
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.IdUser == currentUserId);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Пользователь не найден!";
                return RedirectToAction("Login", "Account");
            }

            // Проверяем пароль
            if (user.Password != confirmPassword)
            {
                TempData["ErrorMessage"] = "Неверный пароль!";
                return View();
            }

            try
            {
                // Удаляем связанные данные
                var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.IdUser == currentUserId);
                if (profile != null)
                {
                    _context.UserProfiles.Remove(profile);
                }

                // Удаляем учебные материалы, если пользователь - преподаватель
                if (user.IdTeacher.HasValue)
                {
                    var materials = await _context.CourseMaterials
                        .Where(m => m.IdTeacher == user.IdTeacher.Value)
                        .ToListAsync();
                    if (materials.Any())
                    {
                        _context.CourseMaterials.RemoveRange(materials);
                    }

                    // Удаляем связи преподавателя с дисциплинами
                    var teacherDisciplines = await _context.TeacherDisciplines
                        .Where(td => td.IdTeacher == user.IdTeacher.Value)
                        .ToListAsync();
                    if (teacherDisciplines.Any())
                    {
                        _context.TeacherDisciplines.RemoveRange(teacherDisciplines);
                    }
                }

                // Удаляем запись студента или преподавателя
                if (user.Role == "Student" && user.IdStudent.HasValue)
                {
                    var student = await _context.Students.FindAsync(user.IdStudent.Value);
                    if (student != null)
                    {
                        _context.Students.Remove(student);
                    }
                }
                else if (user.Role == "Teacher" && user.IdTeacher.HasValue)
                {
                    var teacher = await _context.Teachers.FindAsync(user.IdTeacher.Value);
                    if (teacher != null)
                    {
                        _context.Teachers.Remove(teacher);
                    }
                }

                // Удаляем пользователя
                _context.Users.Remove(user);

                await _context.SaveChangesAsync();

                // Выходим из системы
                _authService.Logout();

                TempData["SuccessMessage"] = "Ваш аккаунт был успешно удален.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ошибка при удалении аккаунта: {ex.Message}";
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> RegisterUser()
        {
            if (!_authService.IsAuthenticated() || _authService.GetUserRole() != "Admin")
                return RedirectToAction("AccessDenied", "Account");

            ViewBag.Groups = await _context.StudentGroups
                .Include(g => g.Departament)
                .OrderBy(g => g.NumberGroup)
                .ToListAsync();
            ViewBag.Departaments = await _context.Departaments
                .OrderBy(d => d.Name)
                .ToListAsync();
            ViewBag.Disciplines = await _context.Disciplines
                .OrderBy(d => d.Name)
                .ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterUser(
            string login,
            string password,
            string confirmPassword,
            string role,
            string email,
            string phone,
            string secondName,
            string name,
            string patronymic = null,
            int? groupId = null,
            int? departamentId = null,
            DateTime? dateBirthday = null,
            List<int> disciplineIds = null)
        {
            if (!_authService.IsAuthenticated() || _authService.GetUserRole() != "Admin")
                return RedirectToAction("AccessDenied", "Account");

            // Валидация
            if (password != confirmPassword)
            {
                TempData["ErrorMessage"] = "Пароли не совпадают!";
                return RedirectToAction("RegisterUser");
            }

            if (password.Length < 6)
            {
                TempData["ErrorMessage"] = "Пароль должен содержать минимум 6 символов!";
                return RedirectToAction("RegisterUser");
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Login == login);
            if (existingUser != null)
            {
                TempData["ErrorMessage"] = "Пользователь с таким логином уже существует!";
                return RedirectToAction("RegisterUser");
            }

            if (role != "Student" && role != "Teacher")
            {
                TempData["ErrorMessage"] = "Выберите корректную роль!";
                return RedirectToAction("RegisterUser");
            }

            if (string.IsNullOrEmpty(secondName) || string.IsNullOrEmpty(name))
            {
                TempData["ErrorMessage"] = "Фамилия и имя обязательны для заполнения!";
                return RedirectToAction("RegisterUser");
            }

            try
            {
                // Создаем пользователя
                var user = new User
                {
                    Login = login,
                    Password = password,
                    Role = role,
                    CreatedDate = DateTime.Now,
                    LastLogin = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Создаем профиль
                var profile = new UserProfile
                {
                    IdUser = user.IdUser,
                    Email = email,
                    Phone = phone,
                    UpdatedDate = DateTime.Now
                };
                _context.UserProfiles.Add(profile);

                // Создаем запись студента или преподавателя
                if (role == "Student")
                {
                    var student = new Student
                    {
                        Login = login,
                        Password = password,
                        Name = name,
                        SecondName = secondName,
                        Patronymic = patronymic,
                        PhoneNumber = phone,
                        IdGroup = groupId,
                        DateBirthday = dateBirthday
                    };
                    _context.Students.Add(student);
                    await _context.SaveChangesAsync();

                    user.IdStudent = student.IdStudent;
                }
                else if (role == "Teacher")
                {
                    var teacher = new Teacher
                    {
                        Login = login,
                        Password = password,
                        Name = name,
                        SecondName = secondName,
                        Patronymic = patronymic,
                        PhoneNumber = phone,
                        IdDepartament = departamentId
                    };
                    _context.Teachers.Add(teacher);
                    await _context.SaveChangesAsync();

                    user.IdTeacher = teacher.IdTeacher;

                    // Добавляем дисциплины преподавателю
                    if (disciplineIds != null && disciplineIds.Any())
                    {
                        foreach (var disciplineId in disciplineIds)
                        {
                            var teacherDiscipline = new TeacherDiscipline
                            {
                                IdTeacher = teacher.IdTeacher,
                                IdDiscipline = disciplineId
                            };
                            _context.TeacherDisciplines.Add(teacherDiscipline);
                        }
                        await _context.SaveChangesAsync();
                    }
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Пользователь {secondName} {name} успешно зарегистрирован!";
                return RedirectToAction("AdminDashboard", "Home");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ошибка при регистрации: {ex.Message}";

                ViewBag.Groups = await _context.StudentGroups
                    .Include(g => g.Departament)
                    .OrderBy(g => g.NumberGroup)
                    .ToListAsync();
                ViewBag.Departaments = await _context.Departaments
                    .OrderBy(d => d.Name)
                    .ToListAsync();
                ViewBag.Disciplines = await _context.Disciplines
                    .OrderBy(d => d.Name)
                    .ToListAsync();

                return View();
            }
        }
    }
}