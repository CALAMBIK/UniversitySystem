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
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return View("GuestIndex");
            }

            ViewBag.StudentsCount = await _context.Students.CountAsync();
            ViewBag.TeacherCount = await _context.Teachers.CountAsync();
            ViewBag.GroupsCount = await _context.StudentGroups.CountAsync();
            ViewBag.DepartamentsCount = await _context.Departaments.CountAsync();
            ViewBag.DisciplineCount = await _context.Disciplines.CountAsync();
            ViewBag.MaterialsCount = await _context.CourseMaterials.CountAsync();

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

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (_authService.IsAuthenticated())
            {
                return RedirectToAction("Profile", "Account");
            }

            ViewBag.ReturnUrl = returnUrl;
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
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public IActionResult AdminDashboard()
        {
            if (!_authService.IsAuthenticated() || _authService.GetUserRole() != "Admin")
                return RedirectToAction("AccessDenied", "Account");

            ViewBag.StudentsCount = _context.Students.Count();
            ViewBag.TeachersCount = _context.Teachers.Count();
            ViewBag.GroupsCount = _context.StudentGroups.Count();
            ViewBag.DepartamentsCount = _context.Departaments.Count();
            ViewBag.MaterialsCount = _context.CourseMaterials.Count();

            ViewBag.NewsCount = _context.News.Count();
            ViewBag.PublishedNewsCount = _context.News.Count(n => n.IsPublished);

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

            if (user?.Teacher != null)
            {
                // Получаем статистику материалов преподавателя
                ViewBag.MaterialsCount = await _context.CourseMaterials
                    .CountAsync(cm => cm.IdTeacher == user.Teacher.IdTeacher);

                // Получаем дисциплины преподавателя
                ViewBag.Disciplines = await _context.TeacherDisciplines
                    .Include(td => td.Discipline)
                    .Include(td => td.Group)
                    .Where(td => td.IdTeacher == user.Teacher.IdTeacher)
                    .ToListAsync();

                // Получаем последние материалы
                ViewBag.RecentMaterials = await _context.CourseMaterials
                    .Include(cm => cm.Group)
                    .Include(cm => cm.Discipline)
                    .Where(cm => cm.IdTeacher == user.Teacher.IdTeacher)
                    .OrderByDescending(cm => cm.CreatedDate)
                    .Take(5)
                    .ToListAsync();
            }

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

            if (user?.Student != null)
            {
                ViewBag.GroupStudentsCount = await _context.Students
                    .CountAsync(s => s.IdGroup == user.Student.IdGroup);

                // Получаем материалы для студента
                ViewBag.RecentMaterials = await _context.CourseMaterials
                    .Include(cm => cm.Teacher)
                    .Include(cm => cm.Discipline)
                    .Where(cm => cm.IdGroup == user.Student.IdGroup)
                    .OrderByDescending(cm => cm.CreatedDate)
                    .Take(5)
                    .ToListAsync();

                // Получаем преподавателей студента
                ViewBag.Teachers = await _context.CourseMaterials
                    .Include(cm => cm.Teacher)
                    .Where(cm => cm.IdGroup == user.Student.IdGroup)
                    .Select(cm => cm.Teacher)
                    .Distinct()
                    .ToListAsync();
            }

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

        public IActionResult Error()
        {
            return View();
        }
    }
}