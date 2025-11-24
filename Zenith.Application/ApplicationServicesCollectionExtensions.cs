using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Zenith.Application.Database;
using Zenith.Application.Hashing;
using Zenith.Application.Repository;
namespace Zenith.Application;
public static class ApplicationServicesCollectionExtensions //This is used to inject the dependancies into the API, such as the data rereival code
{
    //this is used in program.cs to add the services to the project when run, allowing for them to be loosely coupled
    public static IServiceCollection AddApplicationServices(this IServiceCollection services) 
    {
        services.AddSingleton<IAccountRepo, AccountRepo>(); //adding the account repository
        services.AddSingleton<IQuestionStatisticsRepo, QuestionStatisticsRepo>(); //adding the question statistics repository 
        return services;
    }
    //adding the database to the services so it can be loosely coupled to the project
    public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDBConnectionFactory>(_ => new NpgsqlConnectionFactory(connectionString));
        return services;
    }
    //allowing for the hasing algorithms to be add to the services and thereforte can be loosley coupled into the repository
    public static IServiceCollection AddHashing(this IServiceCollection services)
    {
        services.AddSingleton<IHashing, Hashing.Hashing>();
        return services;
    }
}