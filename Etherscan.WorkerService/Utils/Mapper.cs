using Etherscan.WorkerService.Dto;
using Etherscan.WorkerService.Model;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Text;

namespace Etherscan.WorkerService.Utils
{
    public class Mapper
    {
        public static Block MapBlock(BlockDto blockDto)
        {
            Block block = new Block();
            block.BlockNumber = Helper.HexadecimalToInt(blockDto.number);
            block.GasLimit = Helper.HexadecimalToInt(blockDto.gasLimit);
            block.GasUsed = Helper.HexadecimalToInt(blockDto.gasUsed);
            block.Hash = blockDto.hash;
            block.ParentHash = blockDto.parentHash;
            block.Miner = blockDto.miner;
            return block;
        }
        public static Transaction MapTransaction(TransactionDto transactionDto, int blockId)
        {
            Transaction transaction = new Transaction();
            transaction.BlockId = blockId;
            transaction.Hash = transactionDto.hash;
            transaction.From = transactionDto.from;
            transaction.To = transactionDto.to;
            transaction.Value = Helper.HexadecimalToBigNumber(transactionDto.value);
            transaction.Gas = Helper.HexadecimalToBigNumber(transactionDto.gas);
            transaction.GasPrice = Helper.HexadecimalToBigNumber(transactionDto.gasPrice);
            transaction.TransactionIndex = Helper.HexadecimalToInt(transactionDto.transactionIndex);
            return transaction;
        }
    }
}
