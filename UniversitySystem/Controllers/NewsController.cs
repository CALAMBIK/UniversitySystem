using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversitySystem.Models;
using UniversitySystem.Data;
using UniversitySystem.Services;

namespace UniversitySystem.Controllers
{
    public class NewsController : Controller
    {
        private readonly UniversityContext _context;
        private readonly AuthService _authService;

        public NewsController(UniversityContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<IActionResult> Admin()
        {
            if (!_authService.IsAuthenticated() || _authService.GetUserRole() != "Admin")
                return RedirectToAction("AccessDenied", "Account");

            var news = await _context.News
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();

            return View(news);
        }

        public IActionResult Create()
        {
            if (!_authService.IsAuthenticated() || _authService.GetUserRole() != "Admin")
                return RedirectToAction("AccessDenied", "Account");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(News news)
        {
            if (!_authService.IsAuthenticated() || _authService.GetUserRole() != "Admin")
                return RedirectToAction("AccessDenied", "Account");

            if (ModelState.IsValid)
            {
                news.CreatedDate = DateTime.Now;
                news.PublishDate = DateTime.Now;
                news.Author = _authService.GetUserName();

                _context.News.Add(news);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Новость успешно создана!";
                return RedirectToAction("Admin");
            }

            return View(news);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!_authService.IsAuthenticated() || _authService.GetUserRole() != "Admin")
                return RedirectToAction("AccessDenied", "Account");

            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, News news)
        {
            if (!_authService.IsAuthenticated() || _authService.GetUserRole() != "Admin")
                return RedirectToAction("AccessDenied", "Account");

            if (id != news.IdNews)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingNews = await _context.News.FindAsync(id);
                    if (existingNews == null)
                    {
                        return NotFound();
                    }

                    existingNews.Title = news.Title;
                    existingNews.Content = news.Content;
                    existingNews.ImageUrl = news.ImageUrl;
                    existingNews.IsPublished = news.IsPublished;
                    existingNews.PublishDate = news.PublishDate;

                    _context.News.Update(existingNews);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Новость успешно обновлена!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Admin");
            }
            return View(news);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!_authService.IsAuthenticated() || _authService.GetUserRole() != "Admin")
                return RedirectToAction("AccessDenied", "Account");

            var news = await _context.News.FindAsync(id);
            if (news != null)
            {
                _context.News.Remove(news);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Новость успешно удалена!";
            }

            return RedirectToAction("Admin");
        }

        [HttpPost]
        public async Task<IActionResult> TogglePublish(int id)
        {
            if (!_authService.IsAuthenticated() || _authService.GetUserRole() != "Admin")
                return RedirectToAction("AccessDenied", "Account");

            var news = await _context.News.FindAsync(id);
            if (news != null)
            {
                news.IsPublished = !news.IsPublished;
                _context.News.Update(news);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = news.IsPublished ?
                    "Новость опубликована!" : "Новость снята с публикации!";
            }

            return RedirectToAction("Admin");
        }

        private bool NewsExists(int id)
        {
            return _context.News.Any(e => e.IdNews == id);
        }
    }
}