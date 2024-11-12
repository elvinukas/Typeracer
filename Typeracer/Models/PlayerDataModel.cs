namespace Typeracer.Models;

public class PlayerDataModel {
    public string Username { get; set; }
    public double BestWPM { get; set; }
    public double BestAccuracy { get; set; }
    public string GameId { get; set; } // this is required currently for storing the statistics for a user
}