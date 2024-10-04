using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using Newtonsoft.Json.Linq;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using ControllerBase = Microsoft.AspNetCore.Mvc.ControllerBase;

[ApiController]
[Route("api/[controller]")]
public class GraphController : ControllerBase
{
    [HttpPost("generate")]
    public IActionResult GenerateGraph()
    {
        try
        {
            GenerateGraphInternal();
            return Ok(new { message = "Graph generated successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    private void GenerateGraphInternal()
    {
        // Read the JSON data
        var jsonData = System.IO.File.ReadAllText("wwwroot/statistics/game-data.json");
        var gameData = JObject.Parse(jsonData);

        // Extract data for the graph
        var typingData = gameData["TypingData"];
        var totalWords = (int)gameData["TotalAmountOfWords"];
        var wpmData = new double[totalWords];

        for (int i = 0; i < totalWords; i++)
        {
            wpmData[i] = (double)typingData[i]["CurrentWordsPerMinute"];
        }

        // Create the plot model
        var plotModel = new PlotModel { Title = "Words Per Minute" };
        var lineSeries = new LineSeries { Title = "WPM" };

        for (int i = 0; i < totalWords; i++)
        {
            lineSeries.Points.Add(new DataPoint(i + 1, wpmData[i]));
        }

        plotModel.Series.Add(lineSeries);

        // Save the plot as an image
        var pngExporter = new PngExporter { Width = 600, Height = 400 };
        using (var stream = System.IO.File.Create("wwwroot/images/wpm-graph.png"))
        {
            pngExporter.Export(plotModel, stream);
        }
    }
}