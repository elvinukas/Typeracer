using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Typeracer.Context;
using Typeracer.Models;

namespace TyperacerIntegrationTests.ControllersIT;

public class StatisticsControllerIT : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly AppDbContext _context;

    public StatisticsControllerIT(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        var serviceProvider = factory.Services;
        _context = serviceProvider.GetRequiredService<AppDbContext>();
    }
    
    [Fact]
    public async Task GetStatistics_ReturnsNotFound_WhenStatisticsDoesNotExist()
    {
        var statisticsId = Guid.NewGuid().ToString();

        var response = await _client.GetAsync($"/api/Statistics/{statisticsId}");
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task GetStatistics_ReturnsStatistics_WhenStatisticsExists()
    {
        var id = Guid.NewGuid();
        var statistics = new StatisticsModel();
        statistics.StatisticsId = id;
        _context.Statistics.Add(statistics);
        _context.SaveChanges();
        
        var response = await _client.GetAsync($"/api/Statistics/{id}");
        var returnedStatistics = await response.Content.ReadFromJsonAsync<StatisticsModel>();
        
        response.EnsureSuccessStatusCode();
        Assert.NotNull(returnedStatistics);
        Assert.Equal(id, returnedStatistics.StatisticsId);
    }
    
    [Fact]
    public async Task Save_ReturnsBadRequest_WhenStatisticsDataIsNull()
    {
        var response = await _client.PostAsJsonAsync("/api/Statistics/save", (StatisticsModel)null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task Save_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        var invalidStatistics = new StatisticsModel
        {
            TypingData = new List<TypingData>
            {
                new TypingData
                {
                    BeginningTimestampWord = DateTime.UtcNow
                }
            }
        };
        
        var response = await _client.PostAsJsonAsync("/api/Statistics/save", invalidStatistics);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task Save_ReturnsOk_WhenStatisticsDataIsValid()
    {
        var validStatistics = new StatisticsModel
        {
            LocalStartTime = DateTime.UtcNow,
            LocalFinishTime = DateTime.UtcNow.AddSeconds(5),
            ParagraphId = Guid.NewGuid(),
            TypedAmountOfWords = 10,
            TypedAmountOfCharacters = 50,
            NumberOfWrongfulCharacters = 2,
            TypingData = new List<TypingData>
            {
                new TypingData
                {
                    Word = "This",
                    BeginningTimestampWord = DateTime.UtcNow,
                    EndingTimestampWord = DateTime.UtcNow.AddSeconds(1),
                    AmountOfMistakesInWord = 0,
                    CurrentWordsPerMinute = 60,
                    CurrentAccuracy = 98
                }
            }
        };

        var response = await _client.PostAsJsonAsync("/api/Statistics/save", validStatistics);
        var successResponse = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Statistics received and game information saved to database", successResponse["message"].ToString());
        Assert.NotNull(successResponse["gameId"]);
    }
}