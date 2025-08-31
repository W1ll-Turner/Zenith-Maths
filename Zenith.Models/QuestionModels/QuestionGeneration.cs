namespace Zenith.Models.Account;

public class QuestionStack
{
    //This is a stack of 10 questions 
    private int pointer = -1;
    public IQuestion[] Questions = new IQuestion[10]; //using the IQuestion interace so it can store all classes whhc inherited from it
    
    
    public void Push(IQuestion question)
    {
        pointer++;
        Questions[pointer] = question;
    }

    public IQuestion Pop()
    {
        return Questions[pointer--];
    }

    public bool isEmpty()
    {
        if (pointer == -1)
        {
            return true;
        }
        return false;
    }

}

//all question types no matter the topic will end up as this object to be then sent to the API
public class AnsweredQuestion
{
    public bool Correct { get; set; }
    public string CorrectAnswer { get; set; }
    public string UserAnswer { get; set; }
    public string QuestionText { get; set; }
    public double TimeTaken { get; set; }
}

public class AnsweredQuestionStack
{
    private AnsweredQuestion[] Questions = new AnsweredQuestion[10];
    
}


public class AdditionQuestion : IQuestion
{
    public string QuestionText { get; set; }
    public Fraction Answer { get; set; }
    public Dictionary<int, Func<string>> _generators { get; set; }



    public AdditionQuestion(int difficulty)
    {
        _generators = new Dictionary<int, Func<string>>
        {
            { 1, GenerateEasy },
            { 2, GenerateMedium },
            { 3, GenerateHard }
        };
        Generate(difficulty);
    }

    public void Generate(int difficulty)
    {
        if (_generators.TryGetValue(difficulty,
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
    

   

    
    
    
