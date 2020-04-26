using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDbClient;
using StockManagement.Persistence;
using StockManagement.Service;

namespace StockManagement.Config
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
            services.AddSingleton<IStockManagementService, StockManagementService>();
            services.AddSingleton<IStockDetailRepository, StockDetailRepository>();
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
