using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TenantApp.Providers;

namespace TenantApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            //.AddMvcOptions(options =>
            //{
            //    options.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter()); // 添加xml请求响应格式
            //});

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ITenantProvider, BlobStorageTenantProvider>();
            services.AddScoped<Microsoft.EntityFrameworkCore.DbContextOptions<PlayListContext>>();
            services.AddSingleton<PlayListContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // 状态错误码页
            app.UseStatusCodePages();

            app.UseMvc();
        }
    }
}