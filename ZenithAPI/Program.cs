using Zenith.Application;
namespace ZenithAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var config = builder.Configuration;  //importing the app settings
        // Add services to the container.
        builder.Services.AddHashing();
        builder.Services.AddDatabase(config["Database:ConnectionString"]); 
        builder.Services.AddControllers();
        builder.Services.AddApplicationServices(); //adding the repositroies to the application services, this is so that the API controller is able to use the data access methods

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        
        app.UseAuthorization();
        
        app.Run();
    }
}