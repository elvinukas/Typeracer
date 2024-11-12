namespace Typeracer.Models;

public class PlayerDataModel
{
    public string PlayerID { get; set; }
    public string Username { get; set; }
    public double BestWPM { get; set; }
    public double BestAccuracy { get; set; }
    public Guid GameId { get; set; } // this is required currently for storing the statistics for a user
}