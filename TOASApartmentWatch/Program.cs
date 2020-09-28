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
        static void Main(string[] args)
        {
            Console.WriteLine("TOAS apartment watch starting");

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
