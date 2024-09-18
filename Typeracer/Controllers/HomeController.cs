using System.Diagnostics;
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

    public string GetRandomParagraph()
    {
        // getting the file path
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Paragraphs", "paragraph1.txt");
        Console.Write(filePath);
        
        // splitting the text into paragraphs
        var paragraphs = System.IO.File.ReadAllText(filePath).Split("\n", StringSplitOptions.RemoveEmptyEntries);
        

        var random = new Random();

        // explanation
        // return a random paragraph by generating a random number between 0 and the length of the list
        return paragraphs[random.Next(paragraphs.Length)];
    }

    public IActionResult GetParagraphText()
    {
        var text = GetRandomParagraph();
        return Json(new {text});
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
