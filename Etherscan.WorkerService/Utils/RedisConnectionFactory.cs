using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.IO;
using System.Reflection;

namespace Etherscan.WorkerService.Utils
{
    public class RedisConnectionFactory
    {
        private static Lazy<ConnectionMultiplexer> Connection;
        private static readonly object SyncLock = new object();
        private static IConfigurationRoot _configRoot;
        static RedisConnectionFactory()
        {
            if (Connection == null)
            {
                lock (SyncLock)
                {
                    var configurationBuilder = new ConfigurationBuilder();
                    //var path = Path.Combine(AppContext.BaseDirectory); // get absolute path
                    var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    configurationBuilder.SetBasePath(path).AddJsonFile("appsettings.json", false);
                    _configRoot = configurationBuilder.Build();
                    var redisSection = new Redis();
                    _configRoot.Bind("RedisConfig", redisSection);
                    InitConnection(redisSection);
                }
            }
        }

        private static void InitConnection(Redis redisSection)
        {
            ConfigurationOptions options = new ConfigurationOptions();
            options.DefaultDatabase = redisSection.Database;
            options.SetDefaultPorts();
            options.EndPoints.Add(redisSection.Host, redisSection.Port);
            options.ReconnectRetryPolicy = new LinearRetry(3000);
            options.AbortOnConnectFail = false;

            if (!string.IsNullOrEmpty(redisSection.Password)) options.Password = redisSection.Password;
            Connection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(options));
        }

        public static ConnectionMultiplexer GetConnection() => Connection.Value;
    }
}
