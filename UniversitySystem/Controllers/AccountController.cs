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

        [HttpGet]
        public IActionResult Register()
        {
            if (_authService.IsAuthenticated())
            {
                return RedirectToAction("Profile", "Account");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            string login,
            string password,
            string confirmPassword,
            string role,
            string email,
            string phone,
            string secondName,
            string name,
            string patronymic,
            int? groupId = null,
            int? departamentId = null,
            DateTime? dateBirthday = null,
            string academicDegree = null)
        {
            // Проверка подтверждения пароля
            if (password != confirmPassword)
            {
                TempData["ErrorMessage"] = "Пароли не совпадают!";
                return View();
            }

            // Проверка на минимальную длину пароля
            if (password.Length < 6)
            {
                TempData["ErrorMessage"] = "Пароль должен содержать минимум 6 символов!";
                return View();
            }

            // Проверяем, существует ли пользователь с таким логином
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Login == login);
            if (existingUser != null)
            {
                TempData["ErrorMessage"] = "Пользователь с таким логином уже существует!";
                return View();
            }

            // Проверяем корректность роли
            if (role != "Student" && role != "Teacher")
            {
                TempData["ErrorMessage"] = "Выберите корректную роль!";
                return View();
            }

            // Проверяем обязательные поля ФИО
            if (string.IsNullOrEmpty(secondName) || string.IsNullOrEmpty(name))
            {
                TempData["ErrorMessage"] = "Фамилия и имя обязательны для заполнения!";
                return View();
            }

            try
            {
                // Создаем нового пользователя
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

                // Создаем профиль пользователя
                var profile = new UserProfile
                {
                    IdUser = user.IdUser,
                    Email = email,
                    Phone = phone,
                    UpdatedDate = DateTime.Now
                };
                _context.UserProfiles.Add(profile);
                await _context.SaveChangesAsync();

                // Если это студент или преподаватель, создаем соответствующую запись
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

                    // Обновляем пользователя с IdStudent
                    user.IdStudent = student.IdStudent;
                    await _context.SaveChangesAsync();
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

                    // Обновляем пользователя с IdTeacher
                    user.IdTeacher = teacher.IdTeacher;
                    await _context.SaveChangesAsync();
                }

                // Автоматически логиним пользователя после регистрации
                await _authService.Authenticate(login, password);

                TempData["SuccessMessage"] = "Регистрация прошла успешно! Добро пожаловать в систему.";
                return RedirectToAction("Profile", "Account");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ошибка при регистрации: {ex.Message}";
                return View();
            }
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

            // Получаем пользователя с включенными связанными данными
            var user = await _context.Users
                .Include(u => u.Student)
                    .ThenInclude(s => s.Group)
                    .ThenInclude(g => g.Departament)
                .Include(u => u.Teacher)
                    .ThenInclude(t => t.Departament)
                .FirstOrDefaultAsync(u => u.IdUser == userId);

            // Получаем профиль пользователя отдельно
            var userProfile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.IdUser == userId);

            if (user != null)
            {
                // Получаем дополнительные данные в зависимости от роли
                if (userRole == "Student" && user.IdStudent.HasValue)
                {
                    ViewBag.UserData = user.Student;
                    // Получаем статистику запросов материалов
                    ViewBag.PendingRequestsCount = await _context.MaterialRequests
                        .CountAsync(r => r.IdUser == userId && r.Status == "Pending");
                    ViewBag.CompletedRequestsCount = await _context.MaterialRequests
                        .CountAsync(r => r.IdUser == userId && r.Status == "Completed");
                }
                else if (userRole == "Teacher" && user.IdTeacher.HasValue)
                {
                    ViewBag.UserData = user.Teacher;
                    // Получаем статистику запросов материалов
                    ViewBag.PendingRequestsCount = await _context.MaterialRequests
                        .CountAsync(r => r.IdUser == userId && r.Status == "Pending");
                    ViewBag.CompletedRequestsCount = await _context.MaterialRequests
                        .CountAsync(r => r.IdUser == userId && r.Status == "Completed");
                }
                else if (userRole == "Admin")
                {
                    ViewBag.StudentsCount = await _context.Students.CountAsync();
                    ViewBag.TeachersCount = await _context.Teachers.CountAsync();
                    ViewBag.GroupsCount = await _context.StudentGroups.CountAsync();
                    ViewBag.PendingRequestsCount = await _context.MaterialRequests
                        .CountAsync(r => r.Status == "Pending");
                    ViewBag.TotalRequestsCount = await _context.MaterialRequests.CountAsync();
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

                // Удаляем запросы материалов пользователя
                var requests = await _context.MaterialRequests.Where(r => r.IdUser == currentUserId).ToListAsync();
                if (requests.Any())
                {
                    _context.MaterialRequests.RemoveRange(requests);
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
    }
}