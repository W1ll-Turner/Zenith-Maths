namespace Zenith.Models.QuestionModels;
public class Fraction
{
    private int Numerator { get; set; }
    private int Denominator { get; set; }
    public double DecimalValue { get; set; }
    public string StringFormat { get; set; }

    //constructor to take the initial values of the fraction and then put it into a string form and decimal form
    public Fraction(int numerator, int denominator)
    {
        Numerator = numerator;
        Denominator = denominator;
        DecimalValue = Convert.ToDouble(numerator) / Convert.ToDouble(denominator);
        StringFormat = Convert.ToString(numerator) + "/" + Convert.ToString(denominator);
    }
    
    //using polymorphism and overlaoding the == operator to compare two fractions 
    public static bool operator ==(Fraction a, Fraction b)
    {
        if (a.DecimalValue == b.DecimalValue || (a.Numerator == b.Numerator && a.Denominator == b.Denominator))
        {
            return true;
        }
        return false;
    }
    
    //overlaoding the != to compare two fractions
    public static bool operator !=(Fraction a, Fraction b)
    {
        return !(a == b);
    }
    
    //polymorphism, overlaoding the + operator to add two fractions
    public static Fraction operator +(Fraction operand1, Fraction operand2)
    {
        int commonDenominator = operand1.Denominator * operand2.Denominator; //finding the common denominator 
        
        //comnputing the new numerators of each fraction with the common denomitator
        operand1.Numerator *= operand2.Denominator;
        
        operand2.Numerator *= operand1.Denominator;
        
        //Numerator of the answeer
        int numerator = operand1.Numerator + operand2.Denominator;
        
        //Finding the Highest common factor
        int hcf = HighestCommonFactor(numerator, commonDenominator);
        
        //generating the new fraction in its simplest form 
        Fraction result = new Fraction(numerator/hcf, commonDenominator/hcf);
        return result;
    }
    
    //using polymorphism on the - operator to subtract the fractions from eachother
    public static Fraction operator -(Fraction operand1, Fraction operand2) 
    {
        int commonDenominator = operand1.Denominator * operand2.Denominator; //getting the common denomintor
        
        //computing the new numerators for each of the frcations 
        operand1.Numerator *= operand2.Denominator;
        operand2.Numerator *= operand1.Denominator;
        
        //getting the numerator of the asnwer 
        int numerator = operand1.Numerator - operand2.Denominator;
        //getting highest comomoon factor of what will be the new fraction
        int hcf = HighestCommonFactor(numerator, commonDenominator);
        
        //initialisng the answer as a frcation 
        Fraction result = new Fraction(numerator/hcf, commonDenominator/hcf);
        return result;
        
    }
    
    //using polymorphism to overload the * operator to allow fractions to be multiplied together 
    public static Fraction operator *(Fraction operand1, Fraction operand2) 
    {
        //computing the numerator and denominator 
        int numerator = operand1.Numerator * operand2.Numerator;
        int denominator = operand1.Denominator * operand2.Denominator;
        
        //getting the highest common factor of the new fraction so it can be simplified
        int hcf = HighestCommonFactor(numerator, denominator);
        
        Fraction result = new Fraction(numerator/hcf, denominator/hcf);
        return result;
    }
    
    //using polymorphims to overload the / operator allow two fractions to be divided
    public static Fraction operator /(Fraction operand1, Fraction operand2)
    {
        //computing the numerattor and denominator of each fraction by using keep flip change then multiplying 
        int numerator = operand1.Numerator * operand2.Denominator;
        int deominator =operand1.Denominator * operand2.Numerator;
        
        //finding hcf of the new fraction
        int hcf = HighestCommonFactor(numerator, deominator);
        
        //initialising the fraction and returning it 
        Fraction result = new Fraction(numerator/hcf, deominator/hcf);
        return result;
    }
    
    //this method is used to find the highest common factor of two numbers
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
            result--;
        }
        return result;
    }
}