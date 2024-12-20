using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Typeracer.Context;
using ControllerBase = Microsoft.AspNetCore.Mvc.ControllerBase;

[ApiController]
[Route("api/[controller]")]
public class GraphController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IGraphService _graphService;

    public GraphController(AppDbContext dbContext, IGraphService graphService)
    {
        _dbContext = dbContext;
        _graphService = graphService;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateGraph([FromBody] string gameId)
    {
        try
        {
            var game = await _dbContext.Games
                .Include(g => g.Statistics)
                .ThenInclude(s => s.TypingData.OrderBy(td => td.BeginningTimestampWord))
                .FirstOrDefaultAsync(g => g.GameId == Guid.Parse(gameId));
                

            if (game == null)
            {
                return NotFound(new { message = "Game not found" });
            }
            
            // checking if there is any typing data for this game and if list is not empty
            if (game?.Statistics?.TypingData == null || !game.Statistics.TypingData.Any())
            {
                return NotFound(new { message = "No typing data found for this game." });
            }
            
            // generate graph with paragraph which is received through the database
            // generating graph based on not the total amount of words in the paragraph, but on total amount of words written!
            // that is why paragraph is not needed to be retrieved to generated the graph
            await _graphService.GenerateGraphAsync(game, game.Statistics.TypingData.Count, "red");

            return Ok(new { message = "Graph generated successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
