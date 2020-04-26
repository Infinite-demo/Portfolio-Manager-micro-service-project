using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDbClient;
using StockReport.Persistence;
using StockReport.Service;

namespace StockReport.Config
{
    public static class StartupConfig
    {
        public static ConfigurationSetting RegisterConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ConfigurationSetting>(configuration.GetSection("Configuration"));
            var serviceProvider = services.BuildServiceProvider();
            var iop = serviceProvider.GetService<IOptions<ConfigurationSetting>>();
            return iop.Value;
        }

        public static void RegisterServiceDependancies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IStockReportService, StockReportService>();
            services.AddSingleton<IStockReportRepository, StockReportRepository>();
            services.AddSingleton<IMongoDbQueryRepository, MongoDbQueryRepository>();
        }

        public static void RegisterDbDependancies(this IServiceCollection services, ConfigurationSetting configurationSetting)
        {
            services.AddSingleton<IDatabaseContext>(new DatabaseContext(
                configurationSetting.DatabaseSettings.ConnectionString,
                configurationSetting.DatabaseSettings.DatabaseName));
        }
    }
}
