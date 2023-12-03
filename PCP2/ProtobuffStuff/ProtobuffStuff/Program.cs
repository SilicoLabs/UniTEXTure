// See https://aka.ms/new-console-template for more information
using Google.Protobuf;
using System.Net.Sockets;
using YourNamespace;

class Program
{
    static void Main(string[] args)
    {
        YourMessage message = new YourMessage
        {
            Id = 1,
            Name = "John"
        };

        byte[] bytes = message.ToByteArray();

        Console.WriteLine("Hello, World!");

        // Assume Python server is running on localhost, port 5555
        string pythonServerAddress = "127.0.0.1";
        int pythonServerPort = 5555;

        using (TcpClient client = new TcpClient(pythonServerAddress, pythonServerPort))
        using (NetworkStream stream = client.GetStream())
        {
            // Send the serialized protobuf message to Python
            stream.Write(bytes, 0, bytes.Length);
            // message.WriteTo(stream);
        }
    }
}
