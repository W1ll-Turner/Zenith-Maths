using Npgsql;
using Zenith.Application.Database;
using Zenith.Application.Hashing;
using Zenith.Models.QuestionModels;
using Zenith.Models.QuestionModels;
namespace Zenith.Application.Repository;

public class QuestionStatisticsRepo : IQuestionStatisticsRepo
{
    private readonly IDBConnectionFactory _dbConnection;
    private readonly IHashing _Hashing;
    public QuestionStatisticsRepo(IDBConnectionFactory dbConnection, IHashing Hashing) //loosely coupling the databse connection system
    {
        _dbConnection = dbConnection;
        _Hashing = Hashing;
    }
  
    

    public async Task<bool> AddQuestioningRound(QuestionModels.AnsweredQuestionStack questions, QuestionModels.RoundInfo Statistics, string studentId) 
    {
        //this will add all the information about a round of questionsing to the required tables 
        //needs to macth the topic IDs btw
        //calculates the score the user got 
        int score = questions.CalculateScore();

        
        await using (var connection = (NpgsqlConnection)await _dbConnection.CreateDBConnection())
        {
            var AddQuestionsCommand = new NpgsqlCommand("INSERT INTO questionbank(roundid, question, answer, useranswer, correct, timetaken ) VALUES (@roundid, @question, @answer, @useranswer, @correct, @timetaken)");

            for (int i = 1; i < 11; i++) //This will add each question in the AnsweredQuestion Stack that has been given by the front end to the databsde,
            {
                AddQuestionsCommand.Parameters.Clear(); //making no paramters will conflict with previosu executions of the quersy 
                QuestionModels.AnsweredQuestion currentQuestion = questions.Pop();
                
                string roundId = studentId + "#" + i.ToString();
                //dynamically assigning the querys parameters 
                AddQuestionsCommand.Parameters.AddWithValue("@roundid", roundId);
                AddQuestionsCommand.Parameters.AddWithValue("@question", currentQuestion.Question);
                AddQuestionsCommand.Parameters.AddWithValue("@answer", currentQuestion.CorrectAnswer);
                AddQuestionsCommand.Parameters.AddWithValue("@useranswer", currentQuestion.UserAnswer);
                AddQuestionsCommand.Parameters.AddWithValue("@correct", currentQuestion.Correct);
                AddQuestionsCommand.Parameters.AddWithValue("@timetaken" , currentQuestion.TimeTaken);
                
                AddQuestionsCommand.ExecuteNonQuery();
            }
            
            //need to calculaqte the average time as well btw
            
            string ShortTermId = _Hashing.GenerateShortTermStatsID(studentId);
            var AddStatistcsCommand = new NpgsqlCommand("INSERT INTO shorttermstats(shorttermid, averagetime, score, topicid, difficulty, timecompleted) VALUES (@shorttermid, @averagetime, @score, @topicid, @difficulty, @timecompleted)");
            AddStatistcsCommand.Parameters.AddWithValue("@shorttermid", ShortTermId);
            AddStatistcsCommand.Parameters.AddWithValue("@averagetime", );
        }
        
        return true;
    }

    public Task<IEnumerable<QuestionModels>> GetAllRecentQuestionRounds()
    {
        throw new NotImplementedException();
    }
    

    //public Task<IEnumerable<WeeklySummary>> GetAllWeeklySummary()
    //{
    //    throw new NotImplementedException();
    //}

    public Task<bool> DeleteShortTermData()
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteLongTermData()
    {
        throw new NotImplementedException();
    }
}