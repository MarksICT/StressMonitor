using DataCollection.Browser;
using DataCollection.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var serviceCollection = new ServiceCollection()
    .AddDbContext<DataCollectorDbContext>()
    .AddScoped<IDataCollectionService, BrowserDataCollectionService>();

if (args.Contains("--enable-logging"))
{
    // Add logging
}

var services = serviceCollection.BuildServiceProvider();

services.GetRequiredService<DataCollectorDbContext>().Database.Migrate();

var browserDataCollectionService = services.CreateScope().ServiceProvider.GetRequiredService<IDataCollectionService>();
var monitor = new BrowserActivityMonitor(browserDataCollectionService);

// Continuously read messages
while (true)
{
    try
    {
        monitor.ProcessNextMessage();
    }
    catch (Exception ex)
    {
        File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "fatal_exception.log"), ex.Message + "\n\n" + ex.StackTrace);
    }
}