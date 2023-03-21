using Etherscan.WorkerService.Services;
using Etherscan.WorkerService.Utils;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.Globalization;
using System.IO;

namespace Etherscan.WorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Fatal)
            .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Fatal)
            .MinimumLevel.Override("Grpc.Core.Interceptors", LogEventLevel.Fatal)
            .Enrich.FromLogContext().WriteTo.File(AppContext.BaseDirectory + "\\LOGS\\log.txt", rollingInterval: RollingInterval.Day).CreateLogger();

            try
            {

                //Run as bg service
                //CreateHostBuilder(args).Build().Run();
                //Run as console app
                var a = Int64.Parse("1158e460913d00000", NumberStyles.HexNumber);
                _ = Host.CreateDefaultBuilder().ConfigureServices(ConfigureServices).Build().Services
                        .GetService<Console>()
                        .RunTest();
                System.Console.ReadKey();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "");
            }
            finally { Log.CloseAndFlush(); }
        }

        private static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
        {
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(AppContext.BaseDirectory, @"appsettings.json"); // get absolute path
            configurationBuilder.AddJsonFile(path, optional: false, reloadOnChange: true);
            IConfigurationRoot configRoot = configurationBuilder.Build();
            services.Configure<WorkerConfigs>(configRoot.GetSection("WorkerConfigs"));
            services.Configure<DbConfigs>(configRoot.GetSection("DbConfigs"));

            services.AddSingleton<IRedisDataAgent, RedisDataAgent>();
            services.AddSingleton<DapperContext>();
            services.AddHostedService<Worker>();
            services.AddSingleton<IScanService, ScanService>();
            services.AddSingleton<Console>();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .UseSerilog()
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    var builder = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddEnvironmentVariables().Build();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;
                    services.Configure<WorkerConfigs>(configuration.GetSection("WorkerConfigs"));
                    services.AddSingleton<IRedisDataAgent, RedisDataAgent>();
                    services.AddSingleton<DapperContext>();
                    services.AddHostedService<Worker>();

                    services.AddSingleton<IScanService, ScanService>();

                });
    }
}
