using Zenith.Models.Account;
using Zenith.Models.QuestionModels;


namespace Zenith.Application.Repository;

public interface IQuestionStatisticsRepo
{
    Task<bool> AddQuestioningRound(IEnumerable<QuestionModels.AnsweredQuestion> questions , QuestionModels.RoundInfo statistics, string studentId);
    
    Task<IEnumerable<QuestionModels>> GetAllRecentQuestionRounds(string studentId);
    
    Task<IEnumerable<WeeklySummary>> GetAllLongTermStats(string studentId);
    
    Task<CompletedRoundOfQuestioning> GetMostRecentQuestionRound(string studentId);

    Task<bool> DeleteShortTermData();
    
    Task<bool> DeleteLongTermData();

    Task<bool> AddLongTermData(string ID);
}