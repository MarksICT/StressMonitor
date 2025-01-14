using DataCollection.Common;

namespace DataCollection.Persistence;

public interface IDataCollectionService
{
    void Add<T>(T data) where T : IData;
    void AddRange<T>(IEnumerable<T> data) where T : IData;
    IEnumerable<T> Get<T>() where T : IData;
    IEnumerable<T> Get<T>(DateTimeOffset start, DateTimeOffset end) where T : IData;
    void Delete<T>(T data) where T : IData;
    void DeleteRange<T>(IEnumerable<T> data) where T : IData;
    void Delete<T>(DateTimeOffset start, DateTimeOffset end) where T : IData;
    void DeleteAll<T>() where T : IData;
}