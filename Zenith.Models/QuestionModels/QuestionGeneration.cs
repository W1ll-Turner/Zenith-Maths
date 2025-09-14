using Zenith.Models.Account;

namespace Zenith.Models.QuestionModels;

public class AdditionQuestion : IQuestion
{
    public string QuestionText { get; set; }
    public Fraction Answer { get; set; }
    
    public string AnswerStringFormat { get; set; }
    public Dictionary<int, Func<string>> Generators { get; set; }



    public AdditionQuestion(int difficulty)
    {
        Generators = new Dictionary<int, Func<string>>
        {
            { 1, GenerateEasy },
            { 2, GenerateMedium },
            { 3, GenerateHard }
        };
        Generate(difficulty);
    }

    public void Generate(int difficulty)
    {
        if (Generators.TryGetValue(difficulty,
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
        
        //initialising fraction objects
        Fraction operand1 = new Fraction(random.Next(1,20) , random.Next(1,12));
        Fraction operand2 = new Fraction(random.Next(1,20),  random.Next(1,12));
        //computing the answer
        Answer = operand1 + operand2;
        AnswerStringFormat = Answer.StringFormat + "or" + Convert.ToString(Answer.DecimalValue);
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

        int operand1 = random.Next(1, 150);
        int operand2 = random.Next(1, 150);
        
        //will make a fraction with denominator 1 so that it is technically not a fraction but can still be stored under Answer
        int numerator  = operand1 + operand2;
        
        Fraction answer = new Fraction(numerator, 1);
        
        Answer = answer;
        Console.WriteLine("answre to Q " + answer.Numerator);
        return operand2.ToString() + "+" + operand1 + " = ";
    }

    public bool CheckAnswer(Fraction userAnswer)
    {
        if (userAnswer.DecimalValue == Answer.DecimalValue)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
}
    
    