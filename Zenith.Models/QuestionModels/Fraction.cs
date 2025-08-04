namespace Zenith.Models.Account;

public class Fraction
{
    public int Numerator { get; set; }
    public int Denominator { get; set; }
    public double DecimalValue { get; set; }
    
    public string StringFormat { get; set; }

    public Fraction(int numerator, int denominator)
    {
        Numerator = numerator;
        Denominator = denominator;
        DecimalValue = Convert.ToDouble(numerator) / Convert.ToDouble(denominator);
        StringFormat = Convert.ToString(numerator) + "/" + Convert.ToString(denominator);
    }

    public static Fraction operator +(Fraction operand1, Fraction operand2)//polymorphism, overlaoding the + operator to add two fractions
    {
        int CommonDenominator = operand1.Numerator * operand1.Denominator; //finding the common denominator 
        
        //comnputing the new numerators of each fraction
        operand1.Numerator *= operand2.Denominator;
        
        operand2.Numerator *= operand1.Denominator;
        
        int Numerator = operand1.Numerator + operand2.Denominator;
        
        int HCF = HighestCommonFactor(Numerator, CommonDenominator);
        Fraction result = new Fraction(Numerator/HCF, CommonDenominator/HCF);
        return result;
    }

    private static int HighestCommonFactor(int num1, int num2)
    {
        int result = Math.Min(num1, num2);
        while (result > 0)
        {
            //minimsing result until both numbers are divisble by it 
            if (num1 % result == 0 && num2 % result == 0)
            {
                break;
                
            }
            else
            {
                result--;
            }
        }
        return result;
    }
}