using Zenith.Models.Account;
using Zenith.Models.QuestionModels;

namespace Zenith.Contracts.Request.Account;

public class QuestioningRequests
{
    public class QuestioningRoundCompletionRequest
    {
        public required int Difficulty { get; init; }
        public required string UserId { get; init; }
        public required string Topic { get; init; }
        public required string TimeCompleted { get; set; }
        public required AnsweredQuestionStack QuestionStack { get; init; }
    }
    
    
    
}