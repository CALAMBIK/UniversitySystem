using UniversitySystem.Models;
using Microsoft.EntityFrameworkCore;
using UniversitySystem.Data;

namespace UniversitySystem.Services
{
    public class AuthService
    {
        private readonly UniversityContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(UniversityContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<User?> Authenticate(string login, string password)
        {
            var user = await _context.Users
                .Include(u => u.Student)
                .Include(u => u.Teacher)
                .FirstOrDefaultAsync(u => u.Login == login && u.Password == password);

            if (user != null)
            {
                user.LastLogin = DateTime.Now;
                await _context.SaveChangesAsync();

                string userName = GetUserName(user);

                _httpContextAccessor.HttpContext.Session.SetString("UserId", user.IdUser.ToString());
                _httpContextAccessor.HttpContext.Session.SetString("UserRole", user.Role);
                _httpContextAccessor.HttpContext.Session.SetString("UserName", userName);
            }

            return user;
        }

        private string GetUserName(User user)
        {
            return user.Role switch
            {
                "Student" when user.Student != null => $"{user.Student.SecondName} {user.Student.Name}",
                "Teacher" when user.Teacher != null => $"{user.Teacher.SecondName} {user.Teacher.Name}",
                "Admin" => "Администратор",
                _ => user.Login
            };
        }

        public bool IsAuthenticated()
        {
            return _httpContextAccessor.HttpContext.Session.GetString("UserId") != null;
        }

        public string GetUserRole()
        {
            return _httpContextAccessor.HttpContext.Session.GetString("UserRole") ?? "";
        }

        public string GetUserName()
        {
            return _httpContextAccessor.HttpContext.Session.GetString("UserName") ?? "";
        }

        public int GetUserId()
        {
            var userId = _httpContextAccessor.HttpContext.Session.GetString("UserId");
            return userId != null ? int.Parse(userId) : 0;
        }

        public void Logout()
        {
            _httpContextAccessor.HttpContext.Session.Clear();
        }
    }
}