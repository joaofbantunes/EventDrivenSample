using System.Threading.Tasks;
using BurgerJoint.Events;
using BurgerJoint.Events.Kafka;
using BurgerJoint.StoreFront.Data;
using BurgerJoint.StoreFront.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BurgerJoint.StoreFront
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages(options => options.RootDirectory = "/Features");

            services.AddDbContext<BurgerDbContext>(
                options => options.UseNpgsql(
                    "server=localhost;port=5432;user id=user;password=pass;database=BurgerJointStoreFront"));

            services.AddSingleton<IOrderEventPublisher, KafkaOrderEventPublisher>();

            services.AddSingleton<OutboxMessagePublisher>();
            services.AddSingleton<IOutboxMessageListener>(s => s.GetRequiredService<OutboxMessagePublisher>());
            services.AddHostedService<OutboxHostedService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapFallback(ctx =>
                {
                    ctx.Response.Redirect("/Orders/Create");
                    return Task.CompletedTask;
                });
            });
        }
    }
}