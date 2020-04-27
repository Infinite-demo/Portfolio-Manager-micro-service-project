using Autofac;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Prometheus;
using UserManagement.Config;

namespace UserManagement
{
    public class Startup
    {
        public ILifetimeScope AutofacContainer { get; private set; }
        private ConfigurationSetting _configurationSetting;
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

            #region Swagger

            services.AddSwaggerGen(c =>
            {
                c.IncludeXmlComments(@"UserManagement.xml");
                c.SwaggerDoc("v1", new OpenApiInfo { Title = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, Version = "v1" });
            });

            #endregion Swagger

            services.AddOptions();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseConsul(_configurationSetting);

            app.UseRouting();
            app.UseHttpMetrics();

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
                endpoints.MapMetrics();
            });
        }
    }
}
