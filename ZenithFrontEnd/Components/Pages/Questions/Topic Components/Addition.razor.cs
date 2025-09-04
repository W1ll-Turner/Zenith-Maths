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
    private void Start()
    {
        
        Questions = InitialiseStack(Difficulty  ); //initialising the questions // gett the actruial difficulty from the route 
        AnsweredQuestionStack = new AnsweredQuestionStack();
        Console.WriteLine(Questions.pointer);
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
       
        QuestionText = CurrentQuestion.QuestionText; //displaying the new question

        TimeToAnswer.Start(); //starting the timer 
     
        
    }
    

    public void AnswerQuestion()
    {
        
        TimeToAnswer.Stop(); //stopping the timer 
        bool AnswerCorrect = false;
        
        string answerPattern = "^[0-9](\\/[0-9])?$";
        Regex answerRG = new Regex(answerPattern); //this will make sure the answer has been given withing the correct format
        
        
        if (answerRG.IsMatch(UserAnswer)) //checking the answer is appropraite to start checking
        {
            string[] answer = UserAnswer.Split("/"); //if the answer is a fraction it will be 
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
        Console.WriteLine(Questions.pointer);
        if (Questions.isEmpty())
        {
            NavigationManager.NavigateTo("/Home");
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