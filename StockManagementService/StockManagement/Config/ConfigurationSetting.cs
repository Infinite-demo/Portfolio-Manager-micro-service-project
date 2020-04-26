using System.Collections.Generic;

namespace StockManagement.Config
{
    public class ConfigurationSetting
    {
        public string ServiceName { get; set; }
        public string ServiceHost { get; set; }
        public int ServicePort { get; set; }
        public string ConsulAddresss { get; set; }
        public DatabaseSettings DatabaseSettings { get; set; }
        public RabbitMqConfiguration RabbitMqConfiguration { get; set; }
    }

    public partial class DatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public class RabbitMqConfiguration
    {
        public string VirtualHost { get; set; }
        public bool UseCluster { get; set; }
        public string ClusterName { get; set; }
        public List<string> HostNames { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
