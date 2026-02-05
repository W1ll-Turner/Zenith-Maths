using Npgsql;
using Zenith.Application.Database;
using Zenith.Models.Account;
namespace Zenith.Application.Repository;

//inheritance, this is the implmentation of the IAccountRepo to implement the data collection methods,
public class AccountRepo : IAccountRepo 
{
    private readonly IDBConnectionFactory _dbConnection;
    //loosely coupling the dbconnection factory 
    public AccountRepo(IDBConnectionFactory dbConnection)
    {
        _dbConnection = dbConnection;
    }
    
    public async Task<bool> CreateAccount(SignUp account)
    {
        //connecting to the database
        await using (var connection = (NpgsqlConnection)await _dbConnection.CreateDBConnection())
        {
            //this will get the number of user's in the databse who already have the username that was provided. So the repo knows whetehr to reject the request due to a duplicate account name 
            var command = new NpgsqlCommand("SELECT COUNT(*) FROM students WHERE username = @username", connection);
            //adding the parameters
            command.Parameters.AddWithValue("@username", account.Username);
            
            //getting the count and returning the function if it is greater than 0
            await using (var reader = await command.ExecuteReaderAsync())
            {
                await reader.ReadAsync();
                int id = reader.GetInt32(0);
                if (id > 0)
                {
                    return false;
                }
            }
        }
        
        //connecting to the database 
        await using (var connection = (NpgsqlConnection)await _dbConnection.CreateDBConnection())
        {
            try
            {   
                //query to insert the account info into the database
                var command = new NpgsqlCommand("INSERT INTO students (studentid, email, username, fullname, password, classcode) VALUES ( @student, @email,  @username,  @fullname, @password, @classcode)", connection);
                int id = await CreateId();
                
                //adding the paramerters to the query
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
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
    }

    //this will be used to check if the account exists and if so it returns the id
    public async Task<int> LogIn(LogIn account) 
    {
        //conecting to the database
        await using (var connection = (NpgsqlConnection)await _dbConnection.CreateDBConnection())
        {
            //query to get the id form the databse corresponding to the user's crdentials 
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
    
    private async Task<int> CreateId() //will create a new ID for the account being created 
    {
        await using (var connection = (NpgsqlConnection)await _dbConnection.CreateDBConnection())
        {
            //query to get the largets id form the table
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