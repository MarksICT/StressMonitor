namespace DataCollection.Common;

public interface IDataCollector
{
    void AddData(IData data);
    IAccumulatedData[] GetAccumulatedData();
}