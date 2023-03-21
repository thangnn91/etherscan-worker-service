using System;
using System.Collections.Generic;
using System.Text;

namespace Etherscan.WorkerService.Utils
{
    public class Constant
    {
        public static class Caches
        {
            public const string CacheLastestKey = "CACHE_LASTEST";
            public const string CacheScaned = "CACHE_SCANED";
            public const string CacheConfigScan = "CACHE_CONFIG_SCAN";
        }
        public static class ResponseCode
        {
            public const int Exception = -99;
            public const int Success = 1;
            public const int InvalidData = -600;
            public const int Init = 0;
            public const int Processed = -1;
        }

        public static int InitBlock = 0;
    }
}
