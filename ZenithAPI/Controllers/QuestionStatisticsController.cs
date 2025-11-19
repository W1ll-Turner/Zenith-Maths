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
    
    
    [HttpPost("Add")]
    public async Task<IActionResult> AddQuestioningRound(QuestioningRequests.CompletedQuestionRoundRequest request)
    {
        
        QuestionModels.AnsweredQuestionStack Questions = request.QuestionStack;
        string student = request.UserId;
        QuestionModels.RoundInfo statistics = new QuestionModels.RoundInfo()
        {
            Difficulty = request.Difficulty,
            Topic = request.Topic,
            TimeCompleted = request.TimeCompleted,
        };
        
        bool success = await _questionStatisticsRepo.AddQuestioningRound(Questions, statistics, student);
        return success ? Ok() : BadRequest();
    }
    
    
}