using Microsoft.AspNetCore.Mvc;
using Zenith.Application.Repository;
using Zenith.Contracts.Request.Account;
using Zenith.Models.QuestionModels;

namespace ZenithAPI.Controllers;



[ApiController]
[Route("api/Questions")]
public class QuestionStatisticsController : ControllerBase
{
    
    
    private readonly IQuestionStatisticsRepo _questionStatisticsRepo;

    public QuestionStatisticsController(IQuestionStatisticsRepo questionStatisticsRepo)
    {
        _questionStatisticsRepo = questionStatisticsRepo;
    }
    
    
    [HttpPost("AddShortTermData")]
    public async Task<IActionResult> AddQuestioningRound(QuestioningRequests.CompletedQuestionRoundRequest request)
    {
        Console.WriteLine("request received");
        
        //getting the Answered Questions from the request 
        IEnumerable<QuestionModels.AnsweredQuestion> Questions = request.QuestionStack;
        //getting the user ID from the request
        string student = request.UserId;
        
        //getting the statitical inforamtion and putting it into it's own class
        QuestionModels.RoundInfo statistics = new QuestionModels.RoundInfo()
        {
            Difficulty = request.Difficulty,
            Topic = request.Topic,
            TimeCompleted = request.TimeCompleted,
        };

        Console.WriteLine("Attempting the repo");
        //adding the infromation to the database
        bool success = await _questionStatisticsRepo.AddQuestioningRound(Questions, statistics, student);
        return success ? Ok() : BadRequest();
    }

    [HttpPost("AddLongTermData")]
    public async Task<IActionResult> AddLongTermData()
    {
        bool success = await _questionStatisticsRepo.AddLongTermData("21");
        return success ? Ok() : BadRequest();
    }
    
}