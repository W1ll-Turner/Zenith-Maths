using Microsoft.AspNetCore.Http;
using Npgsql;
using Zenith.Application.Database;
using Zenith.Application.Hashing;
using Zenith.Application.StatsCalculation;
using Zenith.Models.QuestionModels;
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
                
                //Getting the short terms ID so that the priimary key for each entry in the question bank can be generated 
                string ShortTermId = await _Hashing.GenerateShortTermStatsID(studentId); //getting the shortterm id hash for this entry in the table
                //this will keep trakc of which question is going to be put into the solution 
                int questionNum = 1;
                Console.WriteLine("Attempting to insert into question bank");
                
                //This will add each question in the AnsweredQuestion Stack that has been given by the front end to the databsde,
                foreach (QuestionModels.AnsweredQuestion currentQuestion in questions) 
                {
                    Console.WriteLine("Trying query"); 
                    //clearing the paramters out from the query so that when the next quesion is added no paramters conflict 
                    AddQuestionsCommand.Parameters.Clear(); 
                   

                    string roundId = ShortTermId + "#" + questionNum;
                    questionNum++;
                    //dynamically assigning the querys parameters 
                    AddQuestionsCommand.Parameters.AddWithValue("@roundid", roundId);
                    AddQuestionsCommand.Parameters.AddWithValue("@question", currentQuestion.Question);
                    AddQuestionsCommand.Parameters.AddWithValue("@answer", currentQuestion.CorrectAnswer);
                    AddQuestionsCommand.Parameters.AddWithValue("@useranswer", currentQuestion.UserAnswer);
                    AddQuestionsCommand.Parameters.AddWithValue("@correct", currentQuestion.Correct);
                    AddQuestionsCommand.Parameters.AddWithValue("@timetaken", currentQuestion.TimeTaken);


                    Console.WriteLine("parameters intitialised");
                    await AddQuestionsCommand.ExecuteNonQueryAsync();
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
                
                
                //adding the summary statistics for the round to the table  
                //paramertised SQL query working across the shorttermstats table and the topics table to store the relevant data
                var AddStatistcsCommand = new NpgsqlCommand("INSERT INTO shorttermstats(shorttermid, averagetime, score, topicid, difficulty, timecompleted) SELECT @shorttermid, @averagetime, @score, topicid , @difficulty, @timecompleted FROM topic WHERE topicname = 'addition'", connection);
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
                await AddRelationCommand.ExecuteNonQueryAsync();
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

    public Task<IEnumerable<QuestionModels>> GetAllRecentQuestionRounds(string studentId)
    {
        //needs to empty out the short term stats table and the question bank table 
        
        
        throw new NotImplementedException();
    }

    
    //This method will be used to get all of the long terms statistics to the user and send them to the dashboard so that they can be displayed 
    public async Task<IEnumerable<WeeklySummary>> GetAllLongTermStats(string studentId)
    {
        
        //connecting to the database
        await using (var connection = (NpgsqlConnection)await _dbConnection.CreateDBConnection())
        {
            //creating the SQL query to read the data from the databse 
            var command = new NpgsqlCommand("SELECT * FROM longtermstats WHERE longtermstatsid LIKE @studentid'%' ", connection);
            command.Parameters.AddWithValue("@studentid", studentId);

            using (var reader = await command.ExecuteReaderAsync())
            {
                List<WeeklySummary> weeklySummarys = new List<WeeklySummary>();
                
                while (await reader.ReadAsync())
                {
                    string ID = reader.GetString(0);
                    string[] splitID = ID.Split('#');
                    string weekNumber = splitID[1];
                  
                    string[] topicNames = new string[2];
                    //this command will get the appropriate topic name from the topics table
                    var getTopicCommand = new NpgsqlCommand("SELECT topicname FROM topic WHERE topicid = @topicid ", connection);
                    //executing the command twice to get the two Topic names out 
                    for (int i = 0; i < 2; i++)
                    {
                        getTopicCommand.Parameters.AddWithValue("@topicid", reader.GetString(3+i));
                        using (NpgsqlDataReader topicReader = await getTopicCommand.ExecuteReaderAsync())
                        {
                            topicNames[i] = topicReader[0] as string;
                        }
                    }
                    
                    //creating the object that stores that weeks summary statistics 
                    WeeklySummary week = new WeeklySummary()
                    {
                        weekNumber = weekNumber,
                        averageTime = reader.GetDouble(1),
                        averageScore = reader.GetDouble(2),
                        worstTopic = topicNames[0],
                        bestTopic = topicNames[1],
                        completion = reader.GetString(5),
                        
                    };
                    weeklySummarys.Add(week);
                }
                
                //putting the list into a from that can be serialised for JSON and returning it 
                IEnumerable<WeeklySummary> weeklySummary = weeklySummarys;
                return weeklySummary;
                
            }
        }
    }

    public async Task<CompletedRoundOfQuestioning> GetMostRecentQuestionRound(string studentId)
    {
        await using (var connection = (NpgsqlConnection)await _dbConnection.CreateDBConnection())
        {
            //This will get the shorterm ID of all the rounds of queastiopning recently answered by that students 
            var command = new NpgsqlCommand("SELECT shorttermid FROM shorttermstatsbridge WHERE studentid = @studentid ORDER BY shorttermid DESC;", connection);
            command.Parameters.AddWithValue("@studentid", studentId);

            string Id;
            //Getting only the most recent shorttermt id 
            using (var reader = await command.ExecuteReaderAsync())
            {
                Id = reader.GetString(0);
            }
            
            //Getting the information from the shortterms stats that corresponds with the most recently asnwered question
            var getRoundStatisticsCommand = new NpgsqlCommand("SELECT averagetime, score , topicid , difficulty FROM shorttermstats WHERE shorttermid = @shorttermid", connection);
            getRoundStatisticsCommand.Parameters.AddWithValue("@shorttermid", Id);
            
            using (var reader = await getRoundStatisticsCommand.ExecuteReaderAsync())
            {
                
                var getQuestionsCommand = new NpgsqlCommand("SELECT * FROM questionbank WHERE roundid LIKE @shorttermid", connection);
                //This will get all the entries as it will be the shorttermid followed by the quesiton number 
                getQuestionsCommand.Parameters.AddWithValue("@shorttermid", Id + "%");
                
                
                List<QuestionModels.AnsweredQuestion> questions = new List<QuestionModels.AnsweredQuestion>();
                using (var questionReader = await getQuestionsCommand.ExecuteReaderAsync())
                {
                    while (await questionReader.ReadAsync())
                    {
                        QuestionModels.AnsweredQuestion question = new QuestionModels.AnsweredQuestion()
                        {
                            Correct = questionReader.GetBoolean(4),
                            CorrectAnswer = questionReader.GetString(2),
                            UserAnswer = questionReader.GetString(3),
                            Question = questionReader.GetString(1),
                            TimeTaken = questionReader.GetDouble(5),
                        };
                        
                        questions.Add(question);
                    }
                    
                    
                }
                
                //getting the topic 
                var GetTopicCommand = new NpgsqlCommand("SELECT * FROM topic WHERE topicid = @topicid", connection);
                GetTopicCommand.Parameters.AddWithValue("@topicid", reader.GetInt32(2));
                string topic;
                using (var topicReader = await GetTopicCommand.ExecuteReaderAsync())
                {
                    topic = topicReader[0] as string;
                }
                
                //creating the return object 
                CompletedRoundOfQuestioning round = new CompletedRoundOfQuestioning()
                {
                    averageTime = reader.GetDouble(0),
                    score = reader.GetInt32(1),
                    topic = topic,
                    difficulty = reader.GetInt32(3),
                    
                    answeredQuestions = questions
                };

                return round;
            }
            
        }
        
        
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

        
        List<shorttermsstatsinfo> topic1 = new List<shorttermsstatsinfo>();
        List<shorttermsstatsinfo> topic2 = new List<shorttermsstatsinfo>();
        List<shorttermsstatsinfo> topic3 = new List<shorttermsstatsinfo>();
        List<shorttermsstatsinfo> topic4 = new List<shorttermsstatsinfo>();
        List<shorttermsstatsinfo> topic5 = new List<shorttermsstatsinfo>();
        List<shorttermsstatsinfo> topic6 = new List<shorttermsstatsinfo>();
        List<shorttermsstatsinfo> topic7 = new List<shorttermsstatsinfo>();
        List<shorttermsstatsinfo> topic8 = new List<shorttermsstatsinfo>();
        List<shorttermsstatsinfo> topic9 = new List<shorttermsstatsinfo>();
        
        //pulling all data from the shorttermstatrs table 
        await using (var connection = (NpgsqlConnection)await _dbConnection.CreateDBConnection())
        {
            //the query to access the data in the shorttermsats table 
            var GetShorttermInfoCommand = new NpgsqlCommand("SELECT averagetime, score, topicid, difficulty  FROM shorttermstats WHERE shorttermid = @shorttermid", connection);

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
                            topic1.Add(info);
                        }else if (topicid == 2)
                        {
                            topic2.Add(info);
                        }else if (topicid == 3)
                        {
                            topic3.Add(info);
                        }else if (topicid == 4)
                        {
                            topic4.Add(info);
                        }else if (topicid == 5)
                        {
                            topic5.Add(info);
                        }else if (topicid == 6)
                        {
                            topic6.Add(info);
                        }else if (topicid == 7)
                        {
                            topic7.Add(info);
                        }else if (topicid == 8)
                        {
                            topic8.Add(info);
                        }else if (topicid == 9)
                        {
                            topic9.Add(info);
                        }
                        
                    }
                    
                }
            }
            
        }
        
        //calculating the Averages for each topic 
        
        
        TopicAverages[] averages =
        [
            await _StatsCalculation.CalculateTopicAverages(topic1),
            await _StatsCalculation.CalculateTopicAverages(topic2),
            await _StatsCalculation.CalculateTopicAverages(topic3),
            await _StatsCalculation.CalculateTopicAverages(topic4),
            await _StatsCalculation.CalculateTopicAverages(topic5),
            await _StatsCalculation.CalculateTopicAverages(topic6),
            await _StatsCalculation.CalculateTopicAverages(topic7),
            await _StatsCalculation.CalculateTopicAverages(topic8),
            await _StatsCalculation.CalculateTopicAverages(topic9),
        ];
        
        //calculting the completion for each topic 
        double[] CompletionResults = new double[9];
        for (int i = 0; i < CompletionResults.Length; i++)
        {
            CompletionResults[i] = await _StatsCalculation.CalculateTopicCompletion(averages[i]);
        }
        
        //initialising the variables that will be added to the database  
        double completion = await _StatsCalculation.CalclulateOverallCompletion(CompletionResults);
        int worstTopicId = await _StatsCalculation.GetWorstTopicID(CompletionResults);
        int bestTopicId = await _StatsCalculation.GetBestTopicID(CompletionResults);
        double[] averges = await _StatsCalculation.CalculateAverageTimeAndScore(averages);
        //getting the primary key for the table 
        string longTermsStatsId = await _Hashing.GenerateLongTermStatsID(ID);
        
        //opening the databse connection so the entry can be added to the table 
        using (var connection = (NpgsqlConnection)await _dbConnection.CreateDBConnection())
        {
            var AddToLongTermStats = new NpgsqlCommand("INSERT INTO longtermstats(longtermstatid, averagetime, averagescore, worsttopicid, besttopicid, completion) VALUES(@key, @averagetime, @averagescore, @worsttopicid, @besttopicid, @completion )", connection);
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
    
    
    public Task<bool> DeleteShortTermData()
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteLongTermData()
    {
        throw new NotImplementedException();
    }
   
}