namespace DataCollection.Common;

public static class AccumulatedDataExtensions
{
    public static IEnumerable<IAccumulatedData> AggregateTime<T>(this List<T> enumeratedData) where T : class, IData
    {
        var comparer = DataComparerFactory<T>.CreateDataComparer();
        var distinctData = enumeratedData.Distinct(comparer);

        foreach (var data in distinctData)
        {
            var timeSpan = data.StopTime - data.StartTime;
            foreach (var item in enumeratedData)
            {
                if (ReferenceEquals(item, data))
                {
                    continue;
                }
                
                if (comparer.CategoryEquals(item, (T)data))
                {
                    timeSpan += item.StopTime - item.StartTime;
                }
            }

            yield return AccumulatedDataFactory<T>.CreateAccumulatedData((T)data, timeSpan);
        }
    }
}