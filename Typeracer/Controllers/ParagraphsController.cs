using Microsoft.AspNetCore.Mvc;
using Typeracer.Context;
using Typeracer.Models;

namespace Typeracer.Controllers;


[ApiController]
[Route("api/[controller]")]
public class ParagraphsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ParagraphsController(AppDbContext context)
    {
        _context = context;
    }
    
    [HttpGet("{paragraphId}")]
    public IActionResult GetParagraph(string paragraphId)
    {
        Paragraph? paragraph = _context.Paragraphs
            .FirstOrDefault(p => p.Id == Guid.Parse(paragraphId));

        if (paragraph == null)
        {
            return NotFound(new { message = "Paragraph not found." });
        }

        return Ok(paragraph);
    }
    
}