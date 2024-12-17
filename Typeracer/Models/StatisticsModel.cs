using System.ComponentModel.DataAnnotations;

namespace Typeracer.Models;

public record StatisticsModel
{
    public Guid StatisticsId { get; set; } = Guid.NewGuid();
    public DateTime? LocalStartTime { get; set; } // start date and time of the game
    public DateTime? LocalFinishTime { get; set; } // end date and time of the game
    
    public Guid ParagraphId { get; set; }
    public int TypedAmountOfWords { get; set; }
    public int TypedAmountOfCharacters { get; set; }
    public int NumberOfWrongfulCharacters { get; set; }
    
    public double WordsPerMinute { get; set; }
    public double Accuracy { get; set; }
    public Gamemode Gamemode { get; set; }
    public List<TypingData> TypingData { get; set; } = new List<TypingData>(); // storing all data about the typed characters
}

public record TypingData
{
    [Key]
    public Guid TypingDataId { get; set; } = Guid.NewGuid();
    
    public string Word { get; set; }
    public DateTime? BeginningTimestampWord { get; set; } = DateTime.UtcNow;// timestamp of when a word was started to be typed
    public DateTime? EndingTimestampWord { get; set; } = DateTime.UtcNow; // timestamp of when a word was completely typed correctly
    public int AmountOfMistakesInWord { get; set;} // amount of mistakes that were made while writing the word
    
    // data to be calculated
    public double CurrentWordsPerMinute { get; set; } // current wpm
    public double CurrentAccuracy { get; set; }
}