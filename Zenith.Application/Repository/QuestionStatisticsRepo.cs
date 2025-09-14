using Zenith.Application.Database;
using Zenith.Models.QuestionModels;

namespace Zenith.Application.Repository;

public class QuestionStatisticsRepo : IQuestionStatisticsRepo
{
    private readonly IDBConnectionFactory _dbConnection;
    
    public QuestionStatisticsRepo(IDBConnectionFactory dbConnection) //loosely coupling the databse connection system
    {
        _dbConnection = dbConnection;
    }


    public Task<bool> AddQuestioningRound(QuestionModels models)
    {
        //this needs to work out the average time
        throw new NotImplementedException();
    }

    public Task<IEnumerable<QuestionModels>> GetAllRecentQuestionRounds()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<WeeklySummary>> GetAllWeeklySummary()
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteShortTermData()
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteLongTermData()
    {
        throw new NotImplementedException();
    }
}