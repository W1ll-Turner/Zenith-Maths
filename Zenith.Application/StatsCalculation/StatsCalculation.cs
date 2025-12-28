namespace Zenith.Application.StatsCalculation;

public class StatsCalculation : IStatsCalculation
{
    public async Task<double> CalculateTopicCompletion(int  difficulty, int RoundsCompleted, double AverageTime, double AverageScore)
    {
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

   

    
}