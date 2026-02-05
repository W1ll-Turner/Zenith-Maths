using Microsoft.AspNetCore.Mvc;
using Zenith.Application.Repository;
using Zenith.Contracts.Request.Account;
using Zenith.Contracts.Response;
using Zenith.Models.Account;
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
    
    //This will add the round o questioning once the user has finsihed one 
    [HttpPost("AddShortTermData")]
    public async Task<IActionResult> AddQuestioningRound(QuestioningRequests.CompletedQuestionRoundRequest request )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        //getting the Answered Questions from the request 
        IEnumerable<QuestionModels.AnsweredQuestion> Questions = request.questions;
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
        return Ok();
    }

    [HttpPost("AddLongTermData/{Id}")]
    public async Task<IActionResult> AddLongTermData([FromRoute] string Id)
    {
        //passing the ID into the databse layer to handle the logic
        bool success = await _questionStatisticsRepo.AddLongTermData(Id);
        return success ? Ok() : BadRequest();
    }
    
    [HttpGet("GetMostRecentQuestionRound/{Id}")]
    public async Task<IActionResult> GetMostRecentQuestionRound([FromRoute] string  Id)
    {
        //passing the Id into the repo to get the data out 
        CompletedRoundOfQuestioning questionRound= await _questionStatisticsRepo.GetMostRecentQuestionRound(Id);

        if (questionRound == null)
        {
            return NotFound();
        }
        
        //mapping to the response model 
        QuestionStatisticResponses.MostRecentQuestionRoundResponse response = new QuestionStatisticResponses.MostRecentQuestionRoundResponse()
            {
                averageTime = Convert.ToString(questionRound.averageTime),
                score = Convert.ToString(questionRound.score),
                topic = questionRound.topic,
                difficulty = Convert.ToString(questionRound.difficulty),
                
                Questions = questionRound.answeredQuestions
            };
        return Ok(response);
    }

    [HttpGet("GetAllQuestioningRounds/{studentId}")]
    public async Task<IActionResult> GetAllQuestioningRounds([FromRoute] string studentId)
    {
        //getting all data from the databse and returinig it 
        IEnumerable<CompletedRoundOfQuestioning> Response = await _questionStatisticsRepo.GetAllQuestionRounds(studentId);
        
        return Ok(Response);
    }
    
    [HttpGet("GetAllweeklySummarys/{studentid}")]
    public async Task<IActionResult> GetAllWeeklySummarys([FromRoute] string studentid)
    {
        IEnumerable<WeeklySummary> Summaries = await _questionStatisticsRepo.GetAllLongTermStats(studentid);
        
        return Ok(Summaries);
    }
}