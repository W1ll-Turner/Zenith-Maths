using Npgsql;
using Zenith.Application.Database;
using Zenith.Application.Hashing;
using Zenith.Application.StatsCalculation;
using Zenith.Models.QuestionModels;
using Zenith.Models.Account;
namespace Zenith.Application.Repository;

//inheriting from the Interface so the class can be properly implemted
public class QuestionStatisticsRepo : IQuestionStatisticsRepo
{
    private readonly IDBConnectionFactory _dbConnection;
    private readonly IHashing _hashing;
    private readonly IStatsCalculation _statsCalculation;
    
    //loosely coupling the required modules, Hashing, stats calculation and dbconnection
    public QuestionStatisticsRepo(IDBConnectionFactory dbConnection, IHashing hashing, IStatsCalculation statsCalculation) 
    {
        _dbConnection = dbConnection;
        _hashing = hashing;
        _statsCalculation = statsCalculation;
    }
    
    //this will add rounds of questioning the user has just completed to the rounds 
    public async Task<bool> AddQuestioningRound(IEnumerable<QuestionModels.AnsweredQuestion> questions, QuestionModels.RoundInfo Statistics, string studentId) 
    {
        //putting the while method into expception handling so that if a quiery fails a valid retrun is still given 
        try
        {
            //connecting to the database 
            await using (var connection = (NpgsqlConnection)await _dbConnection.CreateDBConnection())
            {
                //This query will add each question from the request to the questionbank
                var addQuestionsCommand = new NpgsqlCommand("INSERT INTO questionbank(roundid, question, answer, useranswer, correct, timetaken ) VALUES (@roundid, @question, @answer, @useranswer, @correct, @timetaken)", connection);
                
                //getting the primary key for both tables
                string shortTermId = await _hashing.GenerateShortTermStatsId(studentId);
                
                //this will keep track of which question is going to be put into the solution 
                int questionNum = 1;
               
                //This loop will add each indiviual question to the question bank table
                foreach (QuestionModels.AnsweredQuestion currentQuestion in questions) 
                {
                    //clearing the paramters out from the query so that when the next question is added, no parameters conflict 
                    addQuestionsCommand.Parameters.Clear(); 
                    
                    //creating the primary key for the table 
                    string roundId = shortTermId + "#" + questionNum;
                    questionNum++;
                    
                    //dynamically assigning the query's parameters 
                    addQuestionsCommand.Parameters.AddWithValue("@roundid", roundId);
                    addQuestionsCommand.Parameters.AddWithValue("@question", currentQuestion.Question);
                    addQuestionsCommand.Parameters.AddWithValue("@answer", currentQuestion.CorrectAnswer);
                    addQuestionsCommand.Parameters.AddWithValue("@useranswer", currentQuestion.UserAnswer);
                    addQuestionsCommand.Parameters.AddWithValue("@correct", currentQuestion.Correct);
                    addQuestionsCommand.Parameters.AddWithValue("@timetaken", currentQuestion.TimeTaken);

                    //executing the query
                    await addQuestionsCommand.ExecuteNonQueryAsync();
                }
                //calculating the average time 
                double totalTime = 0;
                foreach (QuestionModels.AnsweredQuestion question in questions)
                {
                    totalTime += question.TimeTaken;
                }
                double averageTime = totalTime / questionNum;
                //calculating the average score 
                int score = 0;
                foreach (QuestionModels.AnsweredQuestion question in questions)
                {
                    if (question.Correct == true)
                    {
                        score++;
                    }
                }
                //cross parameterised SQL query working to add the information to the shortterm stats table, works acrss the topic and shorttermstats table 
                var addStatistcsCommand = new NpgsqlCommand("INSERT INTO shorttermstats(shorttermid, averagetime, score, topicid, difficulty, timecompleted) SELECT @shorttermid, @averagetime, @score, topicid , @difficulty, @timecompleted FROM topic WHERE topicname = 'addition'", connection);
                //assgingin the parameters 
                addStatistcsCommand.Parameters.AddWithValue("@shorttermid", shortTermId);
                addStatistcsCommand.Parameters.AddWithValue("@averagetime", averageTime);
                addStatistcsCommand.Parameters.AddWithValue("@score", score);
                addStatistcsCommand.Parameters.AddWithValue("@difficulty", Statistics.Difficulty);
                addStatistcsCommand.Parameters.AddWithValue("@timecompleted", Statistics.TimeCompleted);
                //executing the query 
                await addStatistcsCommand.ExecuteNonQueryAsync();
                
                //adding the relation to the shortterterms stast bridge so it can be accessed properly later
                var addRelationCommand = new NpgsqlCommand("INSERT INTO shorttermstatsbridge(studentid, shorttermid) VALUES (@studentid, @shorttermid)", connection);
                //assigning parameters
                addRelationCommand.Parameters.AddWithValue("@studentid", studentId);
                addRelationCommand.Parameters.AddWithValue("@shorttermid", shortTermId);
                //executing query
                await addRelationCommand.ExecuteNonQueryAsync();
            }
            return true;
        }
        catch (Exception ex) 
        {
            //writing the messsage so it can be used for review
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    //this will get all of the recent question rounds answered by a particular user
    public async Task<IEnumerable<CompletedRoundOfQuestioning>> GetAllQuestionRounds(string studentId)
    {
        //initialsing a list of primary keys that will be used to access the data 
        List<string> keys = new List<string>();
        
        //intitialisng databse conneciton 
        await using (var connection = (NpgsqlConnection)await _dbConnection.CreateDBConnection())
        {
            //this query will get all of the keys related to the user's recent round of questions that have been answered
            var getKeysCommand = new NpgsqlCommand("SELECT shorttermid FROM shorttermstatsbridge WHERE studentid = @studentid", connection);
            //adding parameters 
            getKeysCommand.Parameters.AddWithValue("@studentid", studentId);
            //executing the command and reading all the keys that have been returned
            using (var reader = await getKeysCommand.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    keys.Add(reader.GetString(0));
                }
                //if no keys where found the method should stop running
                if (keys.Count == 0)
                {
                    return null;
                }
            }
        }
        //initialsing a list of question rounds 
        List<CompletedRoundOfQuestioning> completedRounds = new List<CompletedRoundOfQuestioning>();
        //will execute the method to get a round of questioning for each key in the list
        foreach (var key in keys)
        {
            //adding a round of questioing to the list 
            completedRounds.Add(await GetRoundOfQuestioning(key));
        }
        
        //Serialising to an IEnumerable for easier data transmission
        IEnumerable<CompletedRoundOfQuestioning> AllQuestioningRounds = completedRounds;
        return AllQuestioningRounds;
        
    }
    
    //This method will be used to get all of the long terms statistics to the user and send them to the dashboard so that they can be displayed 
    public async Task<IEnumerable<WeeklySummary>> GetAllLongTermStats(string studentId)
    {
        //connecting to the database
        await using (var connection = (NpgsqlConnection)await _dbConnection.CreateDBConnection())
        {
            //creating the SQL query to read the data from the databse 
            var command = new NpgsqlCommand("SELECT lst.longtermstatid, lst.averagetime, lst.averagescore , lst.completion,  wst.topicname , bst.topicname, lst.averagedifficulty FROM longtermstats lst JOIN topic wst ON wst.topicid = lst.worsttopicid JOIN topic bst ON bst.topicid = lst.besttopicid WHERE lst.longtermstatid LIKE @studentid ", connection);
            command.Parameters.AddWithValue("@studentid", studentId + "%");
            
            
            List<WeeklySummary> weeklySummarys = new List<WeeklySummary>();
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    string ID = reader.GetString(0);
                    string[] splitID = ID.Split('#');
                    string weekNumber = splitID[1];
                    
                    //creating the object that stores that weekly summary statistics 
                    WeeklySummary week = new WeeklySummary()
                    {
                        weekNumber = weekNumber,
                        averageTime = reader.GetDouble(1),
                        averageScore = reader.GetDouble(2),
                        worstTopic = reader.GetString(4),
                        bestTopic = reader.GetString(5),
                        completion = reader.GetDouble(3),
                        difficulty = reader.GetString(6),
                        
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
            //removing any white space from the string
            studentId = studentId.Trim();
            Console.WriteLine(studentId);
            //This will get the shorterm ID of all the rounds of queastiopning recently answered by that students 
            var command = new NpgsqlCommand("SELECT shorttermid FROM shorttermstatsbridge WHERE studentid = @studentid ORDER BY shorttermid DESC LIMIT 1;", connection);
            command.Parameters.AddWithValue("@studentid", studentId);

            string Id = "";
            //Getting only the most recent shorttermt id 
            using (var reader = await command.ExecuteReaderAsync())
            {
                try
                {
                    reader.Read();
                    Id = reader.GetString(0);
                }catch(Exception ex)
                {
                    return null;
                }
                
            }

            Console.WriteLine("got short term ID");
            Console.WriteLine(Id);
            Console.WriteLine("that was the ID");
            
            return await GetRoundOfQuestioning(Id);
            
        }
        
        
    }


    private async Task<CompletedRoundOfQuestioning> GetRoundOfQuestioning(string Id)
    {
        using (var connection = (NpgsqlConnection)await _dbConnection.CreateDBConnection())
        {
            var getRoundStatisticsCommand = new NpgsqlCommand("SELECT averagetime, score , topicid , difficulty FROM shorttermstats WHERE shorttermid = @shorttermid", connection);
            getRoundStatisticsCommand.Parameters.AddWithValue("@shorttermid", Id);
            double averageTime = 0;
            int Score = 0;
            int difficulty = 0;
            int topicId = 0;
            using (var reader = await getRoundStatisticsCommand.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    averageTime = reader.GetDouble(0);
                    Score = reader.GetInt32(1);
                    topicId = reader.GetInt32(2);
                    difficulty = reader.GetInt32(3);
                    break;
                }

            }

            //getting the topic 
            string topic = "";
            var GetTopicCommand = new NpgsqlCommand("SELECT * FROM topic WHERE topicid = @topicid", connection);
            GetTopicCommand.Parameters.AddWithValue("@topicid", topicId);
            using (var topicReader = await GetTopicCommand.ExecuteReaderAsync())
            {
                while (await topicReader.ReadAsync())
                {
                    topic = topicReader[0] as string;
                    break;
                }

            }


            var getQuestionsCommand = new NpgsqlCommand("SELECT * FROM questionbank WHERE roundid LIKE @shorttermid", connection);
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

                //creating the return object 
                CompletedRoundOfQuestioning round = new CompletedRoundOfQuestioning()
                {
                    averageTime = averageTime,
                    score = Score,
                    topic = topic,
                    difficulty = difficulty,

                    answeredQuestions = questions
                };


                return round;
            }

        }
    }
    
    
    
    //this will move all of the information form the shorttterm statistics table into the longterm stats table where the stast will be calulated and the week summarised 
    public async Task<bool> AddLongTermData(string ID)
    {
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
        
        //this is a 2D array to store each list of objects for whihc the topic which is in
        Dictionary<int, List<shorttermsstatsinfo>> topicData = new Dictionary<int, List<shorttermsstatsinfo>>();
        
        //pulling all data from the shorttermstats table 
        await using (var connection = (NpgsqlConnection)await _dbConnection.CreateDBConnection())
        {
            //the query to access the data in the shorttermsats table 
            var GetShorttermInfoCommand = new NpgsqlCommand("SELECT averagetime, score, topicid, difficulty  FROM shorttermstats WHERE shorttermid = @shorttermid", connection);

            //executing the query for each key 
            foreach (var Key in Keys)
            {
                GetShorttermInfoCommand.Parameters.Clear();
                
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
                        if (topicData.ContainsKey(topicid))
                        {
                            topicData[topicid].Add(info);
                        }
                        else
                        {
                            topicData.Add(topicid, new List<shorttermsstatsinfo>() { info });
                        }
                    }
                    
                }
            }
            
        }
        
        
        //calculating the Averages for each topic 
        int count = 0;
        Dictionary<int, TopicAverages> topicAverages = new Dictionary<int, TopicAverages>();
        foreach (int key in topicData.Keys)
        {
            TopicAverages topicAverage =  await _statsCalculation.CalculateTopicAverages(topicData[key]);
            
            topicAverages.Add(key , topicAverage);
        }
        
        //calculting the completion for each topic 
        Dictionary<int, double>completionResults = new Dictionary<int, double>();
        foreach (int key in topicAverages.Keys)
        {
            completionResults.Add(key, await _statsCalculation.CalculateTopicCompletion(topicAverages[key]));
            
        }

        double CurrentCompletion = await GetCurrentCompletion(ID);
        
        //initialising the variables that will be added to the database  
        double Newcompletion = await _statsCalculation.CalclulateOverallCompletion(completionResults);
        double completion = await _statsCalculation.CompoundCompletion(CurrentCompletion, Newcompletion);
        int worstTopicId = await _statsCalculation.GetWorstTopicID(completionResults);
        int bestTopicId = await _statsCalculation.GetBestTopicID(completionResults);
        
        
        //collecting all of the topic averages into one single average to be stored in the datbase
        double[] averges = await _statsCalculation.CalculateOverallAverageTimeAndScore(topicAverages);
        
        
        //getting the primary key for the table 
        string longTermsStatsId = await _hashing.GenerateLongTermStatsId(ID);
        
        //opening the databse connection so the entry can be added to the table 
        using (var connection = (NpgsqlConnection)await _dbConnection.CreateDBConnection())
        {
            var AddToLongTermStats = new NpgsqlCommand("INSERT INTO longtermstats(longtermstatid, averagetime, averagescore, worsttopicid, besttopicid, completion, averagedifficulty) VALUES(@key, @averagetime, @averagescore, @worsttopicid, @besttopicid, @completion, @averagedifficulty)", connection);
            AddToLongTermStats.Parameters.AddWithValue("@key", longTermsStatsId);
            AddToLongTermStats.Parameters.AddWithValue("@averagetime", averges[1]);
            AddToLongTermStats.Parameters.AddWithValue("@averagescore", averges[0]);
            AddToLongTermStats.Parameters.AddWithValue("@worsttopicid", worstTopicId);
            AddToLongTermStats.Parameters.AddWithValue("@besttopicid", bestTopicId);
            AddToLongTermStats.Parameters.AddWithValue("@completion", completion);
            AddToLongTermStats.Parameters.AddWithValue("@averagedifficulty", Convert.ToString(averges[2]));
            
            AddToLongTermStats.ExecuteNonQuery();
        }

        using (var connection = (NpgsqlConnection)await _dbConnection.CreateDBConnection())
        {
            var AddLongTermRelation = new NpgsqlCommand("INSERT INTO longtermstatsbridge(longtermstatsid, studentid) VALUES(@longtermstatsid, @studentid)", connection);
            AddLongTermRelation.Parameters.AddWithValue("@longtermstatsid", longTermsStatsId);
            AddLongTermRelation.Parameters.AddWithValue("@studentid", ID);
            
            AddLongTermRelation.ExecuteNonQuery();
        }

        using (var connection = (NpgsqlConnection)await _dbConnection.CreateDBConnection())
        {
            var DeleteShortTermRelationships = new NpgsqlCommand("DELETE FROM shorttermstatsbridge WHERE studentid = @studentid", connection);
            DeleteShortTermRelationships.Parameters.AddWithValue("@studentid", ID);
            
            DeleteShortTermRelationships.ExecuteNonQuery();
        }
        
        using (var connection = (NpgsqlConnection)await _dbConnection.CreateDBConnection())
        {
            var DeleteShortTermDataCommand = new NpgsqlCommand("DELETE FROM shorttermstats WHERE shorttermid LIKE @studentid", connection);
            DeleteShortTermDataCommand.Parameters.AddWithValue("@studentid", ID + "%");
            
            DeleteShortTermDataCommand.ExecuteNonQuery();
        }

        using (var connection = (NpgsqlConnection)await _dbConnection.CreateDBConnection())
        {
            var DeleteQuestionBankCommand = new NpgsqlCommand("Delete FROM questionbank WHERE roundid LIKE @studentid", connection);
            DeleteQuestionBankCommand.Parameters.AddWithValue("@studentid", ID + "%");
            
            DeleteQuestionBankCommand.ExecuteNonQuery();
        }
        return true;

    }

    private async Task<double> GetCurrentCompletion(string ID)
    {
        //will try to get the old completion score out the database, if there isn't one an excpetion will be thrown and so it will return 0
        try
        {
            double CurrentCompletion = 0;
            using (var connection = (NpgsqlConnection)await _dbConnection.CreateDBConnection())
            {
                var GetRelationCommand =
                    new NpgsqlCommand("SELECT longtermstatsid FROM longtermstatsbridge WHERE studentid = @studentid ORDER BY longtermstatsid DESC", connection);
                GetRelationCommand.Parameters.AddWithValue("@studentid", ID);

                string Key = null;
                using (var reader = await GetRelationCommand.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Key = reader.GetString(0);
                        break;
                    }
                }

                var GetCompletionCommand =
                    new NpgsqlCommand("SELECT completion FROM longtermstats WHERE longtermstatid = @longtermstatid",
                        connection);
                GetCompletionCommand.Parameters.AddWithValue("@longtermstatid", Key);
                using (var reader = await GetCompletionCommand.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        CurrentCompletion = reader.GetDouble(0);
                    }
                }
            }

            return CurrentCompletion;
            
        }
        catch (Exception e)
        {
            double completion = double.NaN;
            return completion;
        }
        
        
        
    }
}