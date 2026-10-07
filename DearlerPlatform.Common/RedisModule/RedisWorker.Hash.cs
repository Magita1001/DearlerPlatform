using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace DearlerPlatform.Common.RedisModule
{
    public partial class RedisWorker : IRedisWorker
    {
        /// <summary>
        /// 存储Hash键值
        /// </summary>
        public void SetHashMemory(string key, Dictionary<string, string> values)
        {
            var hashEntrys = new List<HashEntry>();
            foreach (var value in values)
            {
                hashEntrys.Add(new HashEntry(value.Key, value.Value));
            }

            SetHashMemory(key, hashEntrys.ToArray());
        }
        public void SetHashMemory(string key, params HashEntry[] entries)
        {
            RedisCore.Db.HashSet(key, entries);
        }

        public void SetHashMemory<T>(string key, T entity, Type type = null)
        {
            type ??= typeof(T);

            List<HashEntry> hashEntries = new List<HashEntry>();
            PropertyInfo[] props = type.GetProperties();

            foreach (var prop in props)
            {
                string name = prop.Name;
                object value = prop.GetValue(entity);

                if (value is bool)
                {
                    value = (bool)value ? 1 : 0;
                }
                hashEntries.Add(new HashEntry(name, value?.ToString()));
            }
            SetHashMemory(key, hashEntries.ToArray());
        }

        /// <summary>
        /// 联合组件 key 的插入
        /// </summary>
        public void SetHashMemory<T>(string key, IEnumerable<T> entries, Func<T, IEnumerable<string>> func)
        {
            #region 初版完整方案及签名
            // //  public void SetHashMemory<T>(string key, IEnumerable<T> entries, Func<T, IEnumerable<string>> func)

            // Type type = typeof(T);

            // foreach (var entity in entries)
            // {
            //     List<HashEntry> hashEntries = new List<HashEntry>();
            //     PropertyInfo[] props = type.GetProperties();

            //     foreach (var prop in props)
            //     {
            //         string name = prop.Name;
            //         object value = prop.GetValue(entity);

            //         // if (value.GetType().Name == "Boolean")
            //         if (value is bool)
            //         {
            //             value = (bool)value ? 1 : 0;
            //         }

            //         hashEntries.Add(new HashEntry(name, value?.ToString()));
            //     }

            //     //这里的委托是返回一个列表，作为联合key的字符串
            //     var valueKeys = func(entity);
            //     SetHashMemory($"{key}:{string.Join(":", valueKeys)}", hashEntries.ToArray());
            // }
            #endregion

            Type type = typeof(T);

            foreach (var entity in entries)
            {
                //这里的委托是返回一个列表，作为联合key的字符串索引
                var valueKeys = func(entity);
                SetHashMemory($"{key}:{string.Join(":", valueKeys)}", entity, type);
            }
        }

        // public List<HashEntry> GetHashMemory(string key)
        // {
        //     var res = RedisCore.Db.HashGetAll(key);
        //     var list = res.ToList();

        //     return list;
        // }

        public List<T> GetHashMemory<T>(string keyLike) where T : new()
        {
            var keys = GetKeys(keyLike);
            List<T> values = new List<T>();
            foreach (var key in keys)
            {
                T t = new T();
                //这里拿到的是一个HashEntry集合 通过循环拿到里面的 kv
                var res = RedisCore.Db.HashGetAll(key);

                var props = t.GetType().GetProperties();
                foreach (var item in res)
                {
                    foreach (var prop in props)
                    {
                        if (prop.Name == item.Name)
                        {
                            prop.SetValue(t, Convert.ChangeType(item.Value, prop.PropertyType));
                            break;
                        }
                    }
                }
                values.Add(t);
            }
            return values;
        }
    }
}