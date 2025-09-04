using Zenith.Models.Account;

namespace Zenith.Contracts.Request.Account;

public class QuestioningRequests
{
    public class QuestioningRoundCompletionRequest
    {
        public required int difficulty { get; init; }
        public required string UserID { get; init; }
        public required string Topic { get; init; }
        public required string TimeCompleted { get; set; }
        public required AnsweredQuestionStack QuestionStack { get; init; }
    }
    
    
    
}