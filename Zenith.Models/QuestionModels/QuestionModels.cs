using Zenith.Models.Account;
namespace Zenith.Models.QuestionModels;

public class QuestionModels
{
    public class QuestionStack<TAnswer> 
    {
        //This is a stack of 10 questions 
        public int Pointer = -1;
        public IQuestion<TAnswer>[] Questions = new IQuestion<TAnswer>[10]; //using the IQuestion interace so it can store all classes whhc inherited from it
        
        public void Push(IQuestion<TAnswer> question)
        {
            Pointer++;
            Questions[Pointer] = question;
        }
        public IQuestion<TAnswer> Pop()
        {
            return Questions[Pointer--];
        }
        public bool IsEmpty()
        {
            if (Pointer == -1)
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
        public string UserAnswer { get; set;  }
        public string Question { get; set; }
        public double TimeTaken { get; set; }
    }
    public class RoundInfo
    {
        public required int Difficulty { get; init; }
        public required string Topic { get; init; }
        public required string TimeCompleted { get; init; }
    }
}