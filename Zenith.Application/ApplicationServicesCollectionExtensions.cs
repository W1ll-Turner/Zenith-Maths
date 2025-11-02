using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Zenith.Application.Database;
using Zenith.Application.Hashing;
using Zenith.Application.Repository;


namespace Zenith.Application;

public static class ApplicationServicesCollectionExtensions //This is used to inject the dependancies into the API, such as the data rereival code
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<IAccountRepo, AccountRepo>();
        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDBConnectionFactory>(_ => new NpgsqlConnectionFactory(connectionString));
        return services;
    }

    public static IServiceCollection AddHashing(this IServiceCollection services)
    {
        services.AddSingleton<IHashing, Hashing.Hashing>();
        return services;
    }
}