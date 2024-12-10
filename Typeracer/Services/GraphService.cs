using Typeracer.Models;
using System.Threading.Tasks;
using System.IO;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using OxyPlot.SkiaSharp;
using System.Runtime.InteropServices;

namespace Typeracer.Services;

public class GraphService : IGraphService
{
    public async Task GenerateGraphAsync (Game game, int totalWords, string WPMColor = "blue") // optional arguments
    {
        var typingData = game.Statistics.TypingData;
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
        
        // creating a direcotory if it doesn't exist
        string directoryPath = Path.Combine(AppContext.BaseDirectory, "wwwroot/images");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string filePath = Path.Combine(directoryPath, "wpm-graph.png");

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var pngExporter = new OxyPlot.WindowsForms.PngExporter { Width = 1100, Height = 300 }; // saves the plot as an image
            using (var stream = File.Create(filePath))
            {
                pngExporter.Export(plotModel, stream);
            }
        }
        else
        {
            var pngExporter = new OxyPlot.SkiaSharp.PngExporter { Width = 1100, Height = 300 }; // saves the plot as an image
            using (var stream = File.Create(filePath))
            {
                pngExporter.Export(plotModel, stream);
            }
        }
        
    }
}