using Typeracer.Models;

namespace TyperacerUnitTests.ModelsUT;

public class ParagraphUT
{
    [Theory]
    [InlineData("This is a test paragraph.", 5)]
    [InlineData("", 1)]
    public void TotalAmountOfWords_ShouldCalculateCorrectly(string text, int expectedNumberOfWords)
    {
        var paragraph = new Paragraph(text, new List<Gamemode> { Gamemode.Standard });
        
        var totalAmountOfWords = paragraph.TotalAmountOfWords;
        
        Assert.Equal(expectedNumberOfWords, totalAmountOfWords);
    }
    
    [Theory]
    [InlineData("This is a test paragraph.", 25)]
    [InlineData("", 0)]
    public void TotalAmountOfCharacters_ShouldCalculateCorrectly(string text, int expectedNumberOfCharacters)
    {
        var paragraph = new Paragraph(text, new List<Gamemode> { Gamemode.Standard });
        
        var totalAmountOfCharacters = paragraph.TotalAmountOfCharacters;
        
        Assert.Equal(expectedNumberOfCharacters, totalAmountOfCharacters);
    }
    
    [Fact]
    public void ParameterizedConstructor_ShouldInitializeProperties()
    {
        var allowedGamemodes = new List<Gamemode> { Gamemode.Standard };
        
        var paragraph = new Paragraph("This is a test paragraph.", allowedGamemodes);
        
        Assert.NotEqual(Guid.Empty, paragraph.Id);
        Assert.Equal("This is a test paragraph.", paragraph.Text);
        Assert.Equal(allowedGamemodes, paragraph.AllowedGamemodes);
    }
}