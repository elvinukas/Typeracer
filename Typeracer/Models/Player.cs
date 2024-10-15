namespace Typeracer.Models;

// values of a player can change, so struct is used instead of record
// extension methods can be called to calculate the average WPM, average accuracy, best WPM and performance score
public struct Player : IComparable<Player>
{
    public readonly Guid PlayerID { get; }
    public string Username { get; set; }
    public List<double> WPMs { get; set; } // each game average WPM should be added here
    public List<double> Accuracies { get; set; } // each game average accuracy should be added here
    public double BestWPM { get; set; }

    // comparing players by their best WPM
    public int CompareTo(Player otherPlayer)
    {
        return BestWPM.CompareTo(otherPlayer.BestWPM);
    }
}