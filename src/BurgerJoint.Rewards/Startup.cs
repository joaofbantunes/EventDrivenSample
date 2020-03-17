using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BurgerJoint.Events;
using BurgerJoint.Events.Kafka;
using BurgerJoint.Rewards.Data;
using BurgerJoint.Rewards.Domain;
using BurgerJoint.Rewards.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BurgerJoint.Rewards
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<RewardsDbContext>(
                options => options.UseNpgsql(
                    "server=localhost;port=5432;user id=user;password=pass;database=BurgerJointRewards"));
            
            services.AddHostedService<EventListenerHostedService>();
            services
                .AddScoped<IOrderEventHandler, RewardsProgramEventHandler>()
                .Decorate<IOrderEventHandler, OrderEventHandlerIdempotenceDecorator>();
            services.AddSingleton(new KafkaOrderEventConsumerSettings
            {
                ConsumerGroup = "rewards"
            });
            services.AddSingleton<IOrderEventConsumer, KafkaOrderEventConsumer>();
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
                endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello World!"); });
            });
        }
    }
}