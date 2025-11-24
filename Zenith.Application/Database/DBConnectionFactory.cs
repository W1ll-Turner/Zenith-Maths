using Npgsql;
using System.Data;


namespace Zenith.Application.Database;


public interface IDBConnectionFactory //creating a interface so that they Class can be loosely coupled
{
    Task<IDbConnection> CreateDBConnection();
}

public class NpgsqlConnectionFactory : IDBConnectionFactory //Implementing the the Interface
{
    private readonly string _connectionString;
    
    public NpgsqlConnectionFactory(string connectionString) //getting the DB Connection string
    {
        _connectionString = connectionString;
    }

    public async Task<IDbConnection> CreateDBConnection()
    {
        //initialisng the connection to the database and returning it 
        var connection = new NpgsqlConnection(_connectionString);
        //Asynchronus as postgres supports this and will allow for multiple qurys to be executed simulatnaously
        await connection.OpenAsync(); 
        return connection;
    }
    
}