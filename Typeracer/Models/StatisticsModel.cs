namespace Typeracer.Models;

public class StatisticsModel
{
    public double CompletionTime { get; set; } // time that it took to finish the entire paragraph (or finish typing)(milliseconds)
    public DateTime? LocalStartTime { get; set; } // start date and time of the game
    public DateTime? LocalFinishTime { get; set; } // end date and time of the game
    
    public int TotalAmountOfWords { get; set; } // amount of words in the paragraph
    public int TotalAmountOfCharacters { get; set; } // amount of characters in the paragraph;
    public int TypedAmountOfWords { get; set; }
    public int TypedAmountOfCharacters { get; set; }
    public int NumberOfWrongfulCharacters { get; set; }
    public List<TypingData> TypingData { get; set; } = new List<TypingData>(); // storing all data about the typed characters
    
    // --------------------------------------
    // FIELDS BELOW ARE TO BE CALCULATED AND NOT RECEIVED
    // --------------------------------------
    
    public double WordsPerMinute { get; set; }
    public double Accuracy { get; set; }
    
}

public class TypingData
{
    public string Word { get; set; }
    public DateTime BeginningTimestampWord { get; set; } // timestamp of when a word was started to be typed
    public DateTime EndingTimestampWord { get; set; } // timestamp of when a word was completely typed correctly
    public int AmountOfMistakesInWord { get; set;} // amount of mistakes that were made while writing the word
    //public bool isCorrect { get; set; }
    
    // data to be calculated
    public double CurrentWordsPerMinute { get; set; } // current wpm
    public double CurrentAccuracy { get; set; }
}