namespace Typeracer.Models;

// values of a player can change, so struct is used instead of record
// extension methods can be called to calculate the average WPM, average accuracy, best WPM and performance score
public class Player : IComparable<Player>
{
    public Guid PlayerID { get; private set; }  // changed from readonly for deserialization (private set for unreachable modification)
    public string Username { get; set; }
    public List<double> WPMs { get; set; } // each game average WPM should be added here
    public List<double> Accuracies { get; set; } // each game average accuracy should be added here
    public double BestWPM { get; set; }
    
    public double BestAccuracy { get; set; }
    
    // parameterless constructor for deserialization
    public Player()
    {
        WPMs = new List<double>();
        Accuracies = new List<double>();
    }


    public Player(string username, Guid playerID, double initialWPM, double initialAccuracy)
    {
        PlayerID = playerID;
        Username = username;
        WPMs = new List<double>();
        Accuracies = new List<double>();
        BestWPM = 0;
        BestAccuracy = 0;
        AddGameResult(initialWPM, initialAccuracy);
        Console.WriteLine($"Player created: {PlayerID} - {username}");
    }
    
    public void AddGameResult(double wpm, double accuracy)
    {
        WPMs.Add(wpm);
        Accuracies.Add(accuracy);
        
        if (wpm > BestWPM)
        {
            BestWPM = wpm;
        }

        if (accuracy > BestAccuracy)
        {
            BestAccuracy = accuracy;
        }
    }

    // comparing players by their best WPM
    public int CompareTo(Player otherPlayer)
    {
        return BestWPM.CompareTo(otherPlayer.BestWPM);
    }
}