using Typeracer.Models;
using Xunit.Abstractions;

namespace TyperacerUnitTests.ModelsUT;

public class PlayerUT
{
    private readonly ITestOutputHelper _testOutputHelper;

    public PlayerUT(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void DefaultConstructor_ShouldInitializeProperties()
    {
        var player = new Player();
        
        Assert.Equal(Guid.Empty, player.PlayerID);
        Assert.Null(player.Username);
        Assert.Empty(player.WPMs);
        Assert.Empty(player.Accuracies);
        Assert.Null(player.BestWPMID);
        Assert.Null(player.BestAccuracyID);
    }
    
    [Fact]
    public void ParameterizedConstructor_ShouldInitializeProperties()
    {
        var player = new Player("test_user", 100, 100);
        
        Assert.NotEqual(Guid.Empty, player.PlayerID);
        Assert.Equal("test_user", player.Username);
        Assert.NotEmpty(player.WPMs);
        Assert.NotEmpty(player.Accuracies);
        Assert.NotNull(player.BestWPMID);
        Assert.NotNull(player.BestAccuracyID);
    }
    
    [Fact]
    public void AddGameResult_ShouldUpdateBestWPMID_WhenNewWPMIsHigher()
    {
        var player = new Player("test_user", 50, 90);
        var initialBestWPMID = player.BestWPMID;

        player.AddGameResult(60, 85);

        Assert.NotEqual(initialBestWPMID, player.BestWPMID);
        Assert.Equal(60, player.WPMs.Find(w => w.WPMId == player.BestWPMID)?.Value);
    }
    
    [Fact]
    public void AddGameResult_ShouldNotUpdateBestWPMID_WhenNewWPMIsLower()
    {
        var player = new Player("test_user", 70, 90);
        var initialBestWPMID = player.BestWPMID;

        player.AddGameResult(60, 85);

        Assert.Equal(initialBestWPMID, player.BestWPMID);
        Assert.Equal(70, player.WPMs.Find(w => w.WPMId == player.BestWPMID)?.Value);
    }
    
    [Theory]
    [InlineData(50, 97, 40, 98, 1)]
    [InlineData(50, 97, 60, 98, -1)]
    [InlineData(50, 97, 50, 97, 0)]
    public void CompareTo_ShouldCalculateCorrectly(double FirstWPM, double FirstAccuracy, double SecondWPM, double SecondAccuracy, int expectedResult)
    {
        var player1 = new Player("player1", FirstWPM, FirstAccuracy);
        var player2 = new Player("player2", SecondWPM, SecondAccuracy);

        var result = player1.CompareTo(player2);
        Assert.Equal(result, expectedResult);
    }
}

public class WPMUT
{
    [Theory]
    [InlineData(85.2, 78.9, 1)]
    [InlineData(78.9, 85.2, -1)]
    [InlineData(85.2, 85.2, 0)]
    public void CompareTo_ShouldCalculateCorrectly(double FirstWPM, double SecondWPM, int expectedResult)
    {
        var wpm1 = new WPM { Value = FirstWPM };
        var wpm2 = new WPM { Value = SecondWPM };

        var result = wpm1.CompareTo(wpm2);
        Assert.Equal(result, expectedResult);
    }
}

public class AccuracyUT
{
    [Theory]
    [InlineData(97, 80, 1)]
    [InlineData(89, 89.9, -1)]
    [InlineData(100, 100, 0)]
    public void CompareTo_ShouldCalculateCorrectly(double FirstAccuracy, double SecondAccuracy, int expectedResult)
    {
        var Accuracy1 = new Accuracy { Value = FirstAccuracy };
        var Accuracy2 = new Accuracy { Value = SecondAccuracy };

        var result = Accuracy1.CompareTo(Accuracy2);
        Assert.Equal(result, expectedResult);
    }
}