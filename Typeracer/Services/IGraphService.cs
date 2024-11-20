using Typeracer.Models;

public interface IGraphService
{
    Task GenerateGraphAsync(Game game, int totalWords, string WPMColor = "blue");
}