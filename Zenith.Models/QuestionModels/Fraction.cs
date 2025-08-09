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

    public static bool operator ==(Fraction a, Fraction b) //This is used to chekc if two Fracxtion objects are equal to eachother 
    {
        
        if (a.DecimalValue == b.DecimalValue || (a.Numerator == b.Numerator && a.Denominator == b.Denominator))
        {
            return true;
        }
        else
        {
            return false;
        }
        

        
    }

    public static bool operator !=(Fraction a, Fraction b)
    {
        return !(a == b);
    }


    public static Fraction operator +(Fraction operand1, Fraction operand2)//polymorphism, overlaoding the + operator to add two fractions
    {
        int CommonDenominator = operand1.Numerator * operand1.Denominator; //finding the common denominator 
        
        //comnputing the new numerators of each fraction with the common denomitator
        operand1.Numerator *= operand2.Denominator;
        
        operand2.Numerator *= operand1.Denominator;
        
        //Numerator of the answeer
        int Numerator = operand1.Numerator + operand2.Denominator;
        
        //Finding the Highest common factor
        int HCF = HighestCommonFactor(Numerator, CommonDenominator);
        
        //generating the new fraction in its simplest form 
        Fraction result = new Fraction(Numerator/HCF, CommonDenominator/HCF);
        return result;
    }

    private static int HighestCommonFactor(int num1, int num2)
    {
        int result = Math.Min(num1, num2);
        while (result > 0)
        {
            //minimsing result until both numbers are divisble by it, the first time this works will be the HCF
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