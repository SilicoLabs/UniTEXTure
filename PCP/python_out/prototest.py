import socket
import stimulus_pb2  # Adjust to your correct module name

# Set up a TCP server on localhost, port 5555
server_address = ('127.0.0.1', 5555)

# Create a TCP socket
with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as server_socket:
    # Bind the socket to the server address
    server_socket.bind(server_address)

    # Listen for incoming connections
    server_socket.listen()

    print(f"Waiting for a connection on {server_address}")

    # Accept the connection
    connection, client_address = server_socket.accept()

    try:
        print("Connection from", client_address)

        # Receive the data
        received_bytes = connection.recv(1024)

        # Parse the protobuf message
        message = stimulus_pb2.MyMessage()
        message.ParseFromString(received_bytes)

        # Access the data
        print(f"Received: Id={message.id}, Name={message.name}")

    finally:
        # Clean up the connection
        connection.close()
