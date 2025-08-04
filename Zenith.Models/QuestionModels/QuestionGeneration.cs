namespace Zenith.Models.Account;

public class Addition : IQuestion
{
    public string QuestionText { get; set; }
    public Fraction Answer { get; set; }
    public string Difficulty { get; set; }
    public Dictionary<string, Func<string>> _generators { get; set; }



    public Addition(string difficulty)
    {
        Difficulty = difficulty.ToString();
        QuestionText = difficulty;

        _generators = new Dictionary<string, Func<string>>
        {
            { "easy", GenerateEasy },
            { "medium", GenerateMedium },
            { "hard", GenerateHard }
        };
    }

    public void Generate()
    {
        if (_generators.TryGetValue(Difficulty,
                out Func<string> generator)) //if the key is within the dictionary it will call the coressponding function as the method generator()
        {
            QuestionText = generator();
        }
        else
        {
            throw new KeyNotFoundException();
        }
    }

    public string GenerateHard()
    {
        
        Random random = new Random();
        
        Fraction operand1 = new Fraction(random.Next(1,20) , random.Next(1,12));
        Fraction operand2 = new Fraction(random.Next(1,20),  random.Next(1,12));
        //computing the answer
        Answer = operand1 + operand2;
        
        int DecimalOrFraction = random.Next(0,1);
        //this will return a fraction or decimal question at random each time, 0 being a fraction and 1 a decimal
        if (DecimalOrFraction == 0)
        {
            return operand1.StringFormat + " + " + operand2.StringFormat;
        }
        else
        {
            return Convert.ToString(operand1.DecimalValue) + " + " + Convert.ToString(operand2.DecimalValue) + " ="; 
        }
    }

    public string GenerateMedium()
    {
        int[] acceptableDenominators = new int[] { 2, 3, 4, 5, 8, 10}; //This is a list of acceptable denominators for the fractions to be generated, This means the resulting fraction or decimal will not be too harsh
        Random random = new Random();
        
        Fraction operand1 = new Fraction(random.Next(1,12) , acceptableDenominators[random.Next(0, acceptableDenominators.Length - 1)]);
        Fraction operand2 = new Fraction(random.Next(1,12),  acceptableDenominators[random.Next(0, acceptableDenominators.Length - 1)]);
        //computing the answer
        Answer = operand1 + operand2;
        
        int DecimalOrFraction = random.Next(0,1);
        //this will return a fraction or decimal question at random each time, 0 being a fraction and 1 a decimal
        if (DecimalOrFraction == 0)
        {
            return operand1.StringFormat + " + " + operand2.StringFormat;
        }
        else
        {
            return Convert.ToString(operand1.DecimalValue) + " + " + Convert.ToString(operand2.DecimalValue) + " ="; 
        }
    }

    public string GenerateEasy()
    {
        Random random = new Random();

        int operand1 = random.Next(0, 150);
        int operand2 = random.Next(0, 150);
        
        //will make a fraction with denominator 1 so that it is technically not a fraction but can still be stored under Answer
        Answer.Numerator = operand1 + operand2;
        Answer.Denominator = 1;
        return operand2.ToString() + operand1 + " = ";
    }
    
}
    

   

    
    
    
