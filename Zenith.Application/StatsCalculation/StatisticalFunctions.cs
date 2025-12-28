namespace Zenith.Application.StatsCalculation;

public static class StatisticalFunctions
{
    //this applies sigmoiod function to the given average score 
    public static double SigmoidAverageScore(double score)
    {
        double exponent = -0.7 * (score - 6);
        double sigmoidValue = 1/(1+Math.Exp(exponent)) + 0.05732; // this is the same as 1/(1+e^-0.7(x-6))
        return sigmoidValue;
    }

    public static double SigmoidAverageTime(double averagetime)
    {
        double exponent = 2.5 * (averagetime - 3);
        double sigmoidValue = 1/(1+Math.Exp(exponent));
        
        return sigmoidValue;
    }

    public static double GeometricMean(double[] Values)
    {
        double product = 1;
        foreach (double value in Values)
        {
            product *= value;
        }
        int numValues = Values.Length;
        double root = 1.0/numValues; //forcing floating point division to take place
        double Mean = Math.Pow(product, root);
        return Mean;
    }
}