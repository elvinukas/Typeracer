using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mime;

namespace Typeracer.Models;

public record Paragraph
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public string Text { get; init; }

    [NotMapped] // these two properties should be calculated each time and not stored in the database
    public int TotalAmountOfWords => Text.Split(' ').Length;

    [NotMapped] public int TotalAmountOfCharacters => Text.Length;
    
    
    public List<Gamemode> AllowedGamemodes { get; set; } // in which gamemodes is this paragraph allowed in

    public Paragraph(string text, List<Gamemode> allowedGamemodes)
    {
        Id = Guid.NewGuid();
        Text = text;
        AllowedGamemodes = allowedGamemodes;
    }
    
}