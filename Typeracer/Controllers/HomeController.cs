using System.Diagnostics;
using System.Net.Mime;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;
using Typeracer.Models;
using System.Text;


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

    public List<Paragraph> GetAllParagraphs(string paragraphName = "paragraph2.txt") // optional arguments
    {   
        // creating empty list of paragraphs
        List<Paragraph> paragraphList = new List<Paragraph>();
        
        // creating empty list of allowed gamemodes
        List<Gamemode> allowedGamemodes = new List<Gamemode>() { Gamemode.Standard };
        
        // getting the file path
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Paragraphs", paragraphName);
        
        // using Filestream to open file as a stream
        using (FileStream filestream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            // using StreamReader to read the file
            using (StreamReader reader = new StreamReader(filestream, Encoding.UTF8))
            {
                string line;
                // reading the file line by line
                while ((line = reader.ReadLine()) != null)
                {
                    if(!string.IsNullOrWhiteSpace(line))
                    {
                        paragraphList.Add(new Paragraph(line, allowedGamemodes));
                    }
                }
            }

        return paragraphList;
    }

    public Paragraph GetRandomParagraph(Gamemode gamemode)
    {
        List<Paragraph> allParagraphs = GetAllParagraphs();
        
        // listing the paragraphs that are allowed for a gamemode
        List<Paragraph> filteredParagraphs = allParagraphs.Where( // LINQ
            p => p.AllowedGamemodes.Contains(gamemode)).ToList();

        if (!filteredParagraphs.Any()) // LINQ
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
        Paragraph paragraph = GetRandomParagraph(gamemode: gamemode); // named arguments
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
