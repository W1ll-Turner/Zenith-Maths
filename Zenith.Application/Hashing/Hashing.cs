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
        /*

         function ShortTermHashKey (studentID)
               query = "SELECT * FROM ShortTermStatsBridge WHERE studentID = studentID"
               columns = runQuery(query)
               entries = columns.length + 1
               Hash = studentID + “#” + ToString(entries)
               return Hash
           endfunction

         */
        
        await using (var connection = (NpgsqlConnection)await _dbConnection.CreateDBConnection())
        {
            var command = new NpgsqlCommand("SELECT COUNT(*) FROM ShortTermStatBridge WHERE StudentId = @StudentId", connection);
            command.Parameters.AddWithValue("StudentId", StudentId);
            
            
        }
    }

    public Task<string> GenerateRoundID(string StudentID)
    {
        
        
        
    }

    public async Task<string> GenerateLongTermStatsID(string StudentId)
    {
        Calendar Calendar = CultureInfo.InvariantCulture.Calendar;
        DateTime date = DateTime.Today;
        int year = date.Year;
        int WeekNum = Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, date.DayOfWeek);

        string Hash = StudentId + "#"  + WeekNum + "#" + year;
        return Hash;

    }
}