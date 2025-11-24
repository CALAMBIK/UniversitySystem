using Microsoft.AspNetCore.Mvc;
using UniversitySystem.Services;
using UniversitySystem.Models;

namespace UniversitySystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthService _authService;

        public AccountController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (_authService.IsAuthenticated())
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string login, string password)
        {
            var user = await _authService.Authenticate(login, password);

            if (user != null)
            {
                TempData["SuccessMessage"] = $"Добро пожаловать, {_authService.GetUserName()}!";

                return user.Role switch
                {
                    "Admin" => RedirectToAction("AdminDashboard", "Home"),
                    "Teacher" => RedirectToAction("TeacherDashboard", "Home"),
                    "Student" => RedirectToAction("StudentDashboard", "Home"),
                    _ => RedirectToAction("Index", "Home")
                };
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

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}