using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;

namespace WebApiDoc
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddLogging();
            
            // Add our repository type
            //services.AddSingleton<ITodoRepository, >

            // inject an implementaion of ISwaggerProvider withdefaulted settings applied
            services.AddSwaggerGen();

            // Add the detail information for the API.
            services.ConfigureSwaggerGen(options =>
            {
                options.SingleApiVersion(new Swashbuckle.Swagger.Model.Info
                {
                    Version = "v1",
                    Title = "ToDo API",
                    Description = "A simple example ASP.NET Core Web API",
                    TermsOfService = "None",
                    Contact = new Swashbuckle.Swagger.Model.Contact
                    {
                        Name = "Jackie Lee",
                        Email = "zzz",
                        Url = "http://dralee.com"
                    },
                    License = new Swashbuckle.Swagger.Model.License
                    {
                        Name = "Use under LICX",
                        Url = "http://url.com"
                    }
                });
            // Determine base path for the application.
            var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                // Set the comments path for the swagger json and ui.
                options.IncludeXmlComments($"{basePath}/WebApiDoc.xml");
            });

            // 查询方式：
            // http://localhost:<random_port>/swagger/v1/swagger.json 页面可以看成生成的终端描述文档。
            // 可以通过访问 http://localhost:<random_port>/swagger/ui 页面来查看。
            // 详情使用：http://www.cnblogs.com/dotNETCoreSG/p/aspnetcore_02-09_web-api-help-pages-using-swagger.html
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseMvcWithDefaultRoute();
            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();

            // Enable middelware to serve swagger-ui asset(HTML,JS,CSS etc.)
            app.UseSwaggerUi();

            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute("default",
                    template: "api/{controller=Todo}/{id?}");
            });

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }
    }
}
