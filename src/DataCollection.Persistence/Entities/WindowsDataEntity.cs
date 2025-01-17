using System.ComponentModel.DataAnnotations;

namespace DataCollection.Persistence.Entities;

public class WindowsDataEntity
{
    public Guid Id { get; init; }
    [MaxLength(1000)]
    public string? WindowTitle { get; init; }
    [MaxLength(1000)]
    public string? ProcessFileName { get; init; }
    [MaxLength(1000)]
    public string? ProcessFriendlyName { get; init; }
    public DateTimeOffset StartTime { get; init; }
    public DateTimeOffset StopTime { get; init; }
}