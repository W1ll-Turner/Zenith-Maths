using System.Linq.Expressions;
using Zenith.Models.Account;

namespace Zenith.Models.QuestionModels;


public class AdditionQuestion : IQuestion 
{
    public string QuestionText { get; set; }
    public Fraction Answer { get; set; } //this is the version of the answer that will be used to chekv if the user got it right
    
    public string AnswerStringFormat { get; set; } //This is the answer that will be displayed to the user and stored in the database
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

    public string GenerateEasy()
    {
        Random random = new Random();

        int operand1 = random.Next(1, 150);
        int operand2 = random.Next(1, 150);
        
        //will make a fraction with denominator 1 so that it is technically not a fraction but can still be stored under Answer
        int numerator  = operand1 + operand2;
        
        Fraction answer = new Fraction(numerator, 1);
        
        Answer = answer;
        AnswerStringFormat = Answer.StringFormat + "or" + Convert.ToString(Answer.DecimalValue);
        
        return operand2.ToString() + "+" + operand1 + " = ";
    }

    public bool CheckAnswer(Fraction userAnswer)
    {
        if (userAnswer == Answer)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

public class SubtractionQuestion : IQuestion
{
    public string QuestionText { get; set; }
    public Fraction Answer { get; set; }
    public string AnswerStringFormat { get; set; }

    public Dictionary<int, Func<string>> Generators { get; set; }


    public SubtractionQuestion(int difficulty)
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
        if (Generators.TryGetValue(difficulty, out Func<string> generator))
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
        //initialisng the random class
        Random random = new Random();
        
        //generating the fractions using random generation
        Fraction operand1 = new Fraction(random.Next(1,20) , random.Next(1,12));
        Fraction operand2 = new Fraction(random.Next(1,20),  random.Next(1,12));
        
        //computing the answer and putting it inot string form 
        Answer = operand1 - operand2;
        AnswerStringFormat = Answer.StringFormat + "or" + Convert.ToString(Answer.DecimalValue);
        
         //returning the answer as either a decimal or fraction 
         int DecimalOrFraction = random.Next(0,1);
         if (DecimalOrFraction == 0)
         {
             return operand1.StringFormat + "-" + operand2.StringFormat;
         }
         else
         {
             return Convert.ToString(operand1.DecimalValue) + " - " + Convert.ToString(operand2.DecimalValue);
         }
    }

    public string GenerateMedium()
    {
        //initialisng the random 
        Random random = new Random();
        int[] acceptableDenominators = new int[] { 2, 3, 4, 5, 8, 10}; //the list of accepatbale denominators for the fractions being used 
        
        //randomly generating the fracions for the questions 
        Fraction operand1 = new Fraction(random.Next(1,12), acceptableDenominators[random.Next(0, acceptableDenominators.Length - 1)]);
        Fraction operand2 = new Fraction(random.Next(1,12),  acceptableDenominators[random.Next(0, acceptableDenominators.Length - 1)]);
        
        //computing the answer the to the question and putting it into the string format
        Answer = operand1 - operand2;
        AnswerStringFormat = Answer.StringFormat + "or" + Convert.ToString(Answer.DecimalValue);
        
        //returning the question string as a decimal or fraction whcih is decided randomly
        int DecimalOrFraction = random.Next(0,1);
        if (DecimalOrFraction == 0)
        {
            return operand1.StringFormat + "-" + operand2.StringFormat;
        }
        else
        {
            return Convert.ToString(operand1.DecimalValue) + " + " + Convert.ToString(operand2.DecimalValue);
        }
    }

    public string GenerateEasy()
    {
        //generatting the numbers for the question
        Random random = new Random();
        int operand1 = random.Next(1, 150);
        int operand2 = random.Next(1, 150);
        
        //computing the answwer
        int numerator = operand1 - operand2;
        
        //storing the answer and putting it into a string format
        Fraction answer = new Fraction(numerator, 1);
        Answer = answer;
        AnswerStringFormat = Answer.StringFormat + "or" + Convert.ToString(Answer.DecimalValue);

        //getting and returning the text for the question
        string questionText = operand1.ToString() + "-" + operand2.ToString();
        return questionText;
    }

    public bool CheckAnswer(Fraction userAnswer)
    {
        if (userAnswer == Answer)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}



public class MultiplicationQuestion : IQuestion
{
    public string QuestionText { get; set; }
    public Fraction Answer { get; set; }
    public string AnswerStringFormat { get; set; }
    public Dictionary<int, Func<string>> Generators { get; set; }
    public void Generate(int difficulty)
    {
        throw new NotImplementedException();
    }

    public string GenerateHard()
    {
        throw new NotImplementedException();
    }

    public string GenerateMedium()
    {
        throw new NotImplementedException();
    }

    public string GenerateEasy()
    {
        throw new NotImplementedException();
    }

    public bool CheckAnswer(Fraction answer)
    {
        throw new NotImplementedException();
    }
}



public class DivisionQuestion : IQuestion
{
    public string QuestionText { get; set; }
    public Fraction Answer { get; set; }
    public string AnswerStringFormat { get; set; }
    public Dictionary<int, Func<string>> Generators { get; set; }

    public void Generate(int difficulty)
    {
        throw new NotImplementedException();
    }

    public string GenerateHard()
    {
        throw new NotImplementedException();
    }

    public string GenerateMedium()
    {
        throw new NotImplementedException();
    }

    public string GenerateEasy()
    {
        throw new NotImplementedException();
    }

    public bool CheckAnswer(Fraction answer)
    {
        throw new NotImplementedException();
    }
}

