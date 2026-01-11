using Zenith.Models.QuestionModels;

namespace Zenith.Contracts.Response;

public class QuestionStatisticResponses
{
    public class MostRecentQuestionRoundResponse
    {

        public string averageTime;
        public string score;
        public List<QuestionModels.AnsweredQuestion>  Questions;
        //public string 
    }
    
    
    
    
    
}