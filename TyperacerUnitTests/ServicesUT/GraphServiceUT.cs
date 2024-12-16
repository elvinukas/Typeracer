using Microsoft.AspNetCore.Hosting;
using Typeracer.Models;
using Typeracer.Services;
using Moq;

namespace TyperacerUnitTests.ServicesUT;

public class GraphServiceUT
{
    [Fact]
    public async Task GenerateGraphAsync_ShouldCreateGraphImage()
    {
        string webRootPath = Path.Combine(Path.GetTempPath(), "test-wwwroot");
        string directoryPath = Path.Combine(webRootPath, "images");
        
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        
        var mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
        mockWebHostEnvironment.Setup(env => env.WebRootPath).Returns(webRootPath);
        
        var statisticsModel = new StatisticsModel
        {
            TypingData = new List<TypingData>
            {
                new TypingData { Word = "testas1", CurrentWordsPerMinute = 10, CurrentAccuracy = 95 },
                new TypingData { Word = "testas1.1", CurrentWordsPerMinute = 20, CurrentAccuracy = 87 },
            }
        };
        
        var game = new Game(statisticsModel);
        var graphService = new GraphService(mockWebHostEnvironment.Object);
        
        await graphService.GenerateGraphAsync(game, 2, WPMColor: "green");
        
        string expectedFilePath = Path.Combine(directoryPath, "wpm-graph.png");
        
        Assert.True(File.Exists(expectedFilePath));
        
        Directory.Delete(webRootPath, true);
    }
}