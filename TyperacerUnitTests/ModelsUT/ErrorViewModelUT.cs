using Typeracer.Models;
using Xunit;

namespace TyperacerUnitTests.ModelsUT;

public class ErrorViewModelUT
{
    [Fact]
    public void RequestId_ShouldBeNull_WhenNotSet()
    {
        var model = new ErrorViewModel();
        
        var requestedId = model.RequestId;
        
        Assert.Null(requestedId);
    }

    [Fact]
    public void ShowRequestId_ShouldBeFalse_WhenRequestIdIsNull()
    {
        var model = new ErrorViewModel();

        var showRequestedId = model.ShowRequestId;
        
        Assert.False(showRequestedId);
    }
    
    [Fact]
    public void ShowRequestId_ShouldBeFalse_WhenRequestIdIsEmpty()
    {
        var model = new ErrorViewModel { RequestId = string.Empty };

        var showRequestedId = model.ShowRequestId;
        
        Assert.False(showRequestedId);
    }

    [Fact]
    public void ShowRequestId_ShouldBeTrue_WhenRequestIdIsNotEmpty()
    {
        var model = new ErrorViewModel { RequestId = "5b6a3548-b798-4b5f-b405-c520b39bd41e" };

        var showRequestedId = model.ShowRequestId;

        Assert.True(showRequestedId);
    }
}