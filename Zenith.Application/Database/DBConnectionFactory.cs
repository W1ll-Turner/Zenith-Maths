using Npgsql;
using System.Data;
namespace Zenith.Application.Database;

//creating a interface so that they Class can be loosely coupled
public interface IDBConnectionFactory 
{
    Task<IDbConnection> CreateDBConnection();
}

/**implemetning the interface to create a db connection factory, this will allow for databse connections to be opened
 with ease and so that querys can be kept isolated 
 */
public class NpgsqlConnectionFactory : IDBConnectionFactory 
{
    
    private readonly string _connectionString;
    public NpgsqlConnectionFactory(string connectionString) //getting the DB Connection string from the JSON config 
    {
        _connectionString = connectionString;
    }
    public async Task<IDbConnection> CreateDBConnection()
    {
        //initialisng the connection, opening it and returning it, the conecion will be closed automatically bu the repository code 
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(); 
        return connection;
    }
}