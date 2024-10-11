using System.Diagnostics;
using System.Net.Mime;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;
using Typeracer.Models;

namespace Typeracer.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Type()
    {
        return View();
    }

    public List<Paragraph> GetAllParagraphs()
    {
        string[] paragraphs;
        // getting the file path
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Paragraphs", "paragraph1_short.txt");
        Console.Write(filePath);
        
        // splitting the text into paragraphs
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            paragraphs = System.IO.File.ReadAllText(filePath).Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
        }
        else
        {
            paragraphs = System.IO.File.ReadAllText(filePath).Split("\n", StringSplitOptions.RemoveEmptyEntries);
        }
        
        List<Gamemode> allowedGamemodes = new List<Gamemode>() { Gamemode.Standard };
        // lambda expression
        var paragraphList = paragraphs.Select(
            text => new Paragraph(text, allowedGamemodes)).ToList();

        return paragraphList;

    }

    public Paragraph GetRandomParagraph(Gamemode gamemode)
    {
        List<Paragraph> allParagraphs = GetAllParagraphs();
        
        // listing the paragraphs that are allowed for a gamemode
        List<Paragraph> filteredParagraphs = allParagraphs.Where(
            p => p.AllowedGamemodes.Contains(gamemode)).ToList();

        if (!filteredParagraphs.Any())
        {
            Console.WriteLine("Error! No paragraphs found for the specified gamemode.");
            return null;
        }
        
        var random = new Random();
        
        // explanation
        // return a random paragraph by generating a random number between 0 and the length of the list
        
        return filteredParagraphs[random.Next(filteredParagraphs.Count)];
    }

    public IActionResult GetParagraphText(Gamemode gamemode)
    {
        Paragraph paragraph = GetRandomParagraph(gamemode);
        if (paragraph == null)
        {
            return NotFound(new { message = "No paragraphs found for specified gamemode." });
        }
        return Json(paragraph);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
