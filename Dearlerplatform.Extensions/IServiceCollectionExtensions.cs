using System.Reflection;
using DearlerPlatform.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dearlerplatform.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection RepositoryRegister(this IServiceCollection services)
        {
            var assemblyCore = Assembly.Load("DearlerPlatform.Core");

            //查询泛型类Repository 这里的参数写法，专门用于查询泛型类， `1 意为有一个泛型参数的泛型类
            var implementationType = assemblyCore.GetTypes().FirstOrDefault(m => m.Name == "Repository`1");

            //根据泛型类找到它的接口，这里的GetGenericTypeDefinition是重点，他会把反射的IRepository<TEntity>改为IRepository<>
            var interfaceType = implementationType?.GetInterface("IRepository`1").GetGenericTypeDefinition();

            if (interfaceType != null && implementationType != null)
            {
                services.AddTransient(interfaceType, implementationType);
            }

            return services;

            //教程方案 需要在 Repository 上添加一个空的 IRepository
            // var asmService = Assembly.Load("DearlerPlatform.Service");
            // var implementationTypes = asmService.GetTypes().Where(
            //     m => m.IsAssignableTo(typeof(IRepository))
            //     && !m.IsAbstract
            //     && !m.IsInterface
            // );
            // foreach (var implementationType in implementationTypes)
            // {
            //     services.AddTransient(implementationType, implementationTypes);
            // }
            // return services;

            //教程的第二版方案
            // var assemblyCore = Assembly.Load("DearlerPlatform.Core");
            // //查询泛型类Repository 这里的参数写法，专门用于查询泛型类， `1 意为有一个泛型参数的泛型类
            // var implementationType = assemblyCore.GetTypes().FirstOrDefault(m => m.Name == "Repository`1");
            // var interfaceType = implementationType?.GetInterface("IRepository`1").GetGenericTypeDefinition();
            // services.AddTransient(typeof(IRepository<>), implementationType);
            // return services;
        }

        //注册其他服务，这里专门创建了一个用于标记的接口：IocTag 使用它来把需要的成员筛选出来
        public static IServiceCollection ServiceRegister(this IServiceCollection services)
        {
            List<Assembly> assemblies = new List<Assembly>();

            //通过service得到配置文件中的 被标记的 程序集名称
            var provider = services.BuildServiceProvider();
            var configuration = provider.GetService<IConfiguration>();
            List<string> iocClasses = configuration.GetSection("IocClasses").Get<List<string>>();

            #region 获取配置文件方案2
            // var provider = services.BuildServiceProvider();
            // var configuration = provider.GetService<IConfiguration>();
            // var iocClasses = configuration["IocClasses2"].Split(",").ToList();
            #endregion

            iocClasses.ForEach(c =>
            {
                assemblies.Add(Assembly.Load(c));
            });

            // var assemblyService = Assembly.Load("DearlerPlatform.Service");
            // var assemblyRedisWorker = Assembly.Load("DearlerPlatform.Common");

            // assemblies.Add(assemblyService);
            // assemblies.Add(assemblyRedisWorker);


            foreach (var assembly in assemblies)
            {
                var implementationTypes = assembly.GetTypes().Where(
                 m => m.IsAssignableTo(typeof(IocTag))
                 && !m.IsAbstract
                 && !m.IsInterface
                );

                foreach (var implementationType in implementationTypes)
                {
                    var interfaceType = implementationType?.GetInterfaces().Where(m => m != typeof(IocTag)).FirstOrDefault();
                    services.AddTransient(interfaceType, implementationType);
                }
            }

            return services;

        }

    }
}