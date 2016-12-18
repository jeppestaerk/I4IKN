using System;
using System.IO;
using System.Text;
using Transportlaget;

namespace serial
{
	class file_server
	{
		private const int BUFSIZE = 1000;
		const int PORT = 9000;

		/// <summary>
		/// Initializes a new instance of the <see cref="file_server"/> class.
		/// </summary>
		private file_server (Transport transport)
		{
			try
			{
				Console.WriteLine("Server køre - afventer Klient");
				string fileToSend = transport.readText();
				Console.WriteLine("Klient forbundet, ønsker at afhente: " + fileToSend);
				long fileSize = LIB.check_File_Exists (fileToSend);
				if (fileSize != 0)
				{
					transport.sendText("FileFound");
					Console.WriteLine ("Filen blev fundet på serveren");
					sendFile (fileToSend, fileSize, transport);
				} 
				else 
				{
					transport.sendText("FileNotFound");
					Console.WriteLine ("Filen blev IKKE fundet på serveren");
				}
			}
			catch (Exception ex) 
			{
				Console.WriteLine (ex.Message);
			}
			finally
			{
				Console.WriteLine ("Afslutter");
			}
		}

		/// <summary>
		/// Sends the file.
		/// </summary>
		/// <param name='fileName'>
		/// File name.
		/// </param>
		/// <param name='fileSize'>
		/// File size.
		/// </param>
		/// <param name='tl'>
		/// Tl.
		/// </param>
		private void sendFile(String fileName, long fileSize, Transport transport)
		{
			FileStream fileStream = null;

			try
			{
				Console.WriteLine ("Størrelse på fil: " + fileSize);
				transport.sendText(fileSize.ToString());
				byte[] SendingBuffer = null;
				fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
				int NoOfPackets = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(fileStream.Length) / Convert.ToDouble(BUFSIZE)));
				int TotalLength = (int)fileStream.Length;
				int CurrentPacketLength, bytesSent = 0;
				for (int i = 1; i < NoOfPackets+1; i++)
				{
					if (TotalLength > BUFSIZE) 
					{
						CurrentPacketLength = BUFSIZE;
						TotalLength = TotalLength - CurrentPacketLength;
						bytesSent += BUFSIZE;
					} 
					else 
					{
						CurrentPacketLength = TotalLength;
						bytesSent += CurrentPacketLength;
					}
					SendingBuffer = new byte[CurrentPacketLength];
					fileStream.Read(SendingBuffer, 0, CurrentPacketLength);
					transport.send(SendingBuffer, (int)SendingBuffer.Length);
					Console.Write("\rAfsendt " + i + " af " + NoOfPackets + " pakker til klienten. Ialt " + bytesSent + " bytes afsendt");
				}
				Console.WriteLine ("\nFilen blev afsendt - Lukker forbindelsen");
			}
			catch (Exception ex) 
			{
				Console.WriteLine (ex.Message);
			}
			finally 
			{
				if (fileStream != null)
					fileStream.Close ();
			}
		}

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		public static void Main (string[] args)
		{
			while(true)
			{
				Transport trans = new Transport (BUFSIZE);
				new file_server (trans);
			}
		}
	}
}
