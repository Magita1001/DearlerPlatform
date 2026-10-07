using DearlerPlatform.Common.RedisModule;
using DearlerPlatform.Core.Repository;
using DearlerPlatform.Domain;
using DearlerPlatform.Service.Models;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace DearlerPlatform.Web.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IRepository<Customer> _customerRepository;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IRepository<Customer> _customerRepository, IRedisWorker redisWorker)
    {
        _logger = logger;
        this._customerRepository = _customerRepository;
        RedisWorker = redisWorker;
    }

    public IRedisWorker RedisWorker { get; }

    // [HttpGet(Name = "GetWeatherForecast")]
    // public IEnumerable<WeatherForecast> Get()
    // {
    //     return Enumerable.Range(1, 5).Select(index => new WeatherForecast
    //     {
    //         Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
    //         TemperatureC = Random.Shared.Next(-20, 55),
    //         Summary = Summaries[Random.Shared.Next(Summaries.Length)]
    //     })
    //     .ToArray();       
    // }

    [HttpGet]
    public async Task<string> Get(string key)
    {
        return await RedisWorker.GetStringAsync(key);
    }
    [HttpPost]
    public void SetHash(TextHashViewModel hashViewModel)
    {
        List<UserInfo> userInfos = new List<UserInfo>()
        {
            new UserInfo(){ Id = 1, UserName = "Alice", Age=18},
            new UserInfo(){ Id = 2, UserName = "donk", Age=16},
            new UserInfo(){ Id = 3, UserName = "rossi", Age=15},
        };

        //TODO: 查询： SetHashMemory中对主键的约束是什么？
        RedisWorker.SetHashMemory(hashViewModel.Key, userInfos, m => new[] { m.Id.ToString(), m.UserName.ToString() });
    }
    [HttpGet]
    public List<UserInfo> GetHashEntries(string key)
    {
        return RedisWorker.GetHashMemory<UserInfo>(key);
    }

    public class UserInfo()
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public int Age { get; set; }
    }

}