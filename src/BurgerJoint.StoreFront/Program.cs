using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BurgerJoint.StoreFront.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BurgerJoint.StoreFront
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var db = scope
                    .ServiceProvider
                    .GetRequiredService<BurgerDbContext>();
                db
                    .Database
                    .EnsureCreated();

                if (!db.Dishes.Any())
                {
                    db.Dishes.AddRange(
                        new Dish(Guid.NewGuid(), "Bland", "Just burger and bread!"),
                        new Dish(Guid.NewGuid(), "Cheese", "Bland plus cheese"),
                        new Dish(Guid.NewGuid(), "A lot of cheese", "Remember cheese? Yeah, it's the same, but with a lot of cheese!")
                    );

                    db.SaveChanges();
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}