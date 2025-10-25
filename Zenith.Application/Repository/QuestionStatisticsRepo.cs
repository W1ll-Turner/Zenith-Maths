using Npgsql;
using Zenith.Application.Database;
using Zenith.Models.QuestionModels;
using Zenith.Models.QuestionModels;
namespace Zenith.Application.Repository;

public class QuestionStatisticsRepo : IQuestionStatisticsRepo
{
    private readonly IDBConnectionFactory _dbConnection;
    
    public QuestionStatisticsRepo(IDBConnectionFactory dbConnection) //loosely coupling the databse connection system
    {
        _dbConnection = dbConnection;
    }
  
    

    public async Task<bool> AddQuestioningRound(QuestionModels.AnsweredQuestionStack questions, QuestionModels.RoundInfo Statistics) 
    {
        //this will add all the information about a round of questionsing to the required tbales 
        //needs to calcualte the score 
        
        
        //calculates the score 
        int score = questions.CalculateScore();


        await using (var connection = (NpgsqlConnection)await _dbConnection.CreateDBConnection())
        {
            var command = new NpgsqlCommand("INSERT INTO ")
            
        }
        
        return true;
    }

    public Task<IEnumerable<QuestionModels>> GetAllRecentQuestionRounds()
    {
        throw new NotImplementedException();
    }
    

    //public Task<IEnumerable<WeeklySummary>> GetAllWeeklySummary()
    //{
    //    throw new NotImplementedException();
    //}

    public Task<bool> DeleteShortTermData()
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteLongTermData()
    {
        throw new NotImplementedException();
    }
}