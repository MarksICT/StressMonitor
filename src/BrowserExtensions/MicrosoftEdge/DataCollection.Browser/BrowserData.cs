using DataCollection.Common;

namespace DataCollection.Browser;

public class BrowserData(int tabId, Uri? uri, DateTimeOffset startTime, DateTimeOffset stopTime) : IData
{
    public int TabId { get; } = tabId;
    public Uri? Uri { get; } = uri;
    public DateTimeOffset StartTime { get; } = startTime;
    public DateTimeOffset StopTime { get; } = stopTime;

    public bool CategoryEquals(IData? other)
    {
        return other is BrowserData otherData && CategoryEquals(otherData);
    }

    public bool CategoryEquals(BrowserData? other)
    {
        return other is not null && other.TabId.Equals(TabId);
    }
}
