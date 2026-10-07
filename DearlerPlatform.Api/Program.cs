
using DearlerPlatform.Api.Extensions;

namespace DearlerPlatform.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.ServiceEnter();

        var app = builder.Build();

        app.InitEnter();
        app.InitMap();

        app.Run();
    }
}
