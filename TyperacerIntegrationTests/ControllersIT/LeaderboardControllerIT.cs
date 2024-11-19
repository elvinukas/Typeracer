using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Typeracer.Context;
using Typeracer.Models;
using Xunit.Abstractions;

namespace TyperacerIntegrationTests.ControllersIT;

public class LeaderboardControllerIT : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly HttpClient _client;
    private readonly AppDbContext _context;

    public LeaderboardControllerIT(CustomWebApplicationFactory<Program> factory, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _client = factory.CreateClient();
        var serviceProvider = factory.Services;
        _context = serviceProvider.GetRequiredService<AppDbContext>();
    }
    
    [Fact]
    public async Task GetLeaderboard_ReturnsOk_WithLeaderboardData()
    {
        var player = new Player
        {
            Username = "testuser"
        };
        _context.Players.Add(player);
        _context.SaveChanges();

        var response = await _client.GetAsync("/api/Leaderboard");

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var leaderboard = await response.Content.ReadFromJsonAsync<List<object>>();
        Assert.NotNull(leaderboard);
        Assert.NotEmpty(leaderboard);
    }
    
    [Fact]
    public async Task SavePlayerData_ReturnsBadRequest_WhenPlayerDataIsNull()
    {
        var response = await _client.PostAsJsonAsync("/api/Leaderboard/save", (PlayerDataModel)null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task SavePlayerData_ReturnsBadRequest_WhenUsernameIsMissing()
    {
        var playerData = new PlayerDataModel
        {
            BestWPM = 100,
            BestAccuracy = 95,
            GameId = Guid.NewGuid().ToString()
        };

        var response = await _client.PostAsJsonAsync("/api/Leaderboard/save", playerData);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task SavePlayerData_ReturnsOk_WhenPlayerDataIsValid()
    {
        var statisticsModel = new StatisticsModel();
        var game = new Game(statisticsModel);
        var gameId = game.GameId;
        _context.Games.Add(game);
        _context.SaveChanges();
        
        var playerData = new PlayerDataModel
        {
            Username = "testuser",
            BestWPM = 100,
            BestAccuracy = 95,
            GameId = gameId.ToString()
        };

        var response = await _client.PostAsJsonAsync("/api/Leaderboard/save", playerData);

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var rawResponse = await response.Content.ReadAsStringAsync();
        Assert.Equal("Player added to game successfully!", rawResponse);
    }
    
    [Fact]
    public async Task SavePlayerData_ReturnsBadRequest_WhenGameAlreadyHasPlayer()
    {
        var player = new Player
        {
            Username = "existinguser"
        };
        _context.Players.Add(player);
        _context.SaveChanges();

        var statisticsModel = new StatisticsModel();
        var game = new Game(statisticsModel)
        {
            PlayerId = player.PlayerID
        };
        _context.Games.Add(game);
        _context.SaveChanges();

        var playerData = new PlayerDataModel
        {
            Username = "newuser",
            BestWPM = 100,
            BestAccuracy = 95,
            GameId = game.GameId.ToString()
        };

        var response = await _client.PostAsJsonAsync("/api/Leaderboard/save", playerData);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var rawResponse = await response.Content.ReadAsStringAsync();
        Assert.Equal("Game already has a player attached!", rawResponse);
    }
}