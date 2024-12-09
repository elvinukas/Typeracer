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

            var paragraph = await _dbContext.Paragraphs
                .FirstOrDefaultAsync(p => p.Id == game.Statistics.ParagraphId);

            if (paragraph == null)
            {
                return NotFound(new { message = "Paragraph not found" });
            }

            // generate graph with paragraph which is received through the database
            await _graphService.GenerateGraphAsync(game, paragraph.TotalAmountOfWords, "red");

            return Ok(new { message = "Graph generated successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
