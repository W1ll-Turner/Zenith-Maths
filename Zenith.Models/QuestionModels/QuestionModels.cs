using Zenith.Models.Account;

namespace Zenith.Models.QuestionModels;

public class QuestionModels
{

    class CompletedQuestionRound //This is a model to store a completed round of questions as well as all ascociated data
    {
        public string Topic {get; set;}
        public string TimeCompleted {get; set;}
        public int Score {get; set;}
        public AnsweredQuestionStack Questions { get; set; }
        
    }
    
    public class QuestionStack
    {
        //This is a stack of 10 questions 
        public int Pointer = -1;
        public IQuestion[] Questions = new IQuestion[10]; //using the IQuestion interace so it can store all classes whhc inherited from it
    
    
        public void Push(IQuestion question)
        {
            Pointer++;
            Questions[Pointer] = question;
        }

        public IQuestion Pop()
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
        public string UserAnswer { get; set; }
        public string QuestionText { get; set; }
        public double TimeTaken { get; set; }

        public AnsweredQuestion(bool correct, string correctAnswer, string userAnswer, string questionText, double timeTaken)
        {
            Correct = correct;
            CorrectAnswer = correctAnswer;
            UserAnswer = userAnswer;
            QuestionText = questionText;
            TimeTaken = timeTaken;
        }
    
    }

    public class AnsweredQuestionStack
    {
        private AnsweredQuestion[] Questions = new AnsweredQuestion[10];
        private int pointer = -1;

        public void Push(AnsweredQuestion question)
        {
            pointer++;
            Questions[pointer] = question;
        }

        public int CalculateScore()
        {
            int score = 0;
            foreach (AnsweredQuestion question in Questions)
            {
                if (question.Correct == true)
                {
                    score++;
                }
                
            }
            return score;
        }
    }

    public class RoundInfo
    {
        public required int Difficulty { get; init; }
        public required string UserId { get; init; }
        public required string Topic { get; init; }
        public required string TimeCompleted { get; init; }
    }
}

