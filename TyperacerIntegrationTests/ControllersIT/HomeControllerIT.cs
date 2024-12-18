using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Typeracer.Context;
using Typeracer.Controllers;
using Typeracer.Models;
using Xunit.Abstractions;

namespace TyperacerIntegrationTests.ControllersIT;

public class HomeControllerIT : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly AppDbContext _context;
    private readonly ILogger<HomeController> _logger;
    private readonly Dictionary<string, List<Gamemode>> _paragraphFiles;

    public HomeControllerIT(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        var serviceProvider = factory.Services;
        _context = serviceProvider.GetRequiredService<AppDbContext>();
        _logger = new LoggerFactory().CreateLogger<HomeController>();
        _paragraphFiles = serviceProvider.GetRequiredService<Dictionary<string, List<Gamemode>>>();
    }

    [Fact]
    public void ParameterizedConstructor_ShouldInitializeProperties()
    {
        var controller = new HomeController(_logger, _context, _paragraphFiles);

        Assert.NotNull(controller);
        var loggerField = controller.GetType().GetField("_logger", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(loggerField.GetValue(controller));
        var dbContextField = controller.GetType().GetField("_dbContext", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(dbContextField.GetValue(controller));
        
    }
    
    [Fact]
    public void HomeController_Constructor_ShouldInsertParagraphsToDb()
    {
        var controller = new HomeController(_logger, _context, _paragraphFiles);

        var paragraphs = _context.Paragraphs.ToList();
        Assert.NotEmpty(paragraphs);
    }
    
    [Fact]
    public async Task Index_ShouldReturnView()
    {
        var response = await _client.GetAsync("/Home/Index");

        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Contains("<title>", responseString);
    }
    
    [Fact]
    public async Task Privacy_ShouldReturnView()
    {
        var response = await _client.GetAsync("/Home/Privacy");

        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Contains("<title>", responseString);
    }

    [Fact]
    public void GetRandomParagraph_ShouldReturnParagraph_ForValidGamemode()
    {
        var controller = new HomeController(_logger, _context, _paragraphFiles);
        var gamemode = Gamemode.Standard;
        
        var paragraph = controller.GetRandomParagraph(gamemode);
        
        Assert.NotNull(paragraph);
        Assert.Contains(gamemode, paragraph.AllowedGamemodes);
    }
    
    [Fact]
    public void GetRandomParagraph_ShouldReturnNull_ForInvalidGamemode()
    {
        var controller = new HomeController(_logger, _context, _paragraphFiles);
        var gamemode = (Gamemode)999;

        var paragraph = controller.GetRandomParagraph(gamemode);

        Assert.Null(paragraph);
    }
    
    [Fact]
    public void GetParagraphText_ShouldReturnJsonResult_ForValidGamemode()
    {
        var controller = new HomeController(_logger, _context, _paragraphFiles);
        var gamemode = Gamemode.Short;

        var result = controller.GetParagraphText(gamemode) as JsonResult;

        Assert.NotNull(result);
        var paragraph = result.Value as Paragraph;
        Assert.NotNull(paragraph);
        Assert.Contains(gamemode, paragraph.AllowedGamemodes);
    }
    
    [Fact]
    public void GetParagraphText_ShouldReturnNotFound_ForInvalidGamemode()
    {
        var controller = new HomeController(_logger, _context, _paragraphFiles);
        var gamemode = (Gamemode)999;

        var result = controller.GetParagraphText(gamemode);
        
        Assert.IsType<NotFoundObjectResult>(result);
        var notFoundResult = result as NotFoundObjectResult;
        var message = notFoundResult.Value.GetType().GetProperty("message").GetValue(notFoundResult.Value, null).ToString();
        Assert.Equal("No paragraphs found for specified gamemode.", message);
    }

    [Fact]
    public void Error_ShouldReturnViewResult_WithErrorViewModel()
    {
        var controller = new HomeController(_logger, _context, _paragraphFiles);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        
        var result = controller.Error() as ViewResult;
        
        Assert.NotNull(result);
        var model = result.Model as ErrorViewModel;
        Assert.NotNull(model);
        Assert.Equal(Activity.Current?.Id ?? controller.HttpContext.TraceIdentifier, model.RequestId);
    }
}