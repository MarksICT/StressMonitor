using System.ComponentModel.DataAnnotations;

namespace DataCollection.Persistence.Entities;

public class BrowserDataEntity
{
    public Guid Id { get; init; }
    [MaxLength(50)]
    public int TabId { get; init; }
    public Uri? Uri { get; init; }
    public DateTimeOffset StartTime { get; init; }
    public DateTimeOffset StopTime { get; init; }
}