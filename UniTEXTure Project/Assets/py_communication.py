import socket
from UniTexture_pb2 import UniTexture  # Import your generated Protobuf class
import subprocess

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
                message = UniTexture()
                message.ParseFromString(data)
                print(data)
                yaml_path = message.YamlPath
                print(yaml_path)

                # RUN THE PYTHON SCRIPT HERE it should look like run_texture.py {yaml_path from the passed UniTexture_pb2.UniTexture object}
                subprocess.run(["python", "../../TEXTurePaper/scripts/run_texture.py", "../" + yaml_path])

                break

    finally:
        # Clean up the connection
        connection.close()