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
}