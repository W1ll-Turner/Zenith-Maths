using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Zenith.Models.Account;

namespace ZenithFrontEnd.Components.Pages.Questions.Topic_Components;

public partial class Addition : ComponentBase
{
    public int Difficulty { get; set; }
    public string QuestionText { get; set; } = "";
    public string UserAnswer { get; set; } = "";
    
    
    public bool Error = false;
    protected override async Task OnInitializedAsync()
    {
        Error = false;
        QuestionStack questionStack = InitialiseStack(2  ); //initialising the questions // gett the actruial difficulty from the local storgae
        AnsweredQuestionStack answeredQuestions = new AnsweredQuestionStack();
        QuestionSequence(questionStack);


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


    public void QuestionSequence(QuestionStack questionStack)
    {
        question
    }
    

    public void AnswerQuestion(AdditionQuestion CurrentQuestion)
    {
        string answerPattern = "^[0-9](\\/[0-9])?$";
        Regex answerRG = new Regex(answerPattern); //this will make sure the answer has been given withing the correct format
        if (answerRG.IsMatch(QuestionText))
        {
            string[] answer = UserAnswer.Split("/");
            if (answer.Length == 1)
            {
                Fraction input = new Fraction(int.Parse(answer[0]), 1);
                CurrentQuestion.CheckAnswer(input);
            }else if (answer.Length == 2)
            {
                Fraction input = new Fraction(int.Parse(answer[0]), int.Parse(answer[1]));
                CurrentQuestion.CheckAnswer(input);
            }
            else
            {
                //wrong inpout type 
                Console.WriteLine("wrong input type");
            }
            
        }
        {
            Error = true;//wring input 
        }
        


    }
    
    
    
    
}