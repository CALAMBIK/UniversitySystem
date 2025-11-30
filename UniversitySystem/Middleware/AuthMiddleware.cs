using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace UniversitySystem.Middleware
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path;

            var publicPaths = new[]
            {
                "/Account/Login",
                "/Home/Login",
                "/Account/Register",
                "/Home/Index",
                "/Home/About",
                "/Home/Contact",
                "/Home/News",
                "/Home/Promotions",
                "/Home/NewsDetails",
                "/Home/PromotionDetails",
                "/Home/Search",
                "/Home/Error",
                "/css/",
                "/js/",
                "/lib/"
            };

            bool isPublicPath = publicPaths.Any(p =>
                path.StartsWithSegments(p, StringComparison.OrdinalIgnoreCase) ||
                path.Value?.Contains(p) == true);

            if (!isPublicPath && context.Session.GetString("UserId") == null)
            {
                context.Response.Redirect("/Account/Login?returnUrl=" + Uri.EscapeDataString(path));
                return;
            }

            await _next(context);
        }
    }
}