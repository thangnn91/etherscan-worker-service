using System;
using System.Collections.Generic;
using System.Text;

namespace Etherscan.WorkerService.Utils
{
    public class Redis
    {
        public int Database { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Password { get; set; }
    }
    public class WorkerConfigs
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public bool ScanTx { get; set; }
        public int DelayIfError { get; set; }
        public int DelayAfterFinished { get; set; }
        
    }

    public class DbConfigs
    {
        public string ConnectString { get; set; }
    }

}
