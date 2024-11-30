using DataCollection.Common;
using System.Collections.Generic;
using System.Linq;

namespace DataCollection.Windows;

public class WindowsDataCollector : IDataCollector
{
    private readonly List<WindowsData> _data = [];

    public WindowsDataCollector()
    {
        AccumulatedDataFactory<WindowsData>.RegisterFactory((properties, time) =>
            new AccumulatedWindowsData(properties.WindowTitle, properties.ProcessFileName, properties .ProcessFriendlyName, time));
        DataComparerFactory<WindowsData>.RegisterFactory(() => new WindowsDataComparer());
    }

    public IAccumulatedData[] GetAccumulatedData()
    {
        var accumulatedData = _data.AggregateTime().ToArray();
        return accumulatedData;
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
        _data.Add(data);
    }
}
