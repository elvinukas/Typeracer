namespace Typeracer.Models;

public static class Extensions
{
    private static double averageWPM;
    private static double averageAccuracy;
    private static double bestWPM;
    
    public static double CalculateAverageWPM(this Player player)
    {
        if (player.WPMs == null || player.WPMs.Count == 0)
        {
            return -1;
        }
        
        averageWPM = player.WPMs.Average();
        return averageWPM;
    }

    public static double CalculateAverageAccuracy(this Player player)
    {
        if (player.Accuracies == null || player.Accuracies.Count == 0)
        {
            return -1;
        }
        
        averageAccuracy = player.Accuracies.Average();
        return averageAccuracy;
    }

    public static double CalculateBestWPM(this Player player)
    {
        if (player.WPMs == null || player.WPMs.Count == 0)
        {
            return -1;
        }
        
        bestWPM = player.WPMs.Max();
        return bestWPM;
    }

    public static double CalculatePerformanceScore(this Player player)
    {
        if (player.WPMs == null || player.WPMs.Count == 0 || player.Accuracies == null || player.Accuracies.Count == 0)
        {
            return -1;
        }

        if (averageWPM == 0)
        {
            averageWPM = player.WPMs.Average();
        }

        if (averageAccuracy == 0)
        {
            averageAccuracy = player.Accuracies.Average();
        }
        
        return (averageWPM * averageAccuracy) / 100;
    }
}