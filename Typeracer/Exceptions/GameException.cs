namespace Typeracer.Exceptions;

public class GameException : Exception
{
    public string Message { get; }

    public GameException()
    { 
        Message = "Error message not specified.";
    }

    public GameException(string message) : base(message)
    {
        Message = message;
    }
}