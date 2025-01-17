using DataCollection.Persistence;

namespace DataCollection.Browser;

public class BrowserActivityMonitor(IDataCollectionService dataCollectionService)
{
    private int _activeTab;
    private Uri? _activeUrl;
    private DateTimeOffset _activeTabStartTime;
    
    private readonly IDataCollectionService _dataCollectionService = dataCollectionService;
    
    public void ProcessNextMessage()
    {
        var message = MessageProcessor.Read();
        if (message == null)
        {
            return;
        }

        if (_activeTabStartTime == default)
        {
            MessageProcessor.Write("_activeTabStartTime is default");
            if (message.Action != Action.Activated)
            {
                return;
            }
            _activeTab = message.TabId;
            _activeUrl = new Uri(message.Url);
            _activeTabStartTime = message.Time;
            return;
        }

        var closeActiveTabMessage = message.Action == Action.Closed && message.TabId == _activeTab;
        if (message.Action != Action.Activated && message.Action != Action.Updated && !closeActiveTabMessage)
        {
            MessageProcessor.Write("message.Action is not Activated or Updated");
            return;
        }

        _dataCollectionService.Add(new BrowserData(_activeTab, _activeUrl, _activeTabStartTime,
            DateTimeOffset.UtcNow));

        if (message.Action == Action.Closed)
        {
            _activeTab = 0;
            _activeUrl = null;
            _activeTabStartTime = default;
            return;
        }

        _activeTab = message.TabId;
        _activeUrl = new Uri(message.Url);
        _activeTabStartTime = message.Time;

        MessageProcessor.Write("Activity received");
    }
}