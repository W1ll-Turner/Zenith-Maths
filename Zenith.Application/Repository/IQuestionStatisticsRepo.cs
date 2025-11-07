using Zenith.Models.QuestionModels;


namespace Zenith.Application.Repository;

public interface IQuestionStatisticsRepo
{
    Task<bool> AddQuestioningRound(QuestionModels.AnsweredQuestionStack questions , QuestionModels.RoundInfo statistics, string studentId);
    
    Task<IEnumerable<QuestionModels>> GetAllRecentQuestionRounds();
    
    //Task<IEnumerable<WeeklySummary>> GetAllWeeklySummary();

    Task<bool> DeleteShortTermData();
    
    Task<bool> DeleteLongTermData();
}