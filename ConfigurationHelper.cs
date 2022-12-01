using Microsoft.Extensions.Configuration;

namespace EthTestTask
{
    public static class ConfigurationHelper
    {
        public static IConfiguration GetConfiguration() => new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .Build();
    }
}