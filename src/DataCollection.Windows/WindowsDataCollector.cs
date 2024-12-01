using DataCollection.Common;
using DataCollection.Persistence;
using System.Linq;

namespace DataCollection.Windows;

public class WindowsDataCollector : IDataCollector
{
    private readonly IDataCollectionService _collectionService;

    public WindowsDataCollector(IDataCollectionService collectionService)
    {
        AccumulatedDataFactory<WindowsData>.RegisterFactory((properties, time) =>
            new AccumulatedWindowsData(properties.WindowTitle, properties.ProcessFileName, properties .ProcessFriendlyName, time));
        DataComparerFactory<WindowsData>.RegisterFactory(() => new WindowsDataComparer());
        _collectionService = collectionService;
    }

    public IAccumulatedData[] GetAccumulatedData()
    {
        var accumulatedData = _collectionService.Get<WindowsData>().AggregateTime();
        return accumulatedData as IAccumulatedData[] ?? accumulatedData.ToArray();
    }

    void IDataCollector.AddData(IData data)
    {
        if (data is WindowsData windowsData)
        {
            AddData(windowsData);
        }
    }

    public void AddData(WindowsData data)
    {
        _collectionService.Add(data);
    }
}
