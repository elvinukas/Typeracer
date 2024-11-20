using Typeracer.Exceptions;

namespace TyperacerUnitTests.ExceptionsUT;

public class GameExceptionUT
{

    [Fact]
    public void DefaultContructor_ShouldInitialiseProperties()
    {
        var exception = new GameException();
        
        Assert.True(exception.Message == "Error message not specified.");
    }
    
    [Fact]
    public void ParameterizedConstructor_ShouldInitializeProperties()
    {
        var exception = new GameException("test message");
        
        Assert.Equal("test message", exception.Message);
    }
}