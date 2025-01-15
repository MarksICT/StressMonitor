using DataCollection.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using DataCollection.Persistence.Entities;

namespace DataCollection.Windows;

public class WindowsDataCollectionService(DataCollectorDbContext dbContext) : IDataCollectionService
{
    private readonly DataCollectorDbContext _dbContext = dbContext;

    void IDataCollectionService.Add<T>(T data)
    {
        if (data is WindowsData windowsData)
        {
            Add(windowsData);
        }
    }
    void IDataCollectionService.AddRange<T>(IEnumerable<T> data)
    {
        if (data is IEnumerable<WindowsData> windowsData)
        {
            AddRange(windowsData);
        }
    }
    IEnumerable<T> IDataCollectionService.Get<T>() => Get().Cast<T>();
    IEnumerable<T> IDataCollectionService.Get<T>(DateTimeOffset start, DateTimeOffset end) => Get(start, end).Cast<T>();

    void IDataCollectionService.Delete<T>(T data)
    {
        if (data is WindowsData windowsData)
        {
            Delete(windowsData);
        }
    }
    void IDataCollectionService.DeleteRange<T>(IEnumerable<T> data)
    {
        if (data is IEnumerable<WindowsData> windowsData)
        {
            DeleteRange(windowsData);
        }
    }
    void IDataCollectionService.Delete<T>(DateTimeOffset start, DateTimeOffset end) => Delete(start, end);
    void IDataCollectionService.DeleteAll<T>() => DeleteAll();

    public void Add(WindowsData data)
    {
        _dbContext.WindowsData.Add(new WindowsDataEntity
        {
            Id = Guid.CreateVersion7(), ProcessFileName = data.ProcessFileName,
            ProcessFriendlyName = data.ProcessFriendlyName, StartTime = data.StartTime, StopTime = data.StopTime,
            WindowTitle = data.WindowTitle
        });
        _dbContext.SaveChanges();
    }

    public void AddRange(IEnumerable<WindowsData> data)
    {
        _dbContext.AddRange(data.Select(d => new WindowsDataEntity
        {
            Id = Guid.CreateVersion7(),
            ProcessFileName = d.ProcessFileName,
            ProcessFriendlyName = d.ProcessFriendlyName,
            StartTime = d.StartTime,
            StopTime = d.StopTime,
            WindowTitle = d.WindowTitle
        }));
        _dbContext.SaveChanges();
    }

    public IEnumerable<WindowsData> Get()
    {
        return _dbContext.WindowsData.Select(data => new WindowsData(data.WindowTitle ?? string.Empty,
            data.ProcessFileName ?? string.Empty, data.ProcessFriendlyName ?? string.Empty, data.StartTime,
            data.StopTime));
    }

    public IEnumerable<WindowsData> Get(DateTimeOffset start, DateTimeOffset end) 
    {
        return _dbContext.WindowsData
            .Where(data => data.StartTime >= start && data.StopTime <= end)
            .Select(data => new WindowsData(data.WindowTitle ?? string.Empty, data.ProcessFileName ?? string.Empty,
                data.ProcessFriendlyName ?? string.Empty, data.StartTime, data.StopTime));
    }

    public void Delete(WindowsData data) 
    {
        var entity = _dbContext.WindowsData.FirstOrDefault(e => data.Equals(e));
        if (entity is not null)
        {
            _dbContext.WindowsData.Remove(entity);
            _dbContext.SaveChanges();
        }
    }

    public void DeleteRange(IEnumerable<WindowsData> data) 
    {
        var entities = _dbContext.WindowsData.Where(e => data.Any(d => d.Equals(e)));
        _dbContext.WindowsData.RemoveRange(entities);
        _dbContext.SaveChanges();
    }

    public void Delete(DateTimeOffset start, DateTimeOffset end)
    {
        var entities = _dbContext.WindowsData.Where(e => e.StartTime >= start && e.StopTime <= end);
        _dbContext.WindowsData.RemoveRange(entities);
        _dbContext.SaveChanges();
    }

    public void DeleteAll() 
    {
        _dbContext.WindowsData.RemoveRange(_dbContext.WindowsData);
        _dbContext.SaveChanges();
    }
}