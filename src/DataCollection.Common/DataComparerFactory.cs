namespace DataCollection.Common;

public class DataComparerFactory<T> where T : IData
{
    public delegate IDataEqualityComparer<T> DataComparerFactoryMethod();
    public delegate Task<IDataEqualityComparer<T>> DataComparerFactoryMethodAsync();
    protected static Dictionary<Type, DataComparerFactoryMethod> Factories { get; set; } = [];
    protected static Dictionary<Type, DataComparerFactoryMethodAsync> FactoriesAsync { get; set; } = [];

    public static bool IsRegistered()
    {
        return Factories.TryGetValue(typeof(T), out _) || FactoriesAsync.TryGetValue(typeof(T), out _);
    }

    public static void RegisterFactory(DataComparerFactoryMethod factoryMethod)
    {
        Factories[typeof(T)] = factoryMethod;
    }

    public static void RegisterFactory(DataComparerFactoryMethodAsync factoryMethod)
    {
        FactoriesAsync[typeof(T)] = factoryMethod;
    }

    public static IDataEqualityComparer<T> CreateDataComparer()
    {
        if (Factories.TryGetValue(typeof(T), out var factoryMethod))
        {
            return factoryMethod();
        }

        if (FactoriesAsync.TryGetValue(typeof(T), out var factoryMethodAsync))
        {
            var task = factoryMethodAsync();
            task.Wait();
            return task.Result;
        }

        throw new ArgumentException($"No factory registered for type {typeof(T)}", nameof(T));
    }

    public static async Task<IDataEqualityComparer<T>> CreateAsync()
    {
        if (FactoriesAsync.TryGetValue(typeof(T), out var factoryMethodAsync))
        {
            return await factoryMethodAsync();
        }

        if (Factories.TryGetValue(typeof(T), out var factoryMethod))
        {
            return factoryMethod();
        }

        throw new ArgumentException($"No factory registered for type {typeof(T)}", nameof(T));
    }
}