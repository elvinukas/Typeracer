using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using OxyPlot.SkiaSharp;
using Typeracer.Models;
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
            GenerateGraphInternal("red");
            return Ok(new { message = "Graph generated successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    private void GenerateGraphInternal(string WPMColor = "blue") // optional arguments
    {
        var jsonData = System.IO.File.ReadAllText("wwwroot/statistics/game-data.json"); // reads the JSON data
        var game = JsonConvert.DeserializeObject<Game>(jsonData);
        Console.WriteLine(game);
        
        var typingData = game.Statistics.TypingData; // extracts data for the graph
        var totalWords = game.Statistics.Paragraph.TotalAmountOfWords;
        var wpmData = new double[totalWords];
        var accuracyData = new double[totalWords];

        for (int i = 0; i < totalWords; i++)
        {
            wpmData[i] = (double)typingData[i].CurrentWordsPerMinute;
            accuracyData[i] = (double)typingData[i].CurrentAccuracy;
        }
        
        double minWpm = wpmData.Min() / 2; // LINQ
        double maxWpm = wpmData.Max(); // LINQ
        double padding = 5;
        double minY = minWpm - padding;
        double maxY = maxWpm + padding;
        
        var plotModel = new PlotModel // creates the plot model
        {
            
        };
        var wpmLineSeries = new LineSeries 
        { 
            Title = "WPM",
            Color = WPMColor == "red" ? OxyColor.FromRgb(206, 0, 0) : OxyColor.FromRgb(11, 94, 215),
            StrokeThickness = 3
        };
        var wpmAreaSeries = new AreaSeries
        {
            Color = WPMColor == "red" ? OxyColor.FromArgb(25, 206, 0, 0) : OxyColor.FromArgb(25, 11, 94, 215),
            Fill = WPMColor == "red" ? OxyColor.FromArgb(25, 206, 0, 0) : OxyColor.FromArgb(25, 11, 94, 215)
        };
        
        var accuracyLineSeries = new LineSeries 
        { 
            Title = "Accuracy",
            Color = OxyColors.DarkGreen,
            StrokeThickness = 3,
            YAxisKey = "RightAxis"
        };

        for (int i = 0; i < totalWords; i++)
        {
            wpmLineSeries.Points.Add(new DataPoint(i + 1, wpmData[i]));
            wpmAreaSeries.Points.Add(new DataPoint(i + 1, wpmData[i]));
            wpmAreaSeries.Points2.Add(new DataPoint(i + 1, minY));
            
            accuracyLineSeries.Points.Add(new DataPoint(i + 1, accuracyData[i]));
        }
        
        plotModel.Series.Add(accuracyLineSeries);
        plotModel.Series.Add(wpmAreaSeries);
        plotModel.Series.Add(wpmLineSeries);
        
        plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
        {
            Position = OxyPlot.Axes.AxisPosition.Bottom,
            Minimum = 1,
            Maximum = totalWords,
            MajorGridlineStyle = LineStyle.Solid,
            MajorGridlineColor = OxyColors.DarkGray,
            Layer = OxyPlot.Axes.AxisLayer.AboveSeries
        });

        plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
        {
            Position = OxyPlot.Axes.AxisPosition.Left,
            Title = "ŽPM",
            TitleColor = WPMColor == "red" ? OxyColor.FromRgb(206, 0, 0) : OxyColor.FromRgb(11, 94, 215),
            TitleFontWeight = OxyPlot.FontWeights.Bold,
            Minimum = minY,
            Maximum = maxY,
            MajorGridlineStyle = LineStyle.Solid,
            MajorGridlineColor = OxyColors.DarkGray,
            Layer = OxyPlot.Axes.AxisLayer.AboveSeries
        });
        
        plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
        {
            Position = OxyPlot.Axes.AxisPosition.Right,
            Title = "TIKSLUMAS (%)",
            TitleColor = OxyColors.DarkGreen,
            TitleFontWeight = OxyPlot.FontWeights.Bold,
            Minimum = 0,
            Maximum = 105,
            MajorGridlineStyle = LineStyle.Solid,
            MajorGridlineColor = OxyColors.DarkGray,
            Key = "RightAxis",
            Layer = OxyPlot.Axes.AxisLayer.AboveSeries
        });


        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var pngExporter = new OxyPlot.WindowsForms.PngExporter { Width = 1100, Height = 300 }; // saves the plot as an image
            using (var stream = System.IO.File.Create("wwwroot/images/wpm-graph.png"))
            {
                pngExporter.Export(plotModel, stream);
            }
        }
        else
        {
            var pngExporter = new OxyPlot.SkiaSharp.PngExporter { Width = 1100, Height = 300 }; // saves the plot as an image
            using (var stream = System.IO.File.Create("wwwroot/images/wpm-graph.png"))
            {
                pngExporter.Export(plotModel, stream);
            }
        }
        
    }
}