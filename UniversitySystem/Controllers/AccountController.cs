using Microsoft.AspNetCore.Mvc;
using UniversitySystem.Services;
using UniversitySystem.Models;
using UniversitySystem.Data;
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

            if (user != null)
            {
                if (userRole == "Student" && user.IdStudent.HasValue)
                {
                    ViewBag.UserData = user.Student;
                }
                else if (userRole == "Teacher" && user.IdTeacher.HasValue)
                {
                    ViewBag.UserData = user.Teacher;
                }
                else if (userRole == "Admin")
                {
                    ViewBag.StudentsCount = await _context.Students.CountAsync();
                    ViewBag.TeachersCount = await _context.Teachers.CountAsync();
                    ViewBag.GroupsCount = await _context.StudentGroups.CountAsync();
                }
            }

            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}