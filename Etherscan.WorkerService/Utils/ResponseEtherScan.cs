using System;
using System.Collections.Generic;
using System.Text;

namespace Etherscan.WorkerService.Utils
{
    public class ResponseEtherScan<T>
    {
        public string jsonrpc { get; set; }
        public int id { get; set; }
        public T result { get; set; }
    }
}
