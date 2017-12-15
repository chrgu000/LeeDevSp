using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiddlewareDemo
{
    public class GreetingMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly GreetingOptions _options;

        public GreetingMiddleware(RequestDelegate next, GreetingOptions options)
        {
            _next = next;
            _options = options;
        }

        public async Task Invoke(HttpContext context)
        {
            var message = $"Good {_options.GreetAt} {_options.GreetTo}";
            await context.Response.WriteAsync(message);

            //await _next(context);
        }
    }
}
