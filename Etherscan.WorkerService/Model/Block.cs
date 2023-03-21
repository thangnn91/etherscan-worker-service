using System.ComponentModel.DataAnnotations.Schema;

namespace Etherscan.WorkerService.Model
{
    [Table("block")]
    public class Block
    {
        [Column("blockID")]
        public int BlockId { get; set; }
        [Column("blockNumber")]
        public int BlockNumber { get; set; }
        [Column("hash")]
        public string Hash { get; set; }
        [Column("parentHash")]
        public string ParentHash { get; set; }
        [Column("miner")]
        public string Miner { get; set; }
        [Column("blockReward")]
        public decimal BlockReward { get; set; }
        [Column("gasLimit")]
        public decimal GasLimit { get; set; }
        [Column("gasUsed")]
        public decimal GasUsed { get; set; }
    }
}
