using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace tcp
{
    class file_client
    {
        /// <summary>
        /// The PORT.
        /// </summary>
        const int PORT = 9000;
        /// <summary>
        /// The BUFSIZE.
        /// </summary>
        const int BUFSIZE = 1000;

        /// <summary>
        /// Initializes a new instance of the <see cref="file_client"/> class.
        /// </summary>
        /// <param name='args'>
        /// The command-line arguments. First ip-adress of the server. Second the filename
        /// </param>
        private file_client(string[] args)
        {
            // TO DO Your own code
            TcpClient clientSocket = new TcpClient();

            Console.WriteLine("Client started");
            clientSocket.Connect(args[0], PORT);
            Console.WriteLine("Server connected");

            NetworkStream networkStream = clientSocket.GetStream();
            string fileName = args[1];

            LIB.writeTextTCP(networkStream, fileName);

            receiveFile(fileName, networkStream);

            clientSocket.Close();
            networkStream.Close();
        }

        /// <summary>
        /// Receives the file.
        /// </summary>
        /// <param name='fileName'>
        /// File name.
        /// </param>
        /// <param name='io'>
        /// Network stream for reading from the server
        /// </param>
        private void receiveFile(String fileName, NetworkStream io)
        {
            // TO DO Your own code
            string filename = LIB.extractFileName(fileName);
            long fileSize = LIB.getFileSizeTCP(io);
            FileStream fs = new FileStream("/root/Downloads/" + filename, FileMode.Create, FileAccess.Write);
            byte[] recieveBytes = new byte[fileSize];
            int recieved = 0;

            if (fileSize == 0)
            {
                Console.WriteLine("File do not exist on server");
                return;
            }
            else
            {
                Console.WriteLine("File size: " + fileSize);
                while (recieved < fileSize)
                {
                    if ((fileSize - recieved) > BUFSIZE)
                    {
                        io.Read(recieveBytes, 0, BUFSIZE);
                        fs.Write(recieveBytes, 0, BUFSIZE);
                        recieved += BUFSIZE;
                    }
                    else
                    {
                        int read = io.Read(recieveBytes, 0, ((int)fileSize - recieved));
                        fs.Write(recieveBytes, 0, read);
                        recieved += read;
                    }
                    Console.WriteLine("Bytes recieved: " + recieved);
                }
            }
        }

        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name='args'>
        /// The command-line arguments.
        /// </param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Client starts...");
            new file_client(args);
        }
    }
}
