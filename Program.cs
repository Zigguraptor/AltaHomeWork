#pragma warning disable CS1591

using AltaHomeWork_1.Middleware;
using Microsoft.OpenApi.Models;

namespace AltaHomeWork_1;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddControllers();

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "API книжного магазина",
                Description = "API для доступа к списку книг",
                Version = "v1"
            });

            var xmlPath = Path.Combine(AppContext.BaseDirectory, "AltaHomeWork_1.xml");
            options.IncludeXmlComments(xmlPath);
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Visits API v1"); });
        }

        app.UseMiddleware<JsonValidatorMiddleware>();

        app.MapControllers();

        app.Run();
    }
}
