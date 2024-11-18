using Typeracer.Exceptions;

namespace TyperacerUnitTests.ExceptionsUT;

public class NoAddToDBExceptionUT
{
    [Fact]
    public void DefaultConstructor_ShouldInitializeProperties()
    {
        var exception = new NoAddToDBException();
        
        Assert.False(exception.Intentional);
        Assert.Equal("Error message not specified.", exception.Message);
    }
    
    [Fact]
    public void ParameterizedConstructor_ShouldInitializeProperties()
    {
        var exception = new NoAddToDBException("test message", true);
        
        Assert.True(exception.Intentional);
        Assert.Equal("test message", exception.Message);
    }
}