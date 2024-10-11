namespace Typeracer.Models;

// values of a player can change, so struct is used instead of record
public struct Player
{
    public readonly Guid PlayerID { get; }
    public string Username { get; set; }
    public double AverageWPM { get; set; }
    public double AverageAccuracy { get; set; }
    public double BestWPM { get; set; }
    
    
}