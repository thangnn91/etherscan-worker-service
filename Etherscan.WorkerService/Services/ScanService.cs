using Dapper;
using Dapper.Contrib.Extensions;
using Etherscan.WorkerService.Dto;
using Etherscan.WorkerService.Model;
using Etherscan.WorkerService.Utils;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Z.Dapper.Plus;
using static Etherscan.WorkerService.Utils.Constant;

namespace Etherscan.WorkerService.Services
{
    public class ScanService: IScanService
    {
        private readonly ILogger<ScanService> _logger;
        private readonly IRedisDataAgent _redis;
        private readonly IOptionsMonitor<WorkerConfigs> _monitorWorkerConfigs;
        private readonly DapperContext _context;
        public ScanService(
            ILogger<ScanService> logger,
            IRedisDataAgent redis,
            IOptionsMonitor<WorkerConfigs> monitorWorkerConfigs,
            DapperContext context
            )
        {
            _logger = logger;
            _redis = redis;
            _context = context;
            _monitorWorkerConfigs = monitorWorkerConfigs;
            _monitorWorkerConfigs.OnChange((opts) =>
            { 
                System.Console.WriteLine("Config has been changed");
            });
        }

        public async Task ProcessData(CancellationToken stoppingToken)
        {
            
            if (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation(" ---- CancellationRequested ---- ");
                return;
            }
            bool hasError = false;
            string lastestBlockString = await _redis.GetStringValueAsync(Constant.Caches.CacheLastestKey);
            string scanConfigs = await _redis.GetStringValueAsync(Constant.Caches.CacheConfigScan);
            int startBlockToScan = _monitorWorkerConfigs.CurrentValue.From;
            
            //firstTime service is started
            if (string.IsNullOrEmpty(scanConfigs))
            {
                await _redis.SetStringValueAsync(Constant.Caches.CacheConfigScan, $"{_monitorWorkerConfigs.CurrentValue.From}:{_monitorWorkerConfigs.CurrentValue.To}");
                _logger.LogInformation("This is first time function is called");
            }
            //If nothing change, scan from lastest block
            else if(scanConfigs == $"{_monitorWorkerConfigs.CurrentValue.From}:{_monitorWorkerConfigs.CurrentValue.To}")
            {
                int lastestBlock = !string.IsNullOrEmpty(lastestBlockString) ? Convert.ToInt32(lastestBlockString) : Constant.InitBlock;
                if (lastestBlock > _monitorWorkerConfigs.CurrentValue.From && lastestBlock < _monitorWorkerConfigs.CurrentValue.To)
                {
                    startBlockToScan = lastestBlock + 1;
                }
                else if (lastestBlock == _monitorWorkerConfigs.CurrentValue.To)
                {
                    //sleeping
                    _logger.LogInformation("Finished scan blocks, waiting new configs");
                    await Task.Delay(_monitorWorkerConfigs.CurrentValue.DelayAfterFinished);
                    await ProcessData(stoppingToken);
                }
            }
            else //If configs are changed, scan from to in config file
            {
                await _redis.SetStringValueAsync(Constant.Caches.CacheConfigScan, $"{_monitorWorkerConfigs.CurrentValue.From}:{_monitorWorkerConfigs.CurrentValue.To}");
                _logger.LogInformation($"Config was changed");
                startBlockToScan = _monitorWorkerConfigs.CurrentValue.From;
            }

            _logger.LogInformation("Starting process data....");
            //Because api request is limited, we have to run one by one
            //If the number of requests is larger, we can use queue for parallel processing (Redis pub/sub, RabitMQ)
            for (int i = startBlockToScan; i <= _monitorWorkerConfigs.CurrentValue.To; i++)
            {
                (int blockId, int countTransaction) = await ProcessBlock(i);
                //If error, break
                if(blockId <= 0 && blockId!= ResponseCode.Processed)
                {
                    hasError = true;
                    _logger.LogInformation($"Error occurs at block number: {i}....");
                    break;
                }
                else if (_monitorWorkerConfigs.CurrentValue.ScanTx && blockId > 0)
                {
                    await ProcessTransaction(blockId, countTransaction, i.ToString("X"));
                }
            }
            if (hasError)
            {
                _logger.LogInformation($"Because of error, service has been sleeped");
                await Task.Delay(_monitorWorkerConfigs.CurrentValue.DelayIfError);
                await ProcessData(stoppingToken);
            }
            _logger.LogInformation("Finished process data....");
        }

