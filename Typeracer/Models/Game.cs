namespace Typeracer.Models;

public class Game
{
    public Guid GameId { get; set; } // unique id for a game session
    public StatisticsModel Statistics { get; set; } // statistics of a game session

    public Game()
    {
        GameId = Guid.NewGuid();
        Statistics = new StatisticsModel();
    }
    
}