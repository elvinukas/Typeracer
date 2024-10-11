namespace Typeracer.Models;

public record Paragraph
{
    public string Text { get; init; }
    public int TotalAmountOfWords { get; init; }
    public int TotalAmountOfCharacters { get; init; }
    public List<Gamemode> AllowedGamemodes { get; set; } // in which gamemodes is this paragraph allowed in

    public Paragraph(string text, List<Gamemode> allowedGamemodes)
    {
        Text = text;
        TotalAmountOfWords = text.Split(' ').Length;
        TotalAmountOfCharacters = text.Length;
        AllowedGamemodes = allowedGamemodes;
    }
    
}