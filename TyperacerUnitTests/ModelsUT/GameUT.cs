using System.Reflection;
using Typeracer.Models;

namespace TyperacerUnitTests.ModelsUT;

public class GameUT
{
    [Fact]
    public void DefaultConstructor_ShouldInitializeProperties()
    {
        var game = new Game();
        
        Assert.Equal(Guid.Empty, game.GameId);
        Assert.Null(game.PlayerId);
        Assert.Null(game.Player);
        Assert.Equal(Guid.Empty, game.StatisticsId);
        Assert.Null(game.Statistics);
        Assert.Equal(0, game.CompletionTime);
        Assert.Equal(Gamemode.Standard, game.Gamemode);
    }

    [Fact]
    public void ParameterizedConstructor_ShouldInitializeProperties()
    {
        var statisticsModel = new StatisticsModel();

        var game = new Game(statisticsModel);
        
        Assert.NotEqual(Guid.Empty, game.GameId);
        Assert.Null(game.PlayerId);
        Assert.Null(game.Player);
        Assert.Equal(Guid.Empty, game.StatisticsId);
        Assert.Equal(statisticsModel, game.Statistics);
        Assert.Equal(0, game.CompletionTime);
        Assert.Equal(Gamemode.Standard, game.Gamemode);
    }

    [Theory]
    [InlineData(null, "2024-11-08T10:00:00Z", 0)]
    [InlineData("2024-11-08T10:00:00Z", null, 0)]
    [InlineData(null, null, 0)]
    [InlineData("2024-11-08T10:00:00Z", "2024-11-08T10:00:05Z", 5)]
    [InlineData("2024-11-08T10:00:05Z", "2024-11-08T10:00:00Z", 0)]
    public void CalculateCompletionTime(string startTimeString, string finishTimeString, double expectedCompletionTime)
    {
        DateTime? startTime = startTimeString != null ? DateTime.Parse(startTimeString) : (DateTime?)null;
        DateTime? finishTime = finishTimeString != null ? DateTime.Parse(finishTimeString) : (DateTime?)null;
        
        var statisticsModel = new StatisticsModel
        {
            LocalStartTime = startTime,
            LocalFinishTime = finishTime
        };
        
        var game = new Game(statisticsModel);
        
        Assert.Equal(expectedCompletionTime, game.CompletionTime);
    }

    [Fact]
    public void CalculateAdditionalStatistics_ShouldSetWordsPerMinuteAndAccuracy_WhenCompletionTimeIsPositive()
    {
        DateTime startTime = DateTime.UtcNow;
        DateTime finishTime = startTime.AddSeconds(60);
        var statisticsModel = new StatisticsModel
        {
            LocalStartTime = startTime,
            LocalFinishTime = finishTime,
            TypedAmountOfWords = 10,
            TypedAmountOfCharacters = 50,
            NumberOfWrongfulCharacters = 5
        };

        var game = new Game(statisticsModel);
        
        Assert.Equal(10, game.Statistics.WordsPerMinute);
        Assert.Equal(90, game.Statistics.Accuracy);
    }
    
    [Fact]
    public void CalculateAdditionalStatistics_ShouldNotSetWordsPerMinuteAndAccuracy_WhenCompletionTimeIsZero()
    {
        DateTime startTime = DateTime.UtcNow;
        DateTime finishTime = startTime;
        var statisticsModel = new StatisticsModel
        {
            LocalStartTime = startTime,
            LocalFinishTime = finishTime,
            TypedAmountOfWords = 10,
            TypedAmountOfCharacters = 50,
            NumberOfWrongfulCharacters = 5
        };
        
        var game = new Game(statisticsModel);
        
        Assert.Equal(0, game.Statistics.WordsPerMinute);
        Assert.Equal(0, game.Statistics.Accuracy);
    }
    
    [Fact]
    public void CalculateAdditionalStatistics_ShouldCalculateCurrentWordsPerMinuteAndAccuracy_ForEachWord()
    {
        DateTime startTime1 = DateTime.UtcNow;
        DateTime finishTime1 = startTime1.AddSeconds(30);
        DateTime startTime2 = finishTime1;
        DateTime finishTime2 = startTime2.AddSeconds(60);
        var typingData = new List<TypingData>
        {
            new TypingData { Word = "test", BeginningTimestampWord = startTime1, EndingTimestampWord = finishTime1, AmountOfMistakesInWord = 1 },
            new TypingData { Word = "example", BeginningTimestampWord = startTime2, EndingTimestampWord = finishTime2, AmountOfMistakesInWord = 0 }
        };
        var statisticsModel = new StatisticsModel
        {
            LocalStartTime = startTime1,
            LocalFinishTime = finishTime2,
            TypingData = typingData
        };
        var game = new Game(statisticsModel);

        Assert.Equal(2, game.Statistics.TypingData[0].CurrentWordsPerMinute);
        Assert.Equal(75, game.Statistics.TypingData[0].CurrentAccuracy);
        Assert.Equal(1.5, game.Statistics.TypingData[1].CurrentWordsPerMinute);
        Assert.Equal(87.5, game.Statistics.TypingData[1].CurrentAccuracy);
    }
    
    [Theory]
    [InlineData(new double[] { 1, 2, 3, 4, 5 }, 3, new double[] { 1, 1.5, 2, 3, 4 })]
    [InlineData(new double[] { 10, 20, 30, 40, 50 }, 2, new double[] { 10, 15, 25, 35, 45 })]
    [InlineData(new double[] { 5, 5, 5, 5, 5 }, 1, new double[] { 5, 5, 5, 5, 5 })]
    [InlineData(new double[] { 1, 2, 3, 4, 5 }, 5, new double[] { 1, 1.5, 2, 2.5, 3 })]
    [InlineData(new double[] { }, 3, new double[] { })]
    public void CalculateMovingAverage_ShouldCalculateCorrectly(double[] data, int windowSize, double[] expected)
    {
        var game = new Game();
        var method = typeof(Game).GetMethod("CalculateMovingAverage", BindingFlags.NonPublic | BindingFlags.Instance);
        var result = (double[])method.Invoke(game, new object[] { data, windowSize });

        Assert.Equal(expected, result);
    }
    
    [Theory]
    [InlineData(90, 3, 30)]
    [InlineData(120, 0, 0)]
    [InlineData(20, 0.5, 40)]
    [InlineData(0, 1, 0)]
    public void CalculateWPM_ShouldCalculateCorrectly(int typedAmountOfWords, double completionTime, double expectedWPM)
    {
        var game = new Game();
        var method = typeof(Game).GetMethod("CalculateWPM", BindingFlags.NonPublic | BindingFlags.Instance);
        var result = (double)method.Invoke(game, new object[] { typedAmountOfWords, completionTime });

        Assert.Equal(expectedWPM, result);
    }
    
    [Theory]
    [InlineData(100, 10, 90)]
    [InlineData(100, 0, 100)]
    [InlineData(100, 100, 0)]
    [InlineData(0, 0, 0)]
    public void CalculateAccuracy_ShouldCalculateCorrectly(int totalCharacters, int incorrectCharacters, double expectedAccuracy)
    {
        var game = new Game();
        var method = typeof(Game).GetMethod("CalculateAccuracy", BindingFlags.NonPublic | BindingFlags.Instance);
        var result = (double)method.Invoke(game, new object[] { totalCharacters, incorrectCharacters });

        Assert.Equal(expectedAccuracy, result);
    }
}