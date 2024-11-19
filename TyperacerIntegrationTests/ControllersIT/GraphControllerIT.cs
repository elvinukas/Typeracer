using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Typeracer.Context;
using Typeracer.Models;

namespace TyperacerIntegrationTests.ControllersIT;

public class GraphControllerIT : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly AppDbContext _context;

    public GraphControllerIT(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        var serviceProvider = factory.Services;
        _context = serviceProvider.GetRequiredService<AppDbContext>();
    }
    
    [Fact]
    public async Task GenerateGraph_ReturnsNotFound_WhenGameDoesNotExist()
    {
        var gameId = Guid.NewGuid().ToString();

        var response = await _client.PostAsJsonAsync("/api/Graph/generate", gameId);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task GenerateGraph_ReturnsNotFound_WhenParagraphDoesNotExist()
    {
        var statisticsModel = new StatisticsModel();
        var game = new Game(statisticsModel);
        _context.Games.Add(game);
        _context.SaveChanges();

        var response = await _client.PostAsJsonAsync("/api/Graph/generate", game.GameId.ToString());

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    /*
    [Fact]
    public async Task GenerateGraph_ReturnsOk_WhenGraphIsGeneratedSuccessfully()
    {
        var paraId = Guid.NewGuid();
        var paragraph = new Paragraph("This paragraph", new List<Gamemode>());
        paragraph.Id = paraId;
        _context.Paragraphs.Add(paragraph);
        _context.SaveChanges();

        var startTime1 = DateTime.UtcNow;
        var finishTime1 = startTime1.AddSeconds(5);
        var startTime2 = finishTime1;
        var finishTime2 = startTime2.AddSeconds(5);
        
        var statisticsModel = new StatisticsModel
        {
            LocalStartTime = startTime1,
            LocalFinishTime = finishTime2,
            TypedAmountOfWords = 2,
            TypedAmountOfCharacters = 15,
            NumberOfWrongfulCharacters = 0,
            TypingData = new List<TypingData>
            {
                new TypingData
                {
                    Word = "This",
                    BeginningTimestampWord = startTime1,
                    EndingTimestampWord = finishTime1,
                    AmountOfMistakesInWord = 0
                },
                new TypingData
                {
                    Word = "paragraph",
                    BeginningTimestampWord = startTime2,
                    EndingTimestampWord = finishTime2,
                    AmountOfMistakesInWord = 0
                }
            }
        };
        statisticsModel.ParagraphId = paraId;
        var game = new Game(statisticsModel);
        _context.Games.Add(game);
        _context.SaveChanges();

        var response = await _client.PostAsJsonAsync("/api/Graph/generate", game.GameId.ToString());

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    */
}