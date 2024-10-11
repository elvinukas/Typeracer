namespace Typeracer.Models;

public class Game
{
    public Guid GameId { get; init; } // unique id for a game session
    public StatisticsModel Statistics { get; set; } // statistics of a game session
    public double CompletionTime { get; set; }
    public Gamemode Gamemode { get; set; }
    
    public double WordsPerMinute { get; set; }
    public double Accuracy { get;  set; }
    
    public Game() {} // for deserialization
    
    public Game(StatisticsModel statisticsModel)
    {
        GameId = Guid.NewGuid();
        Statistics = statisticsModel;
        Gamemode = Gamemode.Standard; // default gamemode
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
        if (CompletionTime > 0)
        {
            WordsPerMinute = CalculateWPM(Statistics.TypedAmountOfWords, (CompletionTime / 60.0));
            Accuracy = CalculateAccuracy(Statistics.TypedAmountOfCharacters, Statistics.NumberOfWrongfulCharacters);
        }
        else
        {
            WordsPerMinute = 0;
            Accuracy = 0;
        }
        
        // | Calculating CurrentWordsPerMinute and CurrentAccuracy for each word
        
        // this data has to be appended to the typingData inside statistics

        int countedWordsSoFar = 0;
        double timeTakenSoFar = 0;

        foreach (var typingData in Statistics.TypingData)
        {
            TimeSpan timeTakenForWord = typingData.EndingTimestampWord - typingData.BeginningTimestampWord;
            double timeTakenInMinutes = timeTakenForWord.TotalMinutes;
            ++countedWordsSoFar;
            timeTakenSoFar += timeTakenInMinutes;
            
            if (timeTakenInMinutes > 0)
            {
                typingData.CurrentWordsPerMinute = CalculateWPM(countedWordsSoFar, timeTakenSoFar);
            }
            
            else
            {
                // if a 1 letter word is the first word, its wpm is 0.
                if (countedWordsSoFar == 1 && timeTakenInMinutes == 0)
                {
                    typingData.CurrentWordsPerMinute = 0;
                }
                else
                {
                    // returning the previous wpm, since the results do not need to be disturbed by 1 letter word
                    typingData.CurrentWordsPerMinute =
                        CalculateWPM(countedWordsSoFar - 1, timeTakenSoFar - timeTakenInMinutes);
                }

            }
        }
        
        int typedCharsSoFar = 0;
        int amountOfMistakesSoFar = 0;

        foreach (var typingData in Statistics.TypingData)
        {
            typedCharsSoFar += typingData.Word.Length;
            amountOfMistakesSoFar += typingData.AmountOfMistakesInWord;

            typingData.CurrentAccuracy = CalculateAccuracy(typedCharsSoFar, amountOfMistakesSoFar);
        }
        
        

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
        if (totalCharacters != 0)
        {
            return (double)correctCharacters / totalCharacters * 100;
        }
        else
        {
            return 0;
        }
    }
    
    
    
}