using Zenith.Models.Account;
using Zenith.Models.QuestionModels;
using Zenith.Models;
namespace Zenith.Contracts.Request.Account;

public class QuestioningRequests
{
    public class CompletedQuestionRoundRequest
    {
        public int Difficulty { get; set; }
        public string UserId { get; set; }
        public string Topic { get; set; }
        public string TimeCompleted { get; set; }
        public List<QuestionModels.AnsweredQuestion> questions { get; set; }
        
    }

    public class GetMostRecentQuestionRoundRequest
    {
        public string UserId { get; init; }
        
    }
    
    
    
}