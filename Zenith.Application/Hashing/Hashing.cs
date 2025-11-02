using System.Globalization;
using Npgsql;
using Zenith.Application.Database;

namespace Zenith.Application.Hashing;

public class Hashing : IHashing
{
    private readonly IDBConnectionFactory _dbConnection;
    
    public Hashing(IDBConnectionFactory dbConnection) //loosely coupling the databse connection system
    {
        _dbConnection = dbConnection;
    }
    
    public async Task<string> GenerateShortTermStatsID(string StudentId)
    {
        int Entries = 1;
        await using (var connection = (NpgsqlConnection)await _dbConnection.CreateDBConnection())
        {
            var command = new NpgsqlCommand("SELECT COUNT(*) FROM shorttermstatbridge WHERE StudentId = @StudentId", connection); //this will get the number of entries in the shorttermstst bridge table which the current student has
            command.Parameters.AddWithValue("StudentId", StudentId);

            await using (var reader = await command.ExecuteReaderAsync()) //extracting the value the query and incrementing 
            {
                reader.ReadAsync();
                Entries = reader.GetInt32(0) + 1; 
                
            }
        }
        
        string Hash = StudentId + "#" + Entries.ToString();  //making the hash by putting the student id and number of entries together  
        return Hash;
    }
    

    public async Task<string> GenerateLongTermStatsID(string StudentId) //will generate the primary key for the LongTermStatistics table 
    {
        Calendar Calendar = CultureInfo.InvariantCulture.Calendar; //instantiating the calendar class from the inbuilt collections
        DateTime date = DateTime.Today; //getting today's date
        int year = date.Year;  
        int WeekNum = Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, date.DayOfWeek); //getting the number of which the week in the week in year resides: if it is 1st of january it is week 1

        string Hash = StudentId + "#"  + WeekNum + "#" + year; //Putting the hash together
        return Hash;

    }
}