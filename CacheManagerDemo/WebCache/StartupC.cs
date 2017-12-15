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
    // SqlServer分布式缓存
    // 使用Microsoft.Extensions.Caching.SqlServer作为缓存
    // 可通过Microsoft.Extensions.Caching.SqlConfig.Tools创建表结构
    // 或手动创建(由于sqlConfig.Tools版本问题没装成功，故以下结构为猜出)，如下：
    /*
     CREATE DATABASE demodb
     USE demodb

    CREATE TABLE AspnetCache
    (
	    Id NVARCHAR(100) PRIMARY KEY,
	    Value VARBINARY(max),
	    ExpiresAtTime DATETIMEOFFSET,
	    SlidingExpirationInSeconds BIGINT,
	    AbsoluteExpiration DATETIMEOFFSET
    )

    // 数据如下：
    StartTime 	0x323031372F352F31312031363A30303A3034	2017-05-11 08:20:21.7367929 +00:00	1200	NULL
*/

    public class StartupC
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = "server=localhost;database=demodb;uid=sa;pwd=123";
                options.SchemaName = "dbo";
                options.TableName = "AspnetCache";
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
