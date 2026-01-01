using Zenith.Models.Account;

namespace Zenith.Application.StatsCalculation;

public class StatsCalculation : IStatsCalculation
{
    public async Task<TopicAverages> CalculateTopicAverages(List<shorttermsstatsinfo> info)
    {
        //getting the difficulty 
        int difficulty = info[0].difficulty;
        
        //calucltuin the averges using the arithmetic mean 
        double totalTime = 0;
        int Totalscore = 0;
        int numberOfRounds = 0;
        foreach (var item in info)
        {
            totalTime += item.averagetime;
            Totalscore += item.score;
            numberOfRounds++;
        }
        
        //initialising the averges class and adding the value to it
        TopicAverages Averages = new TopicAverages()
        {
            averageTime = totalTime/numberOfRounds,
            averageScore = Totalscore/numberOfRounds,
            numberOfRounds = numberOfRounds,
            difficulty = difficulty
        };
        return Averages;
    }

    public async Task<double> CalculateTopicCompletion(TopicAverages averages)
    {
        int difficulty = averages.difficulty;
        int RoundsCompleted = averages.numberOfRounds;
        double AverageScore = averages.averageTime;
        double AverageTime = averages.averageScore;
        
        //using the mathjemtical functions to code the summary statistics to a value between 0 and 1 
        double AverageScoreSigmoidValue = StatisticalFunctions.SigmoidAverageScore(AverageScore);
        double AverageTimeSigmoidValue = StatisticalFunctions.SigmoidAverageTime(AverageTime);
        double CodedDifficulty = difficulty / 3.0;
        double CodedRoundsCompleted = RoundsCompleted / 10;
        
        //finsding the geometric mean of those values
        double[] Values = new double[]{AverageScoreSigmoidValue, AverageTimeSigmoidValue, CodedDifficulty, CodedRoundsCompleted};
        double CompletionScore = StatisticalFunctions.GeometricMean(Values) * 100;

        //returng the vcompletions 
        return CompletionScore;
    }
    
    
    public async Task<double> CalclulateOverallCompletion(double[] values)
    {
        double CompletionScore = StatisticalFunctions.GeometricMean(values);
        return CompletionScore;
    }

    public async Task<int> GetBestTopicID(double[] values)
    {
        double highest = Math.Max(values[0], values[1]);
        bool found = false;
        int i = 0;
        //linear search to find where the best topic is, and therfore the topic its, must be linear as the index number of each item relates to the topic in which it changed
        while (true)
        {
            if (values[i] == highest)
            {
                break;
            }
            
            i++;
            
        }

        return i + 1;
    }
    public async Task<int> GetWorstTopicID(double[] values)
    {
        double lowest = Math.Min(values[0], values[1]);
        bool found = false;
        int i = 0;
        //linear search to find where the best topic is, and therfore the topic its, must be linear as the index number of each item relates to the topic in which it changed
        while (true)
        {
            if (values[i] == lowest)
            {
                break;
            }
            
            i++;
            
        }

        return i + 1;
    }

    public async Task<double[]> CalculateAverageTimeAndScore(TopicAverages[] averages)
    {
        double TotalScore = 0;
        double TotalTime = 0;
        
        foreach (TopicAverages average in averages)
        {
            TotalScore += average.averageScore;
            TotalTime += average.averageTime;
        }

        double[] values = [TotalScore/9 , TotalTime/9];
        return values;
    }
}