namespace DataCollection.Browser;

public class BrowserMessage
{
    public Action Action { get; set; }
    public int TabId { get; set; }
    public string Url { get; set; } = string.Empty;
    public DateTime Time { get; set; }
}

public enum Action
{
    Activated,
    Updated,
    Closed
}