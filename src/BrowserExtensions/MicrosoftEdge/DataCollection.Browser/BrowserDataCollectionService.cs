using DataCollection.Persistence;
using DataCollection.Persistence.Entities;

namespace DataCollection.Browser;

public class BrowserDataCollectionService(DataCollectorDbContext dbContext) : IDataCollectionService
{
    private readonly DataCollectorDbContext _dbContext = dbContext;

    void IDataCollectionService.Add<T>(T data)
    {
        if (data is BrowserData browserData)
        {
            Add(browserData);
        }
    }

    public void Add(BrowserData data)
    {
        _dbContext.BrowserData.Add(new BrowserDataEntity
        {
            Id = Guid.CreateVersion7(),
            TabId = data.TabId,
            Uri = data.Uri,
            StartTime = data.StartTime,
            StopTime = data.StopTime,
        });

        _dbContext.SaveChanges();
    }

    void IDataCollectionService.AddRange<T>(IEnumerable<T> data)
    {
        if (data is IEnumerable<BrowserData> browserData)
        {
            AddRange(browserData);
        }
    }

    public void AddRange(IEnumerable<BrowserData> data)
    {
        _dbContext.AddRange(data.Select(d => new BrowserDataEntity
        {
            Id = Guid.CreateVersion7(),
            TabId = d.TabId,
            Uri = d.Uri,
            StartTime = d.StartTime,
            StopTime = d.StopTime,
        }));

        _dbContext.SaveChanges();
    }

    IEnumerable<T> IDataCollectionService.Get<T>() => Get().Cast<T>();
    
    public IEnumerable<BrowserData> Get()
    {
        return _dbContext.BrowserData.Select(data =>
            new BrowserData(data.TabId, data.Uri, data.StartTime, data.StopTime));
    }

    IEnumerable<T> IDataCollectionService.Get<T>(DateTimeOffset start, DateTimeOffset end) => Get(start, end).Cast<T>();

    public IEnumerable<BrowserData> Get(DateTimeOffset start, DateTimeOffset end)
    {
        return _dbContext.BrowserData
            .Where(data => data.StartTime >= start && data.StopTime <= end)
            .Select(data => new BrowserData(data.TabId, data.Uri, data.StartTime,
                data.StopTime));
    }

    void IDataCollectionService.Delete<T>(T data)
    {
        if (data is BrowserData browserData)
        {
            Delete(browserData);
        }
    }

    public void Delete(BrowserData data)
    {
        var entity = _dbContext.BrowserData.FirstOrDefault(e => data.Equals(e));
        if (entity is not null)
        {
            _dbContext.BrowserData.Remove(entity);
            _dbContext.SaveChanges();
        }
    }

    void IDataCollectionService.DeleteRange<T>(IEnumerable<T> data)
    {
        if (data is IEnumerable<BrowserData> browserData)
        {
            DeleteRange(browserData);
        }
    }

    public void DeleteRange(IEnumerable<BrowserData> data)
    {
        var entities = _dbContext.BrowserData.Where(e => data.Any(d => d.Equals(e)));
        _dbContext.BrowserData.RemoveRange(entities);
        _dbContext.SaveChanges();
    }

    void IDataCollectionService.Delete<T>(DateTimeOffset start, DateTimeOffset end) => Delete(start, end);

    public void Delete(DateTimeOffset start, DateTimeOffset end)
    {
        var entities = _dbContext.BrowserData.Where(e => e.StartTime >= start && e.StopTime <= end);
        _dbContext.BrowserData.RemoveRange(entities);
        _dbContext.SaveChanges();
    }

    void IDataCollectionService.DeleteAll<T>() => DeleteAll();

    public void DeleteAll()
    {
        _dbContext.BrowserData.RemoveRange(_dbContext.BrowserData);
        _dbContext.SaveChanges();
    }
}
