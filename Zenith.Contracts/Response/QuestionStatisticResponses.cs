using Zenith.Models.QuestionModels;

namespace Zenith.Contracts.Response;

public class QuestionStatisticResponses
{
    public class MostRecentQuestionRoundResponse
    {
        public string averageTime {get; set;}
        public string score {get; set;}
        public string topic {get; set;}
        public string difficulty {get; set;}
        public IEnumerable<QuestionModels.AnsweredQuestion>  Questions {get; set;}
    }
}