using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Zenith.Models.Account;

namespace ZenithFrontEnd.Components.Pages.Questions.Topic_Components;

public partial class Addition : ComponentBase
{
    public int Difficulty { get; set; }
    private string QuestionText { get; set; } = "";
    private string UserAnswer { get; set; } = "";
    private Stopwatch TimeToAnswer { get; set; } = new Stopwatch();
    
    private QuestionStack Questions { get; set; } = new QuestionStack();
    private AdditionQuestion CurrentQuestion { get; set; }
    private AnsweredQuestionStack AnsweredQuestionStack { get; set; } 
    protected override async Task OnInitializedAsync()
    {
        
        Questions = InitialiseStack(Difficulty  ); //initialising the questions // gett the actruial difficulty from the route 
        AnsweredQuestionStack = new AnsweredQuestionStack();
        QuestionSequence(); //prepare the question for the user to answer


    }

    private QuestionStack InitialiseStack(int difficulty) //used to generate a stack of questions 
    {
        int testdifficulty = 1;
        
        QuestionStack questions = new QuestionStack();
        for (int i = 0; i < 10; i++)
        {
            AdditionQuestion question = new AdditionQuestion(testdifficulty);
            Console.WriteLine("pushing item to the stack");
            questions.Push(question);

        }
        
        return questions;
    }


    private void QuestionSequence()
    {
        CurrentQuestion = (AdditionQuestion)Questions.Pop(); //getting the next question off the stack \
        QuestionText = CurrentQuestion.QuestionText;
        TimeToAnswer.Start();
        
    }
    

    public void AnswerQuestion()
    {
        TimeToAnswer.Stop();
        bool AnswerCorrect = false;
        
        string answerPattern = "^[0-9](\\/[0-9])?$";
        Regex answerRG = new Regex(answerPattern); //this will make sure the answer has been given withing the correct format
        if (answerRG.IsMatch(UserAnswer))
        {
            string[] answer = UserAnswer.Split("/");
            if (answer.Length == 1)
            {
                Fraction input = new Fraction(int.Parse(answer[0]), 1);
                AnswerCorrect = CurrentQuestion.CheckAnswer(input);
            }else if (answer.Length == 2)
            {
                Fraction input = new Fraction(int.Parse(answer[0]), int.Parse(answer[1]));
                AnswerCorrect = CurrentQuestion.CheckAnswer(input);
            }
        }
        
        
        AnsweredQuestion answeredQuestion = new AnsweredQuestion(AnswerCorrect , CurrentQuestion.AnswerStringFormat , UserAnswer , CurrentQuestion.QuestionText ,TimeToAnswer.ElapsedMilliseconds);
        AnsweredQuestionStack.Push(answeredQuestion);

        if (Questions.isEmpty())
        {
            SendResultsToAPI(); //round of questioning has finshed time, send results to API then move user onto the summary screen 

        }
        else
        {
            QuestionSequence();
        }
        
        


    }

    private void  SendResultsToAPI()
    {
        
    }
    
    
}