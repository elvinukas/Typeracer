using Typeracer.Models;
using System.Threading.Tasks;

public interface IGraphService
{
    Task GenerateGraphAsync(Game game, string WPMColor = "blue");
}