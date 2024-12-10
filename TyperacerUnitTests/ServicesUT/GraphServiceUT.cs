using Typeracer.Models;
using Typeracer.Services;

namespace TyperacerUnitTests.ServicesUT;

public class GraphServiceUT
{
    [Fact]
    public async Task GenerateGraphAsync_ShouldCreateGraphImage()
    {
        string directoryPath = Path.Combine(AppContext.BaseDirectory, "wwwroot/images");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        
        var statisticsModel = new StatisticsModel
        {
            TypingData = new List<TypingData>
            {
                new TypingData { Word = "testas1", CurrentWordsPerMinute = 10, CurrentAccuracy = 95 },
                new TypingData { Word = "testas1.1", CurrentWordsPerMinute = 20, CurrentAccuracy = 87 },
            }
        };
        
        var game = new Game(statisticsModel);
        var graphService = new GraphService();
        
        await graphService.GenerateGraphAsync(game, 2, WPMColor: "green");
        
        Assert.True(File.Exists("wwwroot/images/wpm-graph.png"));
    }
}