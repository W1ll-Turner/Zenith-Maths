using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Zenith.Contracts.Request.Account;
using Zenith.Models.Account;
using Zenith.Models.QuestionModels;

namespace ZenithFrontEnd.Components.Pages.Questions;

public partial class ComplexQuestionAnsweringPage : ComponentBase
{
    [Parameter]
    public int Difficulty { get; set; }
    
    [Parameter]
    public string Topic { get; set; } = null!;
    private string QuestionText { get; set; } = "";
    private string UserAnswer { get; set; } = "";
    private Stopwatch TimeToAnswer { get; set; } = new Stopwatch();
    
    private QuestionModels.QuestionStack<string> Questions { get; set; } = new QuestionModels.QuestionStack<string>();
    private IQuestion<string> CurrentQuestion { get; set; }
    private List<QuestionModels.AnsweredQuestion> AnsweredQuestions = new List<QuestionModels.AnsweredQuestion>(); 
    public Dictionary<string, Func<bool>> TopicsMapper { get; set; }
    
    public bool StopQuestioning { get; set; } = false;

    public int[] CorrectAnswers { get; set; } = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
    private int questionNum = 0;
    private bool authenticated = false;
    private string StudentId;

    protected override async Task OnAfterRenderAsync(bool firstRender) //This is getting the user's ID from local storage, to make sure it is ready to be passed into the API calls
    {
        if (firstRender)
        {
            string Id = await GetId();
            Console.WriteLine(Id);
            if (Id == null)
            {
                authenticated = false;
            }
            else
            {
                authenticated = true;
            }

            StateHasChanged();
        }
    }
    
    private void Start()
    {
        StopQuestioning = true;
        Console.WriteLine("started");
        //this dictionary maps the topic to the genric methods that will generate the questions, This is dependant on a topic
        TopicsMapper = new Dictionary<string, Func<bool>>()
        {
            {"quadratic", InitialiseQuestionStack<QuadraticQuestion>},
            { "collectingterms", InitialiseQuestionStack<CollectingTermsQuestion> },
            
        };
        
        Topic = "quadratic";
        try
        {
            TopicsMapper.TryGetValue(Topic, out Func<bool>? intialiseStack);
            Console.WriteLine("dictionary one worked initilaisng the stack");
            intialiseStack!();
            return;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return;
            
        }
    }

    private bool InitialiseQuestionStack<TQuestion>() where TQuestion : IQuestion<string>, new()
    {
        
        Difficulty = 2;
        
        
        QuestionModels.QuestionStack<string> questions = new QuestionModels.QuestionStack<string>();

        //genrating 10 quesitons and pushing them onto the stack
        for (int i = 0; i < 10; i++)
        {

            TQuestion question = new TQuestion();
            question.Difficulty = Difficulty;
            question.Generate();
            questions.Push(question);
        }
 
        Questions = questions;
        QuestionSequence();
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
        //stopping the timer 
        TimeToAnswer.Stop(); 
        
        //initialisng a vairable to keep track of if the user got it right or not 
       
        
        //add some code to chekc the answers
        bool answerCorrect = CurrentQuestion.CheckAnswer(UserAnswer);
        CorrectAnswers[questionNum] = answerCorrect? 1:2;
        questionNum++;
        StateHasChanged();
        NextQuestion(answerCorrect);
        
        
    }
    
    private void NextQuestion(bool AnswerCorrect)
    {
        QuestionModels.AnsweredQuestion answeredQuestion = new QuestionModels.AnsweredQuestion
        {
            Correct = AnswerCorrect,
            CorrectAnswer = CurrentQuestion.AnswerStringFormat,
            UserAnswer = UserAnswer,
            Question = CurrentQuestion.QuestionText,
            TimeTaken = TimeToAnswer.ElapsedMilliseconds/1000.0 //converting the milliseconds into seconds 
        };
        AnsweredQuestions.Add(answeredQuestion);
        TimeToAnswer.Reset();
        
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
    
    private async Task  SendResultsToAPI()
    {
        //resetting the array keeping track of the user's answers 
        ResetArray();
        
        //pulling the User ID from the session storage
        
        //getting the current time 
        DateTime temporaryTimeHolder = DateTime.Now;
        string time = temporaryTimeHolder.ToString("HH:mm:ss");
        
        //putting the list of answered questions into an IEnumerbale 
        
        //intitialising the request object to be sent to the API putting fake test data in for now 
        QuestioningRequests.CompletedQuestionRoundRequest request = new QuestioningRequests.CompletedQuestionRoundRequest()
        {
            Difficulty = 1,
            UserId = StudentId,
            Topic = "addition",
            TimeCompleted = time,
            questions =  AnsweredQuestions
        };
       
       
       
        //sending the request to the API to store the round of questioning in the database
        Console.WriteLine("Trying to send therequest");
       
        HttpResponseMessage response = await Http.PostAsJsonAsync("http://localhost:5148/api/Questions/AddShortTermData", request);
 
        NavigationManager.NavigateTo("/RoundComplete");
        

    }
    
    private void ResetArray()
    {
        for (int i = 0; i < 10; i++)
        {
            CorrectAnswers[i] = 0;
        }
        questionNum = 0;
        
    }

    private async Task<string> GetId()
    {
        ProtectedBrowserStorageResult<string> Id = await SessionStorage.GetAsync<string>("Id");
        if (Id.Success)
        {
            Console.WriteLine("There is an ID");
            return Id.Value;
        }
        
        return null;
    }
    
}

