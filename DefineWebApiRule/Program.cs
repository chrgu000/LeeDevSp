using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DefineWebApiRule
{
    class Program
    {
        static void Main(string[] args)
        {
            //WebHost.CreateDefaultBuilder(args)
            new WebHostBuilder().UseKestrel()
                .UseUrls("http://*:8080")
                .ConfigureServices(services =>
                {
                    services.AddMvcCore()
                    .ConfigureApplicationPartManager(manager =>
                    {
                        var featureProvider = new ServiceControllerFeatureProvider(new[] { typeof(ITestService) });
                        manager.FeatureProviders.Add(featureProvider);
                    });
                })
                .Configure(app => app.UseMvc())
                .Build()
                .Run();
            
        }
    }
}
