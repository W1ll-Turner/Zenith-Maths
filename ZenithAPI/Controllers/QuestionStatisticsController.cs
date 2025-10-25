using Microsoft.AspNetCore.Mvc;
using Zenith.Application.Repository;
using Zenith.Contracts.Request.Account;
using Zenith.Models.QuestionModels;

namespace ZenithAPI.Controllers;

public class QuestionStatisticsController : ControllerBase
{
    private readonly IQuestionStatisticsRepo _questionStatisticsRepo;

    public QuestionStatisticsController(IQuestionStatisticsRepo questionStatisticsRepo)
    {
        _questionStatisticsRepo = questionStatisticsRepo;
    }


    [HttpPost]
    public async Task<bool> AddQuestioningRound(QuestioningRequests.CompletedQuestionRoundRequest request)
    {
        
    }
    
    
}