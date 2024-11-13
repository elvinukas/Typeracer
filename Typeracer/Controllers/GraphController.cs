using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using OxyPlot.SkiaSharp;
using Typeracer.Context;
using Typeracer.Models;
using ControllerBase = Microsoft.AspNetCore.Mvc.ControllerBase;
using Typeracer.Serivces;

[ApiController]
[Route("api/[controller]")]
public class GraphController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IGraphService _graphService;
    
    public GraphController(AppDbContext context, IGraphService graphService)
    {
        _context = context;
        _graphService = graphService;
    }
    
    
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateGraph([FromBody] string gameId)
    {
        try
        {
            Game? game = await _context.Games
                .Include(g => g.Statistics)
                    .ThenInclude(s => s.TypingData)
                .Include(g => g.Statistics)
                    .ThenInclude(s => s.Paragraph)
                .FirstOrDefaultAsync(g => g.GameId == Guid.Parse(gameId));
            
            if (game == null)
            {
                return NotFound(new { message = "Game not found" });
            }
            
            
            
            Console.WriteLine("Graph controller received gameId: ", gameId);
            await _graphService.GenerateGraphAsync(game, "red");
            return Ok(new { message = "Graph generated successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}