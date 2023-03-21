using System;
using System.Collections.Generic;
using System.Text;

namespace Etherscan.WorkerService.Dto
{
    public class TransactionDto
    {
        public string hash { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string value { get; set; }
        public string gas { get; set; }
        public string gasPrice { get; set; }
        public string transactionIndex { get; set; }
    }
}
