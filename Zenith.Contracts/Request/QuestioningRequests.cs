using Zenith.Models.Account;
using Zenith.Models.QuestionModels;
using Zenith.Models;
namespace Zenith.Contracts.Request.Account;

public class QuestioningRequests
{
    public class CompletedQuestionRoundRequest
    {
        public required int Difficulty { get; init; }
        public required string UserId { get; init; }
        public required string Topic { get; init; }
        public required string TimeCompleted { get; init; }
        public required QuestionModels.AnsweredQuestionStack QuestionStack { get; init; }
    }
    
    
    
}