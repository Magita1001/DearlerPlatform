using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DearlerPlatform.Common.RedisModule
{


    public partial class RedisWorker : IRedisWorker
    {        
        public void SetString(string key, string value, TimeSpan ts)
        {
            RedisCore.Db.StringSet(key, value, ts);
        }
        public async Task SetStringAsync(string key, string value, TimeSpan ts)
        {
            await RedisCore.Db.StringSetAsync(key, value, ts);
        }
        public void GetString(string key)
        {
            RedisCore.Db.StringGet(key);
        }
        public async Task<string> GetStringAsync(string key)
        {
            return await RedisCore.Db.StringGetAsync(key);
        }
    }
}