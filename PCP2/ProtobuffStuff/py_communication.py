import socket
import random
from ProtoFile_pb2 import YourMessage  # Import your generated Protobuf class

# Set up a TCP/IP socket
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

# Bind the socket to a public host, and a well-known port
sock.bind(("127.0.0.1", 5555))  

# Activate the server; this will keep running until you interrupt the program with Ctrl+C
sock.listen(1)
print("Server listening...")

while True:
    # Accept new connections
    connection, client_address = sock.accept()
    try:
        while True:
            # Receive the data in small chunks and retransmit it
            data = connection.recv(1024)
            print("signal received")
            if data:

                your_message = YourMessage()
                your_message.ParseFromString(data)
                print(data)
                print(your_message)
                break

                # serialized_message = response.SerializeToString()
                # length_prefix = len(serialized_message).to_bytes(4, byteorder='little')
                # connection.send(length_prefix + serialized_message)
                # break
    finally:
        # Clean up the connection
        connection.close()