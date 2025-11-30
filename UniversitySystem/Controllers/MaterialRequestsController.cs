using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversitySystem.Models;
using UniversitySystem.Data;
using UniversitySystem.Services;

namespace UniversitySystem.Controllers
{
    public class MaterialRequestsController : Controller
    {
        private readonly UniversityContext _context;
        private readonly AuthService _authService;

        public MaterialRequestsController(UniversityContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        // Просмотр всех запросов пользователя
        public async Task<IActionResult> MyRequests()
        {
            if (!_authService.IsAuthenticated())
                return RedirectToAction("Login", "Account");

            var userId = _authService.GetUserId();
            var requests = await _context.MaterialRequests
                .Where(r => r.IdUser == userId)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();

            return View(requests);
        }

        // Создание нового запроса
        [HttpGet]
        public IActionResult Create()
        {
            if (!_authService.IsAuthenticated())
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MaterialRequest request)
        {
            if (!_authService.IsAuthenticated())
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                request.IdUser = _authService.GetUserId();
                request.Status = "Pending";
                request.CreatedDate = DateTime.Now;

                _context.MaterialRequests.Add(request);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Запрос успешно создан!";
                return RedirectToAction("MyRequests");
            }

            return View(request);
        }

        // Детали запроса
        public async Task<IActionResult> Details(int id)
        {
            if (!_authService.IsAuthenticated())
                return RedirectToAction("Login", "Account");

            var request = await _context.MaterialRequests
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.IdRequest == id);

            if (request == null || request.IdUser != _authService.GetUserId())
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            return View(request);
        }

        // Для администратора - управление всеми запросами
        public async Task<IActionResult> Admin()
        {
            if (!_authService.IsAuthenticated() || _authService.GetUserRole() != "Admin")
                return RedirectToAction("AccessDenied", "Account");

            var requests = await _context.MaterialRequests
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();

            return View(requests);
        }

        // Изменение статуса запроса (для админа)
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status, string comment)
        {
            if (!_authService.IsAuthenticated() || _authService.GetUserRole() != "Admin")
                return RedirectToAction("AccessDenied", "Account");

            var request = await _context.MaterialRequests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            request.Status = status;
            request.AdminComment = comment;

            if (status == "Approved")
                request.ProcessedDate = DateTime.Now;
            else if (status == "Completed")
                request.CompletedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Статус запроса обновлен!";
            return RedirectToAction("Admin");
        }
    }
}