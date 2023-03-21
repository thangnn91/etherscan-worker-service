using System;
using System.Collections.Generic;
using System.Text;

namespace Etherscan.WorkerService.Dto
{
    public class BlockDto
    {
        public string number { get; set; }
        public string gasLimit { get; set; }
        public string gasUsed { get; set; }
        public string hash { get; set; }
        public string parentHash { get; set; }
        public string miner { get; set; }
    }
}
