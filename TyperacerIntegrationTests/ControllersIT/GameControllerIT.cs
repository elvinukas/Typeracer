using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Typeracer.Context;
using Typeracer.Models;

namespace TyperacerIntegrationTests.ControllersIT;

public class GameControllerIT : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly AppDbContext _context;
    
    public GameControllerIT(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        var serviceProvider = factory.Services;
        _context = serviceProvider.GetRequiredService<AppDbContext>();
    }
    
    [Fact]
    public async Task GetGameById_ReturnsNotFound_WhenGameDoesNotExist()
    {
        var gameId = Guid.NewGuid().ToString();

        var response = await _client.GetAsync($"/api/Game/{gameId}");
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task GetGameById_ReturnsGame_WhenGameExists()
    {
        var statisticsModel = new StatisticsModel();
        var game = new Game(statisticsModel);
        var id = game.GameId;
        _context.Games.Add(game);
        _context.SaveChanges();
        
        var response = await _client.GetAsync($"/api/Game/{id}");
        var returnedGame = await response.Content.ReadFromJsonAsync<Game>();
        
        response.EnsureSuccessStatusCode();
        Assert.NotNull(returnedGame);
        Assert.Equal(id, returnedGame.GameId);
    }
}