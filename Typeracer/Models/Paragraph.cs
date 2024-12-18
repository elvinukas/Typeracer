using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Typeracer.Models;

[Index(nameof(Text), IsUnique = true)]
public record Paragraph
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public string Text { get; init; }

    [NotMapped]
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