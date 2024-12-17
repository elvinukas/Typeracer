namespace Typeracer.Exceptions;

public class NoAddToDBException : Exception
{
    public bool Intentional { get; }
    public string Message { get; }

    public NoAddToDBException()
    { 
        Intentional = false;
        Message = "Error message not specified.";
    }

    public NoAddToDBException(string message, bool intentional = false) : base(message)
    {
        Intentional = intentional;
        Message = message;
    }
}