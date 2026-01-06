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
    public int difficulty { get; set; }
}

public class WeeklySummary
{
    public string weekNumber { get; set; }
    public double averageTime { get; set; }
    public double averageScore { get; set; }
    public string worstTopic { get; set; }
    public string bestTopic { get; set; }
    public string completion { get; set; }
}