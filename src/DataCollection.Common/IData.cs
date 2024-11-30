namespace DataCollection.Common;

public interface IData
{
    DateTimeOffset StartTime { get; }
    DateTimeOffset StopTime { get; }
    bool CategoryEquals(IData? other);
}
