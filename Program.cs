using Microsoft.Extensions.Configuration;
using EthTestTask.Detecters;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Logging;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace EthTestTask
{
    class Program
    {
        public static async Task Main()
        {
            var config = ConfigurationHelper.GetConfiguration();
            var configuration = config.GetSection("detectorConfiguration").Get<DetectorConfiguration>();
            var loggerFactory = GetFactory(config);
            var cancellationToken = new CancellationTokenSource();

            var erc20Detector = new ERC20TokenDetector(configuration, loggerFactory);
            var ethDetector = new EthTransferDetector(configuration, loggerFactory);
            var sDetector = new TransferFromContractDetector(configuration, loggerFactory);

            var task1 = erc20Detector.Detect(cancellationToken.Token);
            var task2 = ethDetector.Detect(cancellationToken.Token);
            var task3 = sDetector.Detect(cancellationToken.Token);

            Console.ReadLine();
            cancellationToken.Cancel();
            await task1;
            await task2;
            await task3;
        }

        private static ILoggerFactory GetFactory(IConfiguration configuration)
        {
            var serilogLogger = new LoggerConfiguration()
               .ReadFrom.Configuration(configuration).CreateLogger();
            //var logger = new SerilogLoggerFactory(serilogLogger).CreateLogger<Program>();
            return new LoggerFactory(new List<ILoggerProvider> { new SerilogLoggerProvider(serilogLogger) });
            //logger.LogInformation("The app started");
            //return logger;
        }
    }
}