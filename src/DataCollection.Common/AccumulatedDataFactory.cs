namespace DataCollection.Common;

public class AccumulatedDataFactory<T> where T : IData
{
    public delegate IAccumulatedData AccumulatedDataFactoryMethod(T data, TimeSpan totalTime);
    public delegate Task<IAccumulatedData> AccumulatedDataFactoryMethodAsync(T data, TimeSpan totalTime);
    protected static Dictionary<Type, AccumulatedDataFactoryMethod> Factories { get; set; } = [];
    protected static Dictionary<Type, AccumulatedDataFactoryMethodAsync> FactoriesAsync { get; set; } = [];

    public static bool IsRegistered()
    {
        return Factories.TryGetValue(typeof(T), out _) || FactoriesAsync.TryGetValue(typeof(T), out _);
    }

    public static void RegisterFactory(AccumulatedDataFactoryMethod factoryMethod)
    {
        Factories[typeof(T)] = factoryMethod;
    }

    public static void RegisterFactory(AccumulatedDataFactoryMethodAsync factoryMethod)
    {
        FactoriesAsync[typeof(T)] = factoryMethod;
    }

    public static IAccumulatedData CreateAccumulatedData(T data, TimeSpan totalTime)
    {
        if (Factories.TryGetValue(typeof(T), out var factoryMethod))
        {
            return factoryMethod(data, totalTime);
        }

        if (FactoriesAsync.TryGetValue(typeof(T), out var factoryMethodAsync))
        {
            var task = factoryMethodAsync(data, totalTime);
            task.Wait();
            return task.Result;
        }

        throw new ArgumentException($"No factory registered for type {typeof(T)}", nameof(data));
    }

    public static async Task<IAccumulatedData> CreateAsync(T data, TimeSpan totalTime)
    {
        if (FactoriesAsync.TryGetValue(typeof(T), out var factoryMethodAsync))
        {
            return await factoryMethodAsync(data, totalTime);
        }

        if (Factories.TryGetValue(typeof(T), out var factoryMethod))
        {
            return factoryMethod(data, totalTime);
        }

        throw new ArgumentException($"No factory registered for type {typeof(T)}", nameof(data));
    }
}
