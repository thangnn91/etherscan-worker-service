using Etherscan.WorkerService.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Etherscan.WorkerService
{
    public class Console
    {
        private readonly IScanService _scanService;
        CancellationTokenSource _cancellationToken;
        public Console(IScanService scanService)
        {
            _scanService = scanService;
        }
        public async Task RunTest()
        {
            _cancellationToken = new CancellationTokenSource();
            await _scanService.ProcessData(_cancellationToken.Token);
            return;
        }
    }
}
