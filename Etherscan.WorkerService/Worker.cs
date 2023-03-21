using Etherscan.WorkerService.Services;
using Etherscan.WorkerService.Utils;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Etherscan.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IRedisDataAgent _redis;
        private readonly IScanService _scanService;
        CancellationTokenSource _cancellationToken;
        public Worker(ILogger<Worker> logger, IRedisDataAgent redis, IScanService scanService)
        {
            _logger = logger;
            _redis = redis;
            _scanService = scanService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(5000, stoppingToken);
            }
        }

        public override Task StartAsync(CancellationToken stoppingToken)
        {
            _cancellationToken = new CancellationTokenSource();
            Task.Run(() => { _scanService.ProcessData(_cancellationToken.Token); });
            return Task.CompletedTask;
        }
        public override Task StopAsync(CancellationToken stoppingToken)
        {
            _cancellationToken.Cancel();
            _logger.LogInformation("------- Stop Service ------");
            return Task.CompletedTask;
        }

        public Task ConsoleTest()
        {
            _cancellationToken = new CancellationTokenSource();
            Task.Run(() => { _scanService.ProcessData(_cancellationToken.Token); });
            return Task.CompletedTask;
        }
    }
}
