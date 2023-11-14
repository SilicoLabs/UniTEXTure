using System;
using System.Net.Sockets;
using Google.Protobuf;
using stimulus; // Add this using directive

namespace ProtoNamespace
{
    class ProtoTest
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World");

            stimulus.MyMessage message = new stimulus.MyMessage // Specify the full namespace
            {
                Id = 1,
                Name = "John"
            };

            byte[] bytes = message.ToByteArray();

            // Assume Python server is running on localhost, port 5555
            string pythonServerAddress = "127.0.0.1";
            int pythonServerPort = 5555;

            using (TcpClient client = new TcpClient(pythonServerAddress, pythonServerPort))
            using (NetworkStream stream = client.GetStream())
            {
                // Send the serialized protobuf message to Python
                stream.Write(bytes, 0, bytes.Length);
            }
        }
    }

    // You can keep the MyMessage class here, or move it to a separate file if needed
    public partial class MyMessage : global::ProtoBuf.IExtensible
    {
        // ... other code
    }
}
