using System;
using System.Threading.Tasks;

namespace Etherscan.WorkerService.Utils
{
    public interface IRedisDataAgent
    {
        [Obsolete("Using async method to improve performance", true)]
        string GetStringValue(string key);
        Task<string> GetStringValueAsync(string key);
        [Obsolete("Using async method to improve performance", true)]
        void SetStringValue(string key, string value, int? expireSecond = null);
        Task SetStringValueAsync(string key, string value, int? expireSecond = null);
        [Obsolete("Using async method to improve performance", true)]
        void DeleteStringValue(string key);
        Task DeleteStringValueAsync(string key);

    }
}
