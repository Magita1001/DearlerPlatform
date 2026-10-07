using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dearlerplatform.Extensions;
using DearlerPlatform.Common.TokenModule.Model;
using DearlerPlatform.Core;
using DearlerPlatform.Service;
using DearlerPlatform.Service.CustomerApp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace DearlerPlatform.Api.Extensions
{
    public static class WebApplicationBuilderExtension
    {
        public static void ServiceEnter(this IServiceCollection services)
        {
            services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            // services.AddOpenApi();

            //跨域处理
            services.AddCors(c => c.AddPolicy("Any", p => p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));

            //自己实现的获取配置文件方法
            var coniguration = services.GetConfiguration();
            var token = coniguration.GetSection("Jwt").Get<JwtTokenModel>();

            #region Jwt验证
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                        //想使用 原始字符串字面量 就使用这个方法，否则使用匿名对象
                        // WriteAsJsonAsync(new { code = 401, err = "无权限" });
                        await context.Response.WriteAsync(res);

                        // return Task.FromResult(0);
                    }
                };
            });
            #endregion


            services.AddDbContext<DealerPlatformContext>(opt =>
            {
                opt.UseSqlServer(coniguration.GetConnectionString("Default"));
            });
            services.AddAutoMapper(typeof(DearlerPlatformProfile));


            services.RepositoryRegister();
            services.ServiceRegister();
            services.AddTransient<ICustomerService, CustomerService>();

            services.AddSwaggerGen(c =>
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
        }

        public static IConfiguration GetConfiguration(this IServiceCollection services)
        {
            //6-2写的 会报错
            // var configuration = services.FirstOrDefault(d => d.ServiceType == typeof(IConfiguration)).ImplementationInstance;
            // return (IConfiguration)configuration;

            //不报错 但不推荐BuildServiceProvider
            var configration = services.BuildServiceProvider().GetService<IConfiguration>();
            return configration;
        }
    }
}