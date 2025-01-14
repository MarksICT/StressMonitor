using DataCollection.Browser;

// Continuously read messages
while (true)
{
    var message = MessageProcessor.Read();
    MessageProcessor.Write("Activity received");
}