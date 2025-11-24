using Microsoft.AspNetCore.Mvc;
using UniversitySystem.Models;
using UniversitySystem.Services;
using UniversitySystem.Data;
using Microsoft.EntityFrameworkCore;

namespace UniversitySystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly UniversityContext _context;
        private readonly AuthService _authService;

        public HomeController(UniversityContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.StudentsCount = await _context.Students.CountAsync();
            ViewBag.TeacherCount = await _context.Teachers.CountAsync();
            ViewBag.GroupsCount = await _context.StudentGroups.CountAsync();
            ViewBag.DepartamentsCount = await _context.Departaments.CountAsync();
            ViewBag.DisciplineCount = await _context.Disciplines.CountAsync();

            ViewBag.LatestNews = await _context.News
                .Where(n => n.IsPublished)
                .OrderByDescending(n => n.PublishDate)
                .Take(3)
                .ToListAsync();

            ViewBag.ActivePromotions = await _context.Promotions
                .Where(p => p.IsActive && p.EndDate >= DateTime.Now)
                .OrderByDescending(p => p.StartDate)
                .Take(2)
                .ToListAsync();

            return View();
        }

        public IActionResult AdminDashboard()
        {
            if (!_authService.IsAuthenticated() || _authService.GetUserRole() != "Admin")
                return RedirectToAction("AccessDenied", "Account");

            ViewBag.UserName = _authService.GetUserName();
            ViewBag.UserRole = _authService.GetUserRole();

            return View();
        }

        public async Task<IActionResult> TeacherDashboard()
        {
            if (!_authService.IsAuthenticated() || _authService.GetUserRole() != "Teacher")
                return RedirectToAction("AccessDenied", "Account");

            var userId = _authService.GetUserId();
            var user = await _context.Users
                .Include(u => u.Teacher)
                .ThenInclude(t => t.Departament)
                .FirstOrDefaultAsync(u => u.IdUser == userId);

            ViewBag.UserName = _authService.GetUserName();
            ViewBag.UserRole = _authService.GetUserRole();
            ViewBag.Teacher = user?.Teacher;

            return View();
        }

        public async Task<IActionResult> StudentDashboard()
        {
            if (!_authService.IsAuthenticated() || _authService.GetUserRole() != "Student")
                return RedirectToAction("AccessDenied", "Account");

            var userId = _authService.GetUserId();
            var user = await _context.Users
                .Include(u => u.Student)
                .ThenInclude(s => s.Group)
                .ThenInclude(g => g.Departament)
                .FirstOrDefaultAsync(u => u.IdUser == userId);

            ViewBag.UserName = _authService.GetUserName();
            ViewBag.UserRole = _authService.GetUserRole();
            ViewBag.Student = user?.Student;

            return View();
        }

        public async Task<IActionResult> News()
        {
            var news = await _context.News
                .Where(n => n.IsPublished)
                .OrderByDescending(n => n.PublishDate)
                .ToListAsync();

            return View(news);
        }

        public async Task<IActionResult> Promotions()
        {
            var promotions = await _context.Promotions
                .Where(p => p.IsActive && p.EndDate >= DateTime.Now)
                .OrderByDescending(p => p.StartDate)
                .ToListAsync();

            return View(promotions);
        }

        public async Task<IActionResult> NewsDetails(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null || !news.IsPublished)
            {
                return NotFound();
            }
            return View(news);
        }

        public async Task<IActionResult> PromotionDetails(int id)
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null || !promotion.IsActive)
            {
                return NotFound();
            }
            return View(promotion);
        }

        public async Task<IActionResult> Search(string searchString)
        {
            ViewBag.SearchString = searchString;

            if (string.IsNullOrEmpty(searchString))
            {
                ViewBag.NewsResults = new List<News>();
                ViewBag.PromotionResults = new List<Promotion>();
                return View();
            }

            var newsResults = await _context.News
                .Where(n => n.IsPublished &&
                           (n.Title.Contains(searchString) || n.Content.Contains(searchString)))
                .ToListAsync();

            var promotionResults = await _context.Promotions
                .Where(p => p.IsActive &&
                           (p.Title.Contains(searchString) || p.Description.Contains(searchString)))
                .ToListAsync();

            ViewBag.NewsResults = newsResults;
            ViewBag.PromotionResults = promotionResults;

            return View();
        }

        public IActionResult InitializeDatabase()
        {
            try
            {
                _context.Database.EnsureDeleted();
                _context.Database.EnsureCreated();

                DbInitializer.Initialize(_context);

                TempData["Message"] = "✅ База данных успешно инициализирована с тестовыми данными!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ Ошибка при инициализации: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Информация о нашем учебном заведении";
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Контактная информация";
            return View();
        }
    }
}