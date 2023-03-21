using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Etherscan.WorkerService.Services
{
    public interface IScanService
    {
        Task ProcessData(CancellationToken stoppingToken);
    }
}
