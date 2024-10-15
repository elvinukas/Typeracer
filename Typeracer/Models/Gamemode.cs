namespace Typeracer.Models;

public enum GamemodeOption
{
    Standard,
    Short, // shorter paragraph
    Hardcore // no mistakes allowed
}

public class Gamemode : IEquatable<Gamemode>
{
    public GamemodeOption Mode { get; set; }
    
    public bool Equals(Gamemode other)
    {
        if (other == null)
        {
            return false;
        }
        return this.Mode == other.Mode;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        
        return Equals(obj as Gamemode);
    }
    
}