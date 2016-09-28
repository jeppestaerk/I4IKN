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
		private file_server ()
		{
			// Opretter en socket
			Console.WriteLine ("- Opretter en socket");
			IPAddress serverIpAdress = IPAddress.Parse("10.0.0.1");
			IPEndPoint serverIpEndPoint = new IPEndPoint(serverIpAdress, PORT);
			TcpListener serverSocket = new TcpListener(serverIpEndPoint);
			TcpClient clientSocket = default(TcpClient);
			serverSocket.Start();

			// Venter på en connect fra en klient
			Console.WriteLine ("- Venter på en connect fra en klient");
			clientSocket = serverSocket.AcceptTcpClient();
			NetworkStream networkStream = clientSocket.GetStream();

			// Modtager filnavn
			Console.WriteLine ("- Modtager filnavn");
			string requestedFile = LIB.readTextTCP(networkStream);

			// Finder filstørrelsen
			Console.WriteLine ("- Finder filstørrelsen");
			long fileSize = LIB.check_File_Exists(requestedFile);

			// Kalder metoden sendFile
			Console.WriteLine ("- Kalder metoden sendFile");
			if(fileSize > 0)
			{
				Console.WriteLine ("- Sender fil");
				LIB.writeTextTCP(networkStream, "Sending file");
				sendFile (requestedFile, fileSize, networkStream);
			}
			else
			{
				LIB.writeTextTCP (networkStream, "Error sending file");
			}

			// Lukker socketen og programmet
			Console.WriteLine ("- Lukker socketen og programmet");
			serverSocket.Stop();
			clientSocket.Close();
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
		private void sendFile (String fileName, long fileSize, NetworkStream io)
		{
			// Sends the file
			FileStream fileStream;
			int numberOfPackets = 0;
			int totalLenght = 0;

			fileStream = new FileStream (fileName, FileMode.Open, FileAccess.Read);
			numberOfPackets = Convert.ToInt32(Math.Ceiling (Convert.ToDouble(fileStream.Length) / Convert.ToDouble(BUFSIZE)));
			totalLenght = Convert.ToInt32(fileStream.Length);

			LIB.writeTextTCP(io, fileSize.ToString());

			for(int i = 0; i < numberOfPackets; i++)
			{
				int currentPacketsLenght = 0;

				if(totalLenght > BUFSIZE)
				{
					currentPacketsLenght = BUFSIZE;
					totalLenght -= currentPacketsLenght;
				}
				else
				{
					currentPacketsLenght = totalLenght;
				}

				byte[] sendingBuffer = new byte[currentPacketsLenght];
				fileStream.Read(sendingBuffer, 0, currentPacketsLenght);
				io.Write(sendingBuffer, 0, currentPacketsLenght);
			}

			fileStream.Close();
			io.Close();
		}

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		public static void Main (string[] args)
		{
			Console.WriteLine ("Server starts...");
			new file_server();
		}
	}
}
