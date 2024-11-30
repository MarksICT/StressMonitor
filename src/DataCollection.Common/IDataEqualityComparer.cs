namespace DataCollection.Common;

public interface IDataEqualityComparer<in T> : IEqualityComparer<IData> where T : IData
{
    bool IEqualityComparer<IData>.Equals(IData? x, IData? y) => Equals((T?)x, (T?)y);
    int IEqualityComparer<IData>.GetHashCode(IData obj) => GetHashCode((T)obj);
    bool Equals(T? x, T? y);
    bool CategoryEquals(T? x, T? y);
    int GetHashCode(T obj);
}