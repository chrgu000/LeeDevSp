using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiddlewareDemo
{
    public static class Extensions
    {

        public static IApplicationBuilder UseGreetingMiddleware(this IApplicationBuilder app, GreetingOptions options)
        {
            return app.UseMiddleware<GreetingMiddleware>(options);
        }

        public static IServiceCollection AddMessageService(this IServiceCollection services, GreetingOptions options)
        {
            return services.AddScoped<IMessageService>(factory => new MessageService(options));
        }

        public static IServiceCollection AddMessageServiceAction(this IServiceCollection services, Action<GreetingOptions> optionsAction)
        {
            var options = new GreetingOptions();
            optionsAction(options);
            return services.AddScoped<IMessageService>(factory => new MessageService(options));
        }
    }
}
