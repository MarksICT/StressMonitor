namespace DataCollection.Common;

public static class AccumulatedDataExtensions
{
    public static IEnumerable<IAccumulatedData> AggregateTime<T>(this IEnumerable<T> enumeratedData) where T : class, IData
    {
        return enumeratedData switch
        {
            T[] enumeratedDataArray => AggregateTime(enumeratedDataArray),
            List<T> enumeratedDataList => AggregateTime(enumeratedDataList),
            _ => AggregateTime(enumeratedData.ToArray())
        };
    }

    private static IEnumerable<IAccumulatedData> AggregateTime<T>(List<T> enumeratedData) where T : class, IData
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

    private static IEnumerable<IAccumulatedData> AggregateTime<T>(T[] enumeratedData) where T : class, IData
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