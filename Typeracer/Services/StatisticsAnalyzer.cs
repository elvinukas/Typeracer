namespace Typeracer.Services;

public class StatisticsAnalyzer<T> where T: class, IComparable<T>
{
    public T FindBestItem(List<T> items)
    {
        if (items == null || items.Count == 0)
            throw new InvalidOperationException("No items available.");
        return items.Max();
    }
    
    public double CalculateAverage(List<T> items, Func<T, double> valueSelector)
    {
        if (items == null || items.Count == 0)
            throw new InvalidOperationException("No items available.");
        return items.Average(valueSelector);
    }
}