namespace Typeracer.Models;

public class Game
{
    public Guid GameId { get; init; } // unique id for a game session
    public StatisticsModel Statistics { get; set; } // statistics of a game session
    public double CompletionTime { get; set; }
    public Gamemode Gamemode { get; set; }

    public Dictionary<string, object> CalculativeStatistics { get; private set; } = new Dictionary<string, object>();
    
    /*
    public double WordsPerMinute { get; set; }
    public double Accuracy { get;  set; }*/
    
    public Game() {} // for deserialization
    
    public Game(StatisticsModel statisticsModel)
    {
        GameId = Guid.NewGuid();
        Statistics = statisticsModel;
        Gamemode = new Gamemode{ Mode = GamemodeOption.Standard }; // default gamemode
        CalculateCompletionTime();
        CalculateAdditionalStatistics();
        
    }

    private void CalculateCompletionTime()
    {
        this.CompletionTime = 0;
        
        if (Statistics.LocalFinishTime.HasValue && Statistics.LocalStartTime.HasValue)
        {
            this.CompletionTime = (Statistics.LocalFinishTime.Value - Statistics.LocalStartTime.Value).TotalSeconds;
        }
        
    }

    private void CalculateAdditionalStatistics()
    {
        double WordsPerMinute = 0;
        double Accuracy = 0;
        
        if (CompletionTime > 0)
        {
            WordsPerMinute = CalculateWPM(Statistics.TypedAmountOfWords, (CompletionTime / 60.0));
            Accuracy = CalculateAccuracy(Statistics.TypedAmountOfCharacters, Statistics.NumberOfWrongfulCharacters);
        }
        
        // Boxing the calculated statistics
        CalculativeStatistics["WordsPerMinute"] = WordsPerMinute;
        CalculativeStatistics["Accuracy"] = Accuracy;

        // | Calculating CurrentWordsPerMinute and CurrentAccuracy for each word
        
        // this data has to be appended to the typingData inside statistics

        double previousWPM = 0;
        List<double> wpmList = new List<double>();
        List<double> accuracyList = new List<double>();

        foreach (var typingData in Statistics.TypingData)
        {
            if (typingData.Word.Length >= 4)
            {
                double completionTime = (typingData.EndingTimestampWord - typingData.BeginningTimestampWord).TotalMinutes;
                typingData.CurrentWordsPerMinute = CalculateWPM(typedAmountOfWords: 1, completionTime: completionTime); // named arguments
                previousWPM = typingData.CurrentWordsPerMinute;
            }
            else
            {
                typingData.CurrentWordsPerMinute = previousWPM;
            }
            
            typingData.CurrentAccuracy = CalculateAccuracy(totalCharacters: typingData.Word.Length, incorrectCharacters: typingData.AmountOfMistakesInWord); // named arguments
            
            wpmList.Add(typingData.CurrentWordsPerMinute);
            accuracyList.Add(typingData.CurrentAccuracy);
        }
        
        double[] smoothedWpmData = CalculateMovingAverage(data: wpmList.ToArray()); // LINQ // named and optional arguments
        double[] smoothedAccuracyData = CalculateMovingAverage(data: accuracyList.ToArray()); // LINQ // named and optional arguments

        for (int i = 0; i < Statistics.TypingData.Count; i++)
        {
            Statistics.TypingData[i].CurrentWordsPerMinute = smoothedWpmData[i];
            Statistics.TypingData[i].CurrentAccuracy = smoothedAccuracyData[i];
        }

    }
    
    private double[] CalculateMovingAverage(double[] data, int windowSize = 7)
    {
        double[] result = new double[data.Length];
        double sum = 0;
        int count = 0;
        for (int i = 0; i < data.Length; i++)
        {
            sum += data[i];
            ++count;

            if (count > windowSize)
            {
                sum -= data[i - windowSize];
                --count;
            }
            
            result[i] = sum / count;
        }
        return result;
    }
    
    
    private double CalculateWPM(int typedAmountOfWords, double completionTime)
    {
        if (completionTime != 0)
        {
            return typedAmountOfWords / completionTime;
        }
        else
        {
            return -1;
        }
    }
    
    private double CalculateAccuracy(int totalCharacters, int incorrectCharacters)
    {
        int correctCharacters = totalCharacters - incorrectCharacters;
        if (totalCharacters != 0 && correctCharacters > 0)
        {
            return (double)correctCharacters / totalCharacters * 100;
        }
        else
        {
            return 0;
        }
    }
    
    
    
}