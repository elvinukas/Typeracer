using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Typeracer.Models;
using System.Text;
using Typeracer.Context;
using Typeracer.Exceptions;


namespace Typeracer.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _dbContext;

    public HomeController(ILogger<HomeController> logger, AppDbContext appDbContext)
    {
        _logger = logger;
        _dbContext = appDbContext;

        List<Gamemode> simpleGamemodes = new List<Gamemode>();
        simpleGamemodes.Add(Gamemode.Standard);
        simpleGamemodes.Add(Gamemode.Hardcore);

        List<Gamemode> shortGamemode = new List<Gamemode>();
        shortGamemode.Add(Gamemode.Short);
        
        try
        {
            InsertParagraphFileToDb("paragraph1.txt", simpleGamemodes);

            InsertParagraphFileToDb("paragraph2.txt", shortGamemode);
            
        }
        catch (NoAddToDBException e)
        {
            Console.WriteLine(e.Message, "Intentional? - " + e.Intentional);
        }
        
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    /*
    public IActionResult Type()
    {
        return View();
    }
    */

    public List<Paragraph> GetAllParagraphs(string paragraphName, List<Gamemode> allowedGamemodes) // optional arguments
    {   
        // creating empty list of paragraphs
        List<Paragraph> paragraphList = new List<Paragraph>();
        
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
        // listing the paragraphs that are allowed for a gamemode
        List<Paragraph> filteredParagraphs = _dbContext.Paragraphs.Where( // LINQ
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

    public void InsertParagraphFileToDb(string filename, List<Gamemode> gamemodes)
    {
        List<Paragraph> paragraphs = GetAllParagraphs(filename, gamemodes);

        using (var transaction = _dbContext.Database.BeginTransaction())
        {
            int i = 0;
            foreach (Paragraph paragraph in paragraphs)
            {
                if (!_dbContext.Paragraphs.Any(p => p.Text == paragraph.Text))
                {
                    _dbContext.Paragraphs.Add(paragraph);
                    ++i;
                    Console.WriteLine("Added a new " + i + " paragraph!");
                }
            }

            if (i == 0)
            {
                throw new NoAddToDBException("No new paragraphs found.", intentional: true);
            }
            
            _dbContext.SaveChanges();
            transaction.Commit();
        }
    }
    

    public IActionResult GetParagraphText(Gamemode gamemode = Gamemode.Short)
    {
        //Paragraph paragraph = GetRandomParagraph(gamemode: gamemode); // named arguments
        Paragraph paragraph = GetRandomParagraph(gamemode: gamemode); 
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
