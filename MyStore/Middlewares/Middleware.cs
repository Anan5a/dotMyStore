using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyStore.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class Middleware
    {
        private readonly RequestDelegate _next;

        public Middleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var watch = Stopwatch.StartNew();
            System.Diagnostics.Debug.WriteLine($"Got request: {httpContext.Request.Method} {httpContext.Request.Path}");
            System.Diagnostics.Debug.WriteLine($"Request type: {httpContext.Request.Headers.Accept}");
            System.Diagnostics.Debug.WriteLine($"User auth status: {httpContext.User.Identity?.IsAuthenticated}");

            //short circuit!
            watch.Stop();
            httpContext.Response.Headers.Append("X-Message", "Short circuit :v");
            httpContext.Response.Headers.Append("X-Time", watch.ElapsedMilliseconds.ToString());

            httpContext.Response.StatusCode = 403;
            await httpContext.Response.WriteAsync("Access denied");
            return;
            //end short circuit
            await _next(httpContext);

            System.Diagnostics.Debug.WriteLine($"After _next(req): {httpContext.Response.StatusCode} {httpContext.Response.Headers.ContentType}");
        }
    }
}
