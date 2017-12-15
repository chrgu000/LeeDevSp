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
using Microsoft.Extensions.Caching.Distributed;
using System.Text;

namespace WebCache
{
    // 分布式缓存
    // 使用Microsoft.Extensions.Caching.Redis作为缓存
    // 在redis-cli中，可通过“InstanceName”+"Key"的方式查看，如：（散列存储）
    // hgetall CoreResCacheDemoStartTime
    /*
     127.0.0.1:6379> hgetall CoreResCacheDemoStartTime
    1) "absexp"
    2) "-1"
    3) "sldexp"
    4) "-1"
    5) "data"
    6) "2017/5/11 15:25:12"
    */
    public class StartupB
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = "localhost";
                options.InstanceName = "CoreResCacheDemo";
            });
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
                var cache = context.RequestServices.GetRequiredService<IDistributedCache>();
                var startTime = await cache.GetStringAsync("StartTime");
                if (string.IsNullOrEmpty(startTime))
                {
                    startTime = DateTime.Now.ToString();
                    await cache.SetAsync("StartTime", Encoding.UTF8.GetBytes(startTime));
                }
                await context.Response.WriteAsync($"start time:{startTime} now ({DateTime.Now})");
            });
        }
    }
}
