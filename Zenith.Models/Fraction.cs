namespace Zenith.Models.Account;

public class Fraction
{
    public int Numerator { get; set; }
    public int Denominator { get; set; }
    public double DecimalValue { get; set; }

    public Fraction(int numerator, int denominator)
    {
        Numerator = numerator;
        Denominator = denominator;
        DecimalValue = Convert.ToDouble(numerator) / Convert.ToDouble(denominator);;
    }
}