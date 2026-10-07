using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DearlerPlatform.Common.RedisModule
{
    public partial class RedisWorker : IRedisWorker
    {
        public RedisWorker(RedisCore redisCore)
        {
            RedisCore = redisCore;
        }
        public RedisCore RedisCore { get; }

        /// <summary>
        /// 通过 SCAN 获取所有的带通配的 Key
        /// </summary>
        /// <param name="key">可以带通配符(*) 的 Key</param>
        /// <returns></returns>
        public List<string> GetKeys(string key)
        {
            List<string> keyList = new List<string>();
            // var endPoints = RedisCore.Conn.GetEndPoints().First();
            var endPoints = RedisCore.Conn.GetEndPoints();
            //代表某一台物理 Redis 服务器的网络终端标识（IP + 端口）。用于传给GetServer
            var endPoint = endPoints[0];
            //根据具体的 endpoint 地址，获取对应 Redis 节点的服务器管理接口（IServer）。
            var server = RedisCore.Conn.GetServer(endPoint);
            //通过 Server 得到所有符合条件的 Key
            var keys = server.Keys(0, key).ToList();
            keys.ForEach(k =>
            {
                keyList.Add(k);
            });

            return keyList;
        }
        public void RemoveKey(string key)
        {
            RedisCore.Db.KeyDelete(key);
        }
    }
}