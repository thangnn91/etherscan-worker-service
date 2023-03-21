using System.ComponentModel.DataAnnotations.Schema;

namespace Etherscan.WorkerService.Model
{
    [Table("transaction")]
    public class Transaction
    {
        [Column("transactionID")]
        public int TransactionID { get; set; }
        [Column("blockID")]
        public int BlockId { get; set; }
        [Column("hash")]
        public string Hash { get; set; }
        [Column("from")]
        public string From { get; set; }
        [Column("to")]
        public string To { get; set; }
        [Column("value")]
        public decimal Value { get; set; }
        [Column("gas")]
        public decimal Gas { get; set; }
        [Column("gasPrice")]
        public decimal GasPrice { get; set; }
        [Column("transactionIndex")]
        public int TransactionIndex { get; set; }
    }
}
