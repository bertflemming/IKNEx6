using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace tcp
{
    class file_server
    {
        /// <summary>
        /// The PORT
        /// </summary>
        const int PORT = 9000;
        /// <summary>
        /// The BUFSIZE
        /// </summary>
        const int BUFSIZE = 1000;

        /// <summary>
        /// Initializes a new instance of the <see cref="file_server"/> class.
        /// Opretter en socket.
        /// Venter på en connect fra en klient.
        /// Modtager filnavn
        /// Finder filstørrelsen
        /// Kalder metoden sendFile
        /// Lukker socketen og programmet
        /// </summary>
        private file_server()
        {
            // TO DO Your own code

            TcpListener serverSocket = new TcpListener(IPAddress.Any, PORT);
            serverSocket.Start();
            Console.WriteLine("Server started");


            while (true)
            {
                try
                {
                    TcpClient clientSocket = serverSocket.AcceptTcpClient();
                    Console.WriteLine("Accept connection from client");
                    NetworkStream networkStream = clientSocket.GetStream();
                    string fileName = LIB.readTextTCP(networkStream);
                    long fileSize = LIB.check_File_Exists(fileName);

                    if (fileSize == 0)
                    {
                        Console.WriteLine("File does not exist");
                    }
                    else
                    {
                        LIB.writeTextTCP(networkStream, fileSize.ToString());

                        sendFile(fileName, fileSize, networkStream);

                        Console.WriteLine("File send");
                    }

                    clientSocket.Close();
                    networkStream.Close();

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }


        }

        /// <summary>
        /// Sends the file.
        /// </summary>
        /// <param name='fileName'>
        /// The filename.
        /// </param>
        /// <param name='fileSize'>
        /// The filesize.
        /// </param>
        /// <param name='io'>
        /// Network stream for writing to the client.
        /// </param>
        private void sendFile(String fileName, long fileSize, NetworkStream io)
        {
            // TO DO Your own code
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            byte[] sendBytes = new byte[BUFSIZE];
            int send = 0;

            while (send < fileSize)
            {
                if ((fileSize - send) > BUFSIZE)
                {
                    fs.Read(sendBytes, 0, BUFSIZE);
                    io.Write(sendBytes, 0, BUFSIZE);
                    send += BUFSIZE;
                }
                else
                {
                    int read = fs.Read(sendBytes, 0, ((int)fileSize - send));
                    io.Write(sendBytes, 0, read);
                    send += read;
                }
                Console.WriteLine("Bytes send: " + send);
            }

            fs.Close();
        }

        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name='args'>
        /// The command-line arguments.
        /// </param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Server starts...");
            new file_server();
        }
    }
}