        private async Task<(int, int)> ProcessBlock(int blockNumber)
        {
            try
            {
                if(await IsProcessed(blockNumber))
                {
                    _logger.LogInformation($"Block {blockNumber} was processed");
                    return (ResponseCode.Processed, 0);
                }

                string tagHex = blockNumber.ToString("X");
                var responseData = await $"{_monitorWorkerConfigs.CurrentValue.BaseUrl}?module=proxy&action=eth_getBlockByNumber&tag=0x{tagHex}&boolean=false&apikey={_monitorWorkerConfigs.CurrentValue.ApiKey}".GetJsonAsync<ResponseEtherScan<BlockDto>>();
                if (responseData != null && responseData?.result != null)
                {
                    _logger.LogInformation($"response scan block: {JsonConvert.SerializeObject(responseData)}");
                    var responseCountHex = await $"{_monitorWorkerConfigs.CurrentValue.BaseUrl}?module=proxy&action=eth_getBlockTransactionCountByNumber&tag=0x{tagHex}&boolean=false&apikey={_monitorWorkerConfigs.CurrentValue.ApiKey}".GetJsonAsync<ResponseEtherScan<string>>();
                    if (responseCountHex != null && responseCountHex?.result != null)
                    {
                        int countTransaction = Helper.HexadecimalToInt(responseCountHex.result);
                        _logger.LogInformation($"count tx: {countTransaction}");
                        if (countTransaction > 0)
                        {
                            const string sql = @"
                                SELECT blockId
                                FROM block
                                WHERE blockNumber = @blockNumber
                                LIMIT 1";
                            var param = new { blockNumber };
                            Block newBlock = Mapper.MapBlock(responseData.result);
                            //save block to db
                            int blockId = ResponseCode.Init;
                            using (var connection = _context.CreateConnection())
                            {
                                //double check exist
                                var existed = await connection.QuerySingleOrDefaultAsync<Block>(sql, param);
                                if (existed == null)
                                {
                                    blockId = await connection.InsertAsync(newBlock);
                                }
                                
                            }
                            await _redis.SetStringValueAsync(Constant.Caches.CacheLastestKey, blockNumber.ToString());
                            await _redis.SetStringValueAsync($"{Constant.Caches.CacheScaned}:{blockNumber}", "1");
                            _logger.LogInformation($"blockId: {blockId}, lastest executed block: {blockNumber}");
                            return (blockId, countTransaction);
                        }
                    }
                }
                return (ResponseCode.InvalidData, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return (ResponseCode.Exception, 0);
            }
            
        }

        private async Task ProcessTransaction(int blockId, int countTransaction, string tagHex)
        {
            try
            {
                if (countTransaction <= 0)
                    return;

                var listTransactions = new List<Transaction>();
                for (int i = 0; i < countTransaction; i++)
                {
                    try
                    {
                        string indexHex = i.ToString("X");
                        var responseTransaction = await $"{_monitorWorkerConfigs.CurrentValue.BaseUrl}?module=proxy&action=eth_getTransactionByBlockNumberAndIndex&tag=0x{tagHex}&index=0x{indexHex}&apikey={_monitorWorkerConfigs.CurrentValue.ApiKey}".GetJsonAsync<ResponseEtherScan<TransactionDto>>();
                        if (responseTransaction != null && responseTransaction?.result != null)
                        {
                            Transaction transaction = Mapper.MapTransaction(responseTransaction.result, blockId);
                            listTransactions.Add(transaction);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.ToString());
                    }
                }

                if (listTransactions.Count > 0)
                {
                    using (var connection = _context.CreateConnection())
                    {
                        connection.BulkInsert(listTransactions);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private async Task<bool> IsProcessed(int blockNumber)
        {
            string cacheBlock = await _redis.GetStringValueAsync($"{Constant.Caches.CacheScaned}:{blockNumber}");
            return !string.IsNullOrEmpty(cacheBlock);
        }
    }
}
