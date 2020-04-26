using Autofac;
using AutoMapper;
using EventBus;
using EventBus.Contracts;
using GreenPipes;
using MassTransit;
using MassTransit.RabbitMqTransport;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Service.Events;
using StockReport.Config;
using StockReport.EventHandlers;
using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace StockReport
{
    public class Startup
    {
        public IBusControl _busControl;
        private ConfigurationSetting _configurationSetting;
        public ILifetimeScope AutofacContainer { get; private set; }
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment env)
        {
            // In ASP.NET Core 3.0 `env` will be an IWebHostEnvironment, not IHostingEnvironment.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            this.Configuration = builder.Build();
        }

        // ConfigureServices is where you register dependencies. This gets
        // called by the runtime before the ConfigureContainer method, below.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add services to the collection. Don't build or return
            // any IServiceProvider or the ConfigureContainer method
            // won't get called.

            // load appsettings
            _configurationSetting = services.RegisterConfiguration(Configuration);
            services.AddConsulConfig(_configurationSetting);

            services.AddControllers();
            services.AddAutoMapper(typeof(Startup));
            services.RegisterDbDependancies(_configurationSetting);
            services.RegisterServiceDependancies(Configuration);

            RegisterEventDependancies(services);
            AddEventHandlers(services);

            #region Swagger

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, Version = "v1" });
            });

            #endregion Swagger

            services.AddOptions();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {

        }

        private IBusControl RabbitMqBusControl(IServiceProvider serviceProvider)
        {
            if (_busControl != null) return _busControl;

            _busControl = Bus.Factory.CreateUsingRabbitMq(config =>
            {
                config.UseCircuitBreaker(cb =>
                {
                    cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                    cb.TripThreshold = 15;
                    cb.ActiveThreshold = 10;
                    cb.ResetInterval = TimeSpan.FromMinutes(5);
                });

                var clusterName = _configurationSetting.RabbitMqConfiguration.UseCluster ? _configurationSetting.RabbitMqConfiguration.ClusterName :
                _configurationSetting.RabbitMqConfiguration.HostNames.First();


                var host = config.Host(new Uri($"rabbitmq://{clusterName}/" +
                    $"{_configurationSetting.RabbitMqConfiguration.VirtualHost}"), h =>
                    {
                        h.Username(_configurationSetting.RabbitMqConfiguration.UserName);
                        h.Password(_configurationSetting.RabbitMqConfiguration.Password);
                        if (_configurationSetting.RabbitMqConfiguration.UseCluster)
                        {
                            h.UseCluster(c =>
                            {
                                foreach (var node in _configurationSetting.RabbitMqConfiguration.HostNames)
                                    c.Node(node);
                            });
                        }
                    });

                config.AutoDelete = false;
                config.Durable = true;
                RegisterReceiveEndpoints(serviceProvider, config);
            });

            return _busControl;
        }

        private void RegisterReceiveEndpoints(IServiceProvider serviceProvider, IRabbitMqBusFactoryConfigurator config)
        {
            config.ReceiveEndpoint(getUniqueName(typeof(StockUpdateEvent).Name), ep =>
            {
                ep.UseRetry(rc =>
                {
                    rc.Exponential(5, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(25), TimeSpan.FromMinutes(5));
                    ep.Consumer(typeof(IConsumer<StockUpdateEvent>), type => serviceProvider.GetService<IConsumer<StockUpdateEvent>>());
                    ep.Handler<StockUpdateEvent>(MessageHandler);
                });
            });
        }

        private Task MessageHandler<T>(T obj)
        {
            return Task.CompletedTask;
        }

        private void RegisterEventDependancies(IServiceCollection services)
        {
            services.AddSingleton<IHostedService, HostedBackgroundService>();
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            services.AddSingleton<IPublishEndpoint>(sp => sp.GetService<IBusControl>());
            services.AddTransient<IEventBus, EventBusService>();
            services.AddSingleton(RabbitMqBusControl);
            services.AddTransient(typeof(IConsumer<>), typeof(EventConsumer<>));
        }

        private static string getUniqueName(string eventName)
        {
            string hostName = Dns.GetHostName();
            string callingAssembly = Assembly.GetCallingAssembly().GetName().Name;
            return $"{hostName}.{callingAssembly}.{eventName}";
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseConsul(_configurationSetting);

            ConfigureEventBus(app);

            app.UseRouting();

            app.UseAuthorization();

            #region Swagger

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Swagger Movies Demo V1");
            });

            #endregion Swagger

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.Subscribe<StockUpdateEvent, StockUpdateEventHandler>();
        }

        public void AddEventHandlers(IServiceCollection services)
        {
            services.AddTransient<StockUpdateEventHandler>();
        }
    }
}
