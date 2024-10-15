namespace Typeracer.Models;

// values of a player can change, so struct is used instead of record
public struct Player : IComparable<Player>
{
    public readonly Guid PlayerID { get; }
    public string Username { get; set; }
    public double AverageWPM { get; set; }
    public double AverageAccuracy { get; set; }
    public double BestWPM { get; set; }

    // comparing players by their best WPM
    public int CompareTo(Player otherPlayer)
    {
        return BestWPM.CompareTo(otherPlayer.BestWPM);
    }
}