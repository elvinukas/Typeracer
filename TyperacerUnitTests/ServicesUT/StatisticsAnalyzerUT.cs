using Typeracer.Services;
using System;
using System.Collections.Generic;
using Typeracer.Models;

namespace TyperacerUnitTests.ServicesUT;

public class StatisticsAnalyzerUT
{
    [Fact]
    public void FindBestItem_ShouldReturnMaxItem()
    {
        var wpmAnalyzer = new StatisticsAnalyzer<WPM>();
        var wpms = new List<WPM>
        {
            new WPM { Value = 34.2 },
            new WPM { Value = 43.9 },
            new WPM { Value = 50 },
            new WPM { Value = 98.1 },
            new WPM { Value = 40 }
        };
        
        var result = wpmAnalyzer.FindBestItem(wpms);
        Assert.Equal(98.1, result.Value);
    }
    
    [Fact]
    public void FindBestItem_ShouldThrowException_WhenItemsIsNull()
    {
        var wpmAnalyzer = new StatisticsAnalyzer<WPM>();
        List<WPM> wpms = null;
        Assert.Throws<InvalidOperationException>(() => wpmAnalyzer.FindBestItem(wpms));
    }
    
    [Fact]
    public void FindBestItem_ShouldThrowException_WhenItemsIsEmpty()
    {
        var wpmAnalyzer = new StatisticsAnalyzer<WPM>();
        var wpms = new List<WPM>();
        Assert.Throws<InvalidOperationException>(() => wpmAnalyzer.FindBestItem(wpms));
    }
    
    [Fact]
    public void CalculateAverage_ShouldReturnCorrectAverage()
    {
        var wpmAnalyzer = new StatisticsAnalyzer<WPM>();
        var wpms = new List<WPM>
        {
            new WPM { Value = 34.2 },
            new WPM { Value = 43.9 },
            new WPM { Value = 50 },
            new WPM { Value = 98.1 },
            new WPM { Value = 40 }
        };

        var result = wpmAnalyzer.CalculateAverage(wpms, w => w.Value);
        Assert.Equal(53.24, result, 2);
    }
    
    [Fact]
    public void CalculateAverage_ShouldThrowException_WhenItemsIsNull()
    {
        var wpmAnalyzer = new StatisticsAnalyzer<WPM>();
        List<WPM> wpms = null;
        Assert.Throws<InvalidOperationException>(() => wpmAnalyzer.CalculateAverage(wpms, w => w.Value));
    }

    [Fact]
    public void CalculateAverage_ShouldThrowException_WhenItemsIsEmpty()
    {
        var wpmAnalyzer = new StatisticsAnalyzer<WPM>();
        var wpms = new List<WPM>();
        Assert.Throws<InvalidOperationException>(() => wpmAnalyzer.CalculateAverage(wpms, w => w.Value));
    }
}