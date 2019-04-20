﻿using AuthWebAppNetCore.Web.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace AuthWebAppNetCore.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //CreateWebHostBuilder(args).Build().Run();

            /////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////// Change Configuration to use Seed
            IWebHost host = CreateWebHostBuilder(args).Build();
            RunSeeding(host);
            host.Run();
            /////////////////////////////////////////////////////////////////////////////////////
        }

        /////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////// Call SeedDB
        private static void RunSeeding(IWebHost host)
        {
            IServiceScopeFactory scopeFactory = host.Services.GetService<IServiceScopeFactory>();
            using (IServiceScope scope = scopeFactory.CreateScope())
            {
                SeedDb seeder = scope.ServiceProvider.GetService<SeedDb>();
                seeder.SeedAsync().Wait();
            }
        }
        /////////////////////////////////////////////////////////////////////////////////////

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}