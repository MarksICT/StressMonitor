namespace DataCollection.Browser;

public class BrowserMessage
{
    public string Action { get; set; } = string.Empty;
    public int TabId { get; set; }
    public string Url { get; set; } = string.Empty;
    public DateTime Time { get; set; }
}