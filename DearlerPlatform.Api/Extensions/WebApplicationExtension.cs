using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DearlerPlatform.Api.Extensions
{
    public static class WebApplicationExtension
    {
        public static void InitEnter(this IApplicationBuilder app)
        {
            app.UseAuthentication();

            app.UseCors("Any");

            // var app = builder.Build();
            // Configure the HTTP request pipeline.

            // app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthorization();

        }
        public static void InitMap(this IEndpointRouteBuilder app)
        {
            app.MapControllers();
        }
    }
}