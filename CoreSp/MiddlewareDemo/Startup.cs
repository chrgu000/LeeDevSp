using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace MiddlewareDemo
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMessageService(new GreetingOptions
            {
                GreetAt = "Morning",
                GreetTo = "Jackie"
            });

            //services.AddMessageServiceAction(options =>
            //{
            //    options.GreetAt = "Morning";
            //    options.GreetTo = "Jackie";
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // 运行结果：Good Morning Jackie
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseGreetingMiddleware(new GreetingOptions
            {
                GreetAt = "Morning",
                GreetTo = "Jackie"
            });
        }

        // 运行结果：Good Morning Jackie - by lee
        //public void Configure(IApplicationBuilder app, IHostingEnvironment env, IMessageService messageService)
        //{
        //    app.Run(async (context) =>
        //    {
        //        await context.Response.WriteAsync(messageService.FormatMessage("by lee"));
        //    });
        //}
    }
}
