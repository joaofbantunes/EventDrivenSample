using System.Collections.Generic;
using System.Linq;
using BurgerJoint.Events;
using BurgerJoint.Events.Kafka;
using BurgerJoint.Operations.Domain;
using BurgerJoint.Operations.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BurgerJoint.Operations
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<EventListenerHostedService>();
            services.AddSingleton<IOrderEventHandler, DeliveryEfficiencyTrackingEventHandler>();
            services.AddSingleton(new KafkaOrderEventConsumerSettings
            {
                ConsumerGroup = "operations"
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