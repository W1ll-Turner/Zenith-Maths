using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Zenith.Models.Account;
using Zenith.Models.QuestionModels;

namespace ZenithFrontEnd.Components.Pages.Questions;


public partial class QuestionAnsweringPage:ComponentBase
 
{
    [Parameter]
    public int Difficulty { get; set; }
    
    [Parameter]
    public string Topic { get; set; } = null!;
    private string QuestionText { get; set; } = "";
    private string UserAnswer { get; set; } = "";
    private Stopwatch TimeToAnswer { get; set; } = new Stopwatch();
    
    private QuestionModels.QuestionStack Questions { get; set; } = new QuestionModels.QuestionStack();
    private IQuestion CurrentQuestion { get; set; }
    private QuestionModels.AnsweredQuestionStack AnsweredQuestionStack { get; set; } 
    public Dictionary<string, Func<bool>> TopicsMapper { get; set; }
    
    public bool StopQuestioning { get; set; } = false;

    public int[] CorrectAnswers { get; set; } = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
    private int questionNum = 0;

    private void Start()
    {
        StopQuestioning = true;
        Console.WriteLine("started");
        //this dictionary maps the topic to the genric methods that will generate the questions, This is dependant on a topic
        TopicsMapper = new Dictionary<string, Func<bool>>()
        {
            //the key is the topic and the vlue is a function call that will call the Intitlaise stack method using the appropiate question type 
            { "addition", InitialiseStack<AdditionQuestion> },
            { "subtraction", InitialiseStack<SubtractionQuestion> },
            { "multiplication", InitialiseStack<MultiplicationQuestion> },
            { "division", InitialiseStack<DivisionQuestion> },
            { "differentiation", InitialiseStack<DifferentiationQuestion> },
            { "integration", InitialiseStack<IntegrationQuestion> },
            { "quadratics", InitialiseStack<QuadraticsQuestion> },
            { "collectingterms", InitialiseStack<CollectingTermsQuestion> },
            { "everything", InitialiseStack<TestEverything> }

        };

        Console.WriteLine("dictionary has been made");
        //initilaising the question stack, if the topic cannot be found an exception will be thrown
        try
        {
            TopicsMapper.TryGetValue("addition", out Func<bool>? intialiseStack);
            Console.WriteLine("dictionary one worked initilaisng the stack");

            AnsweredQuestionStack = new QuestionModels.AnsweredQuestionStack();
            intialiseStack!();
            
            return;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return;
            
        }
    }
    
    
    //using a generic method to initilaise a stack of questions, The type of question will depend on what it is called with from the dictionary but it will correspond to the Question topic 
    private bool InitialiseStack<T>() where T : IQuestion, new() 
    {
        
        int testdifficulty = 1;
        
        QuestionModels.QuestionStack questions = new QuestionModels.QuestionStack();

        //genrating 10 quesitons and pushing them onto the stack
        for (int i = 0; i < 10; i++)
        {
            
            T question = new T();
            question.Difficulty = testdifficulty;
            question.Generate();
            questions.Push(question);
        }
 
        Questions = questions;
        QuestionSequence();
        return true;
    }
    
    //this is used to genrate a round of questions that pulls from every topic
    private bool TestEverything()
    {
        return true;
    }
    
    private void QuestionSequence()
    {
        CurrentQuestion = Questions.Pop(); //getting the next question off the stack
       
        QuestionText = CurrentQuestion.QuestionText; //displaying the new question

        TimeToAnswer.Start(); //starting the timer 
    }
    
    public void AnswerQuestion()
    {
        Console.WriteLine("Questions answered");
        
        //stopping the timer 
        TimeToAnswer.Stop(); 
        
        //initialisng a vairable to keep track of if the user got it right or not 
        bool AnswerCorrect = false;
       
        
        
        Console.WriteLine("user answer:" + UserAnswer);
        //Figuiring out whether the answer is a fraction or a natural number 
        string[] answer = UserAnswer.Split("/");

        
        //this if statement will mark the question approprialtely, whether it is a integer number or not essentially 
        if (answer.Length == 1 && answer[0] != "" ) //if it is integer
        {
            
            Console.WriteLine("Split string" + answer[0]);
            try
            {
                //making a fraction with denominator one which will maintain the value of the natural number
                Fraction input = new Fraction(Convert.ToInt32(answer[0]), 1);
                AnswerCorrect = CurrentQuestion.CheckAnswer(input);
                
                
                //Updating the Progress tracker on the GUI
                CorrectAnswers[questionNum] = AnswerCorrect ? 1 : 2; //assiging an appropraite value as to whether the question was right or wrong
                questionNum++;
                StateHasChanged();//re-rendering the page so that the progress is updated 
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("the answer was lenth one and faile to be parsed");
            }
            
        }else if (answer.Length == 2)//if it is a fraction
        {
            try
            {
                Console.WriteLine("the lenth was 2");
                Fraction input = new Fraction(int.Parse(answer[0]), int.Parse(answer[1]));
                AnswerCorrect = CurrentQuestion.CheckAnswer(input);

                //Updating the Progress tracker on the GUI
                CorrectAnswers[questionNum] = AnswerCorrect ? 1 : 2;
                StateHasChanged();
                questionNum++;
            }
            catch (Exception e)
            {
                Console.WriteLine("the answer was lenth two and faile to be parsed");
            }
            
        }
        else
        {
            AnswerCorrect = false;
            Console.WriteLine("Answeer wrong format ");
            
            //Updating the Progress tracker on the GUI
            questionNum++;
            StateHasChanged();
        }
        
        NextQuestion(AnswerCorrect);
        
        
    }

    private void NextQuestion(bool AnswerCorrect)
    {
        QuestionModels.AnsweredQuestion answeredQuestion = new QuestionModels.AnsweredQuestion(AnswerCorrect , CurrentQuestion.AnswerStringFormat , UserAnswer , CurrentQuestion.QuestionText ,TimeToAnswer.ElapsedMilliseconds);
        AnsweredQuestionStack.Push(answeredQuestion);
        if (Questions.IsEmpty()) 
        {

            Console.WriteLine("Questions stack is now empty");
            StopQuestioning = false;
            SendResultsToAPI(); //round of questioning has finshed time, send results to API then move user onto the summary screen 

        }
        else
        {
            Console.WriteLine("Starting the Question Sequence");   
            QuestionSequence();
        }
    }

    private void  SendResultsToAPI()
    {
        //resetting the array
        for (int i = 0; i < 10; i++)
        {
            CorrectAnswers[i] = 0;
        }
        questionNum = 0;
        


    }
}