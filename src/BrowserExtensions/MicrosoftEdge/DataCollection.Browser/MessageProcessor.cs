using System.Text.Json;
using System.Text.Json.Serialization;

namespace DataCollection.Browser;

public class MessageProcessor
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web){ Converters = { new JsonStringEnumConverter() } };

    public static BrowserMessage? Read()
    {
        // First read 4 bytes for message length (little-endian)
        var lengthBytes = new byte[4];
        var read = Console.OpenStandardInput().Read(lengthBytes, 0, 4);

        // If no bytes can be read, it typically indicates EOF or that the browser closed the pipe.
        if (read == 0)
        {
            Environment.Exit(0); // Exit the host.
        }

        var messageLength = BitConverter.ToInt32(lengthBytes, 0);
        // Now read the JSON message itself
        var buffer = new byte[messageLength];
        var totalRead = 0;

        while (totalRead < messageLength)
        {
            var bytesRead = Console.OpenStandardInput().Read(buffer, totalRead, messageLength - totalRead);
            if (bytesRead == 0) // EOF or pipe closed
            {
                Environment.Exit(0);
            }
            totalRead += bytesRead;
        }

        return JsonSerializer.Deserialize<BrowserMessage>(buffer, JsonSerializerOptions);
    }

    public static void Write(object message)
    {
        // Write it back with 4-byte length prefix
        var responseBytes = JsonSerializer.SerializeToUtf8Bytes(message, JsonSerializerOptions);
        var responseLength = BitConverter.GetBytes(responseBytes.Length);

        // Write length prefix
        Console.OpenStandardOutput().Write(responseLength, 0, responseLength.Length);

        // Write actual JSON
        Console.OpenStandardOutput().Write(responseBytes, 0, responseBytes.Length);
        Console.Out.Flush();
    }
}