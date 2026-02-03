using System.Globalization;
using Npgsql;
using Zenith.Application.Database;

namespace Zenith.Application.Hashing;

//inheriting from the interface so that it can be properly implemented
public class Hashing : IHashing
{
    private readonly IDBConnectionFactory _dbConnection;
    
    public Hashing(IDBConnectionFactory dbConnection) //loosely coupling the databse connection system
    {
        _dbConnection = dbConnection;
    }
    
    //this methdod generates the hash for the shorttermid table 
    public async Task<string> GenerateShortTermStatsId(string studentId)
    {
        int entries;
        //creating database connection, automatically closes when statement ends
        await using (var connection = (NpgsqlConnection)await _dbConnection.CreateDBConnection())
        {
            //Query to get the number of entries already linked to that student ID in the table
            var command = new NpgsqlCommand("SELECT COUNT(*) FROM shorttermstatsbridge WHERE Studentid = @StudentId", connection);
            command.Parameters.AddWithValue("@StudentId", studentId); //adding queriy paramters

            //extracting the value the query and incrementing 
            await using (var reader = await command.ExecuteReaderAsync())  
            {
                await reader.ReadAsync();
                entries = reader.GetInt32(0) + 1; //adding 1 so it can be used for the hash
            }
        }
        //making the hash by putting the student id and number of entries together 
        string hash = studentId + "#" + entries.ToString();   
        return hash;
    }
    
    //this will generate the hash for the long terms stats tabele
    public async Task<string> GenerateLongTermStatsId(string studentId) 
    {
        //getting today's date 
        Calendar calendar = CultureInfo.InvariantCulture.Calendar; 
        DateTime date = DateTime.Today; 
        int year = date.Year;  
        
        //getting the week number 
        int weekNum = calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, date.DayOfWeek); 

        //Putting the hash together
        string hash = studentId + "#"  + weekNum + "#" + year; 
        return hash;
    }
}