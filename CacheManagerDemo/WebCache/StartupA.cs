using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

namespace WebCache
{
    // 使用Microsoft.Extensions.Caching.Memory作为缓存

    public class StartupA
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                IMemoryCache cache = context.RequestServices.GetRequiredService<IMemoryCache>();
                if (!cache.TryGetValue("StartTime", out DateTime startTime))
                {
                    cache.Set("StartTime", startTime = DateTime.Now);
                }
                await context.Response.WriteAsync($"start time:{startTime} now ({DateTime.Now})");
            });
        }
    }
}
