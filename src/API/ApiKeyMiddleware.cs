using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace API
{
    // http://www.mithunvp.com/write-custom-asp-net-core-middleware-web-api/
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            if (context.Request.Headers["ApiKey"] == "3A51xfj2O7PvuL8Rc9J7a4OUhP8CXHD7")
                return _next(context);
            else
            {
                context.Response.StatusCode = 403;
                return Task.CompletedTask;
            }
        }
    }

    public static class ApiKeyMiddlewareExtensions
    {
        public static IApplicationBuilder UseApiKey(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiKeyMiddleware>();
        }
    }
}
