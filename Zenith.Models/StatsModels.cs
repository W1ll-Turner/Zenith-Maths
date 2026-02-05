namespace Zenith.Models.Account;
public class shorttermsstatsinfo
{
    public double averagetime { get; set; }
    public int score { get; set; }
    public int difficulty { get; set; }
        
}

public class TopicAverages
{
    public double averageTime { get; set; }
    public double averageScore { get; set; }
    public int numberOfRounds { get; set; }
    public double difficulty { get; set; }
}

public class WeeklySummary
{
    public string weekNumber { get; set; }
    public double averageTime { get; set; }
    public double averageScore { get; set; }
    public string worstTopic { get; set; }
    public string bestTopic { get; set; }
    public string difficulty { get; set; }
    public double completion { get; set; }
}

public class CompletedRoundOfQuestioning
{
    public double averageTime { get; set; }
    public int score { get; set; }
    public string topic { get; set; }
    public int difficulty { get; set; }
    
    public IEnumerable<QuestionModels.QuestionModels.AnsweredQuestion>  answeredQuestions { get; set; }
}