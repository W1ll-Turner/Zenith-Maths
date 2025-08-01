using Npgsql;
using Zenith.Application.Database;
using Zenith.Models.Account;

namespace Zenith.Application.Repository;

public class AccountRepo : IAccountRepo //inheritance, this is the implmentation of the IAccountRepo to implement the data collection methods
{

    private readonly IDBConnectionFactory _dbConnection;

    public AccountRepo(IDBConnectionFactory dbConnection) //loosely coupling the DbConnection to the Repo
    {
        _dbConnection = dbConnection;
    }
    
    public async Task<bool> CreateAccount(SignUp account) //add duplicate account protection
    {
        await using (var connection = (NpgsqlConnection)await _dbConnection.CreateDBConnection())
        {
            try
            {
                var command = new NpgsqlCommand("INSERT INTO students (studentid, email, username, fullname, password, classcode) VALUES ( @student, @email,  @username,  @fullname, @password, @classcode)", connection);
                int id = await CreateId();
                
                //using parameters for the SQL query to prevent agaisnt injection attacks
                command.Parameters.AddWithValue("@student", id); 
                command.Parameters.AddWithValue("@email", account.Email);
                command.Parameters.AddWithValue("@username", account.Username);
                command.Parameters.AddWithValue("@fullname", account.Fullname);
                command.Parameters.AddWithValue("@password", account.Password);
                //this will add a class code to the student depedning on if they are the member of a class or not 
                if (account.ClassCode != null) 
                {
                    command.Parameters.AddWithValue("@classcode", account.ClassCode);
                }
                else
                {
                    command.Parameters.AddWithValue("@classcode", DBNull.Value);
                }

                await command.ExecuteNonQueryAsync(); //executing the query to add the data to the database
                Console.WriteLine("data added");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
            
            
            
        }
        
    }

    public async Task<int> LogIn(LogIn account) //this will be used to check if the account exists and if so it returns the id, this will be used for the authentication token
    {
        await using (var connection = (NpgsqlConnection)await _dbConnection.CreateDBConnection())
        {
            var command = new NpgsqlCommand("SELECT studentid FROM students WHERE password = @password AND username = @username", connection);
            command.Parameters.AddWithValue("@username", account.Username);
            command.Parameters.AddWithValue("@password", account.Password);
            try
            {
                await using (var reader = await command.ExecuteReaderAsync())
                {
                    await reader.ReadAsync();
                    int id = reader.GetInt32(0);
                    return id;


        
                }
            }
            catch
            {
                return 0; //no account will have ID 0 so this can be checked to say that it did not work
            }
            
            
        }
        
    }

    public Task<account> GetAccountInfo(string accountId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAccount(SignUp account)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAccount(string accountId)
    {
        throw new NotImplementedException();
    }
    
    private async Task<int> CreateId() //will create a new ID for the account being created 
    {
        await using (var connection = (NpgsqlConnection)await _dbConnection.CreateDBConnection())
        {
            var command = new NpgsqlCommand("SELECT * FROM students ORDER BY studentid DESC", connection);

            await using (var reader = await command.ExecuteReaderAsync())
            {
                try
                {
                    await reader.ReadAsync();
                    int id = reader.GetInt32(0);
                    int newId = id + 1;
                    return newId;
                }
                catch (Exception ex)
                {
                    return 1;
                }
            }
        }
    }
}