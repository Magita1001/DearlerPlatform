using System.Text;
using Dearlerplatform.Extensions;
using DearlerPlatform.Common.EventBusHelper;
using DearlerPlatform.Common.RedisModule;
using DearlerPlatform.Common.TokenModule.Model;
using DearlerPlatform.Core;
using DearlerPlatform.Service;
using DearlerPlatform.Service.CustomerApp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace DearlerPlatform.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        // builder.Services.AddOpenApi();

        //全权使用Swagger 并添加鉴权模块
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo()
            {
                Title = "DearlerPlatform.Web",
                Version = "v1"
            });

            //鉴权模块 添加安全定义
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Description = "格式:Bearer{token}",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });

            //添加安全要求
            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme()
                    {
                        Reference=new OpenApiReference()
                        {
                            Type= ReferenceType.SecurityScheme,
                            Id="Bearer"
                        },
                    },new string[]{}
                },
            });
        });

        //跨域处理
        builder.Services.AddCors(c => c.AddPolicy("Any", p => p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));

        //依赖注入区
        var connectionString = builder.Configuration.GetConnectionString("Default");
        builder.Services.AddDbContext<DealerPlatformContext>(opt =>
        {
            opt.UseSqlServer(connectionString);
        });

        //注入泛型类的方式，在方法的参数里，而非泛型里进行注入
        // builder.Services.AddTransient(typeof(IRepository<>), typeof(Repository<>));

        //使用反射形式替换注册 IRepository 和其他 标记了 IocTag 的类
        builder.Services.RepositoryRegister();
        builder.Services.ServiceRegister();

        builder.Services.AddScoped(typeof(LocalEventBus<>));
        // builder.Services.AddTransient<ICustomerService, CustomerService>();
        
        builder.Services.AddSingleton<RedisCore>();
        builder.Services.AddTransient<IRedisWorker, RedisWorker>();

        //注册automapper 如果是14.x以上版本，会强制要求第一个参数传入官网注册的apiKey
        builder.Services.AddAutoMapper(typeof(DearlerPlatformProfile));

        var token = builder.Configuration.GetSection("Jwt").Get<JwtTokenModel>();

        #region Jwt验证
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(opt =>
        {
            //验证地址是否为Https，默认true
            opt.RequireHttpsMetadata = false;
            opt.SaveToken = true;
            opt.TokenValidationParameters = new TokenValidationParameters()
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(token.Security)),
                ValidIssuer = token.Issuer,
                ValidAudience = token.Audience
            };
            opt.Events = new JwtBearerEvents()
            {
                OnChallenge = async context =>
                {
                    //此处终止代码
                    context.HandleResponse();
                    string res = """{"code":401,"err":"无权限"}""";
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                    // await context.Response.WriteAsJsonAsync(res);

                    // res 里想使用 原始字符串字面量 就使用这个方法，否则使用匿名对象
                    // WriteAsJsonAsync(new { code = 401, err = "无权限" });

                    await context.Response.WriteAsync(res);

                    // return Task.FromResult(0);
                }
            };
        });
        #endregion


        //App区
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            // app.MapOpenApi();

            // app.UseSwaggerUI(opt =>
            // {
            //     opt.SwaggerEndpoint("/openapi/v1.json", "v1.json");
            // });

            //使用微软的api测试工具
            // app.MapScalarApiReference();

            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthentication();

        app.UseCors("Any");

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}