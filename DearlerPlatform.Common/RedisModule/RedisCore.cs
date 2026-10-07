using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace DearlerPlatform.Common.RedisModule
{
    /// <summary>
    /// Redis 核心类 连接并获取 Redis数据库
    /// </summary>
    public class RedisCore
    {
        public ConnectionMultiplexer Conn { get; set; }
        public IDatabase Db { get; set; }

        public RedisCore(IConfiguration configuration)
        {
            var redisConnectStr = configuration["Redis"];

            ConfigurationOptions configurationOptions = ConfigurationOptions.Parse(redisConnectStr);

            // configurationOptions.AllowAdmin=true; 使用SCAN模糊搜索时的可选项，不需要开启管理员模式

            Conn = ConnectionMultiplexer.Connect(configurationOptions);

            //得到 Redis DataBase
            Db = Conn.GetDatabase();
        }

    }
}