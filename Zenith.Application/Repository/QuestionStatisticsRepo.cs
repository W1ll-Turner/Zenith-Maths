using Microsoft.AspNetCore.Http;
using Npgsql;
using Zenith.Application.Database;
using Zenith.Application.Hashing;
using Zenith.Application.StatsCalculation;
using Zenith.Models.QuestionModels;
using Zenith.Models.QuestionModels;
using Zenith.Models;
using Zenith.Models.Account;

namespace Zenith.Application.Repository;

public class QuestionStatisticsRepo : IQuestionStatisticsRepo
{
    private readonly IDBConnectionFactory _dbConnection;
    private readonly IHashing _Hashing;
    private readonly IStatsCalculation _StatsCalculation;
    public QuestionStatisticsRepo(IDBConnectionFactory dbConnection, IHashing Hashing, IStatsCalculation statsCalculation) //loosely coupling the databse connection system and the hasing algorithms 
    {
        _dbConnection = dbConnection;
        _Hashing = Hashing;
        _StatsCalculation = statsCalculation;
    }
  
    

    public async Task<bool> AddQuestioningRound(IEnumerable<QuestionModels.AnsweredQuestion> questions, QuestionModels.RoundInfo Statistics, string studentId) 
    {
        //this will add all the information about a round of questionsing to the required tables 
        //needs to macth the topic IDs btw
        try
        {
            await using (var connection = (NpgsqlConnection)await _dbConnection.CreateDBConnection())
            {
                
                //paramertarised SQL query which will add each question from the round to the question bank table
                var AddQuestionsCommand = new NpgsqlCommand("INSERT INTO questionbank(roundid, question, answer, useranswer, correct, timetaken ) VALUES (@roundid, @question, @answer, @useranswer, @correct, @timetaken)", connection);

                int questionNum = 1;
                Console.WriteLine("Attempting to insert into question bank");
                //This will add each question in the AnsweredQuestion Stack that has been given by the front end to the databsde,
                foreach (QuestionModels.AnsweredQuestion currentQuestion in questions) 
                {
                    Console.WriteLine("Trying query"); 
                    AddQuestionsCommand.Parameters.Clear(); //making no paramters will conflict with previosu executions of the quersy 
                   

                    string roundId = studentId + "#" + questions.ToString();
                    //dynamically assigning the querys parameters 
                    AddQuestionsCommand.Parameters.AddWithValue("@roundid", roundId);
                    AddQuestionsCommand.Parameters.AddWithValue("@question", currentQuestion.Question);
                    AddQuestionsCommand.Parameters.AddWithValue("@answer", currentQuestion.CorrectAnswer);
                    AddQuestionsCommand.Parameters.AddWithValue("@useranswer", currentQuestion.UserAnswer);
                    AddQuestionsCommand.Parameters.AddWithValue("@correct", currentQuestion.Correct);
                    AddQuestionsCommand.Parameters.AddWithValue("@timetaken", currentQuestion.TimeTaken);


                    Console.WriteLine("parameters intitialised");
                    await AddQuestionsCommand.ExecuteNonQueryAsync();
                    questionNum++;
                }
                
                //getting the summary statistics 
                //calculating the avergae time 
                double totalTime = 0;
                foreach (QuestionModels.AnsweredQuestion question in questions)
                {
                    totalTime += question.TimeTaken;
                }
                double averageTime = totalTime / questionNum;
                
                //calculating the score 
                int score = 0;
                foreach (QuestionModels.AnsweredQuestion question in questions)
                {
                    if (question.Correct == true)
                    {
                        score++;
                    }
                }
                
                string ShortTermId = await _Hashing.GenerateShortTermStatsID(studentId); //getting the shortterm id hash for this entry in the table
                
                //adding the summary statistics for the round to the table  
                //paramertised SQL query working across the shorttermstats table and the topics table to store the relevant data
                var AddStatistcsCommand = new NpgsqlCommand("INSERT INTO shorttermstats(shorttermid, averagetime, score, topicid, difficulty, timecompleted) VALUES (@shorttermid, @averagetime, @score, SELECT topicid FROM topic WHERE topicname = 'addition' , @difficulty, @timecompleted)");
                AddStatistcsCommand.Parameters.AddWithValue("@shorttermid", ShortTermId);
                AddStatistcsCommand.Parameters.AddWithValue("@averagetime", averageTime);
                AddStatistcsCommand.Parameters.AddWithValue("@score", score);
                AddStatistcsCommand.Parameters.AddWithValue("@difficulty", Statistics.Difficulty);
                AddStatistcsCommand.Parameters.AddWithValue("@timecompleted", Statistics.TimeCompleted);
                await AddStatistcsCommand.ExecuteNonQueryAsync();
                
                //adding this relation to the shortterterms stast bridge so it can be accessed properly later
                var AddRelationCommand = new NpgsqlCommand("INSERT INTO shorttermstatsbridge(studentid, shorttermid) VALUES (@studentid, @shorttermid)", connection);
                AddRelationCommand.Parameters.AddWithValue("@studentid", studentId);
                AddRelationCommand.Parameters.AddWithValue("@shorttermid", ShortTermId);
            }
            return true;

        }
        catch (Exception ex) // excpetion handling the qury, if somehting goes wrong it will return false to say it did not work
        {
            Console.WriteLine(ex);
            Console.WriteLine(ex.Message);
            Console.WriteLine("repo failed");
            return false;
        }
        
        
        
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

    //this will move all of the information form the shorttterm statistics table into the longterm stats table where the stast will be calulated and the week summarised 
    public async Task<bool> AddLongTermData(string ID)
    {
        
        //This is where all of the stats calcualtaions will be done 
        //needs to pull data from short term relations and pool it into the longterm stuff
        
        
        List<string> Keys = new List<string>();
        //pulling all short term stats relatyions from that table 
        await using (var connection = (NpgsqlConnection)await _dbConnection.CreateDBConnection())
        {
            var GetQuestionRelationsCommand = new NpgsqlCommand("SELECT shorttermid FROM  shorttermstatsbridge WHERE studentid = @studentid", connection);
            GetQuestionRelationsCommand.Parameters.AddWithValue("@studentid", ID);

            
            //reading every key from the table and storing it in a list to be used to access the data itself 
            await using (var reader = await GetQuestionRelationsCommand.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    Keys.Add(reader.GetString(0));
                }
            }
            
        }

        
        List<shorttermsstatsinfo> Topic1 = new List<shorttermsstatsinfo>();
        List<shorttermsstatsinfo> Topic2 = new List<shorttermsstatsinfo>();
        List<shorttermsstatsinfo> Topic3 = new List<shorttermsstatsinfo>();
        List<shorttermsstatsinfo> Topic4 = new List<shorttermsstatsinfo>();
        List<shorttermsstatsinfo> Topic5 = new List<shorttermsstatsinfo>();
        List<shorttermsstatsinfo> Topic6 = new List<shorttermsstatsinfo>();
        List<shorttermsstatsinfo> Topic7 = new List<shorttermsstatsinfo>();
        List<shorttermsstatsinfo> Topic8 = new List<shorttermsstatsinfo>();
        List<shorttermsstatsinfo> Topic9 = new List<shorttermsstatsinfo>();
        
        //pulling all data from the shorttermstatrs table 
        await using (var connection = (NpgsqlConnection)await _dbConnection.CreateDBConnection())
        {
            //the query to access the data in the shorttermsats table 
            var GetShorttermInfoCommand = new NpgsqlCommand("SELECT averagetime, score, topicid, difficulty  FROM shorttermstatsbridge WHERE shorttermid = @shorttermid", connection);

            //executing the query for each key 
            foreach (var Key in Keys)
            {
                GetShorttermInfoCommand.Parameters.AddWithValue("@shorttermid", Key);
                
                await using (var reader = await GetShorttermInfoCommand.ExecuteReaderAsync())
                {
                    //reading all of the data from the selcted field and putting it into a List corresponing to the topic 
                    while (await reader.ReadAsync())
                    {
                        //fiding out whihc topic the current feild of data belongs to  
                        int topicid = reader.GetInt32(2);
                        shorttermsstatsinfo info = new shorttermsstatsinfo()
                        {
                            averagetime = reader.GetDouble(0),
                            score = reader.GetInt32(1),
                            difficulty = reader.GetInt32(3),
                        };
                        
                        //assigning the data to a relvant list 
                        if (topicid == 1)
                        {
                            Topic1.Add(info);
                        }else if (topicid == 2)
                        {
                            Topic2.Add(info);
                        }else if (topicid == 3)
                        {
                            Topic3.Add(info);
                        }else if (topicid == 4)
                        {
                            Topic4.Add(info);
                        }else if (topicid == 5)
                        {
                            Topic5.Add(info);
                        }else if (topicid == 6)
                        {
                            Topic6.Add(info);
                        }else if (topicid == 7)
                        {
                            Topic7.Add(info);
                        }else if (topicid == 8)
                        {
                            Topic8.Add(info);
                        }else if (topicid == 9)
                        {
                            Topic9.Add(info);
                        }
                        
                    }
                    
                }
            }
            
        }
        
        //calculating the Averages for each topic 
        TopicAverages emptyTopic = new TopicAverages()
        {
            averageScore = 0.0,
            averageTime = 0.0,
            difficulty = 0,
            numberOfRounds = 0,
                
        };
        
        TopicAverages[] Averages =
        [
            await _StatsCalculation.CalculateTopicAverages(Topic1),
            await _StatsCalculation.CalculateTopicAverages(Topic2),
            await _StatsCalculation.CalculateTopicAverages(Topic3),
            await _StatsCalculation.CalculateTopicAverages(Topic4),
            await _StatsCalculation.CalculateTopicAverages(Topic5),
            await _StatsCalculation.CalculateTopicAverages(Topic6),
            await _StatsCalculation.CalculateTopicAverages(Topic7),
            await _StatsCalculation.CalculateTopicAverages(Topic8),
            await _StatsCalculation.CalculateTopicAverages(Topic9)
        ];
        
        //calculting the completion for each topic 
        double[] CompletionResults = new double[9];
        for (int i = 0; i < CompletionResults.Length; i++)
        {
            
            CompletionResults[i] = await _StatsCalculation.CalculateTopicCompletion(Averages[i]);
        }
        
        //initialising the variables that will be added to the database  
        double completion = await _StatsCalculation.CalclulateOverallCompletion(CompletionResults);
        int worstTopicId = await _StatsCalculation.GetWorstTopicID(CompletionResults);
        int bestTopicId = await _StatsCalculation.GetBestTopicID(CompletionResults);
        double[] averges = await _StatsCalculation.CalculateAverageTimeAndScore(Averages);
        //getting the primary key for the table 
        string longTermsStatsId = await _Hashing.GenerateLongTermStatsID(ID);
        
        //opening the databse connection so the entry can be added to the table 
        using (var connection = (NpgsqlConnection)await _dbConnection.CreateDBConnection())
        {
            var AddToLongTermStats = new NpgsqlCommand("INSERT INTO longtermstat(longtermstatid, averagetime, averagescore, worsttopicid, besttopicid, completion) VALUES(@key, @averagetime, @averagescore, @worsttopic, @besttopic, @completion )", connection);
            AddToLongTermStats.Parameters.AddWithValue("@key", longTermsStatsId);
            AddToLongTermStats.Parameters.AddWithValue("@averagetime", averges[1]);
            AddToLongTermStats.Parameters.AddWithValue("@averagescore", averges[0]);
            AddToLongTermStats.Parameters.AddWithValue("@worsttopicid", worstTopicId);
            AddToLongTermStats.Parameters.AddWithValue("@besttopicid", bestTopicId);
            AddToLongTermStats.Parameters.AddWithValue("@completion", completion);
            
            AddToLongTermStats.ExecuteNonQuery();
        }

        return true;

    }
    
   
}