using Zenith.Models.QuestionModels;

namespace Zenith.Contracts.Response;

public class QuestionStatisticResponses
{
    public class MostRecentQuestionRoundResponse
    {
        public string averageTime;
        public string score;
        public string topic;
        public string difficulty;
        public IEnumerable<QuestionModels.AnsweredQuestion>  Questions;
    }
    
    
    
    
    
}