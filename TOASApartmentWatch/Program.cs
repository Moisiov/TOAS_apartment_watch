using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TOASApartmentWatch.Models.Options;
using Telegram.Bot;
using Microsoft.AspNetCore.Hosting;

namespace TOASApartmentWatch
{
    class Program
    {
        private static IServiceProvider _serviceProvider;
        static void Main(string[] args)
        {
            Console.WriteLine("TOAS apartment watch starting");

            /*RegisterServices();

            ApartmentWatch apartmentWatch = new ApartmentWatch(_serviceProvider);
            await apartmentWatch.run();*/

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void RegisterServices()
        {
            var services = new ServiceCollection();
            var configurationBuilder = new ConfigurationBuilder();
            var options = configurationBuilder.AddJsonFile("options.json", false, true)
                .Build();
            services.AddSingleton<IConfiguration>(options);

            _serviceProvider = services.BuildServiceProvider(true);
        }
    }
}
