using System;
using System.IO.Ports;

/// <summary>
/// Link.
/// </summary>
namespace Linklaget
{
	/// <summary>
	/// Link.
	/// </summary>
	public class Link
	{
		/// <summary>
		/// The DELIMITE for slip protocol.
		/// </summary>
		const byte DELIMITER = (byte)'A';
		/// <summary>
		/// The buffer for link.
		/// </summary>
		private byte[] buffer;
		/// <summary>
		/// The serial port.
		/// </summary>
		SerialPort serialPort;

		/// <summary>
		/// Initializes a new instance of the <see cref="link"/> class.
		/// </summary>
		public Link (int BUFSIZE)
		{
			// Create a new SerialPort object with default settings.
			serialPort = new SerialPort ("/dev/ttyS0", 115200, Parity.None, 8, StopBits.One);

			if (!serialPort.IsOpen)
				serialPort.Open ();

			buffer = new byte[(BUFSIZE * 2) + 2];
		}

		/// <summary>
		/// Send the specified buf and size.
		/// </summary>
		/// <param name='buf'>
		/// Buffer.
		/// </param>
		/// <param name='size'>
		/// Size.
		/// </param>
		public void send (byte[] buf, int size)
		{
			buffer [0] = 65; //Insert A at start
			int bytesToSendIndex = 1;
			for (int i = 0; i < size; i++) {
				if (buf [i] == 65) {
					buffer [bytesToSendIndex] = 66;
					bytesToSendIndex++;
					buffer [bytesToSendIndex] = 67;
					bytesToSendIndex++;
				} else if (buf [i] == 66) {
					buffer [bytesToSendIndex] = 66;
					bytesToSendIndex++;
					buffer [bytesToSendIndex] = 68;
					bytesToSendIndex++;
				} else {
					buffer [bytesToSendIndex] = buf [i];
					bytesToSendIndex++;
				}
			}
			buffer [bytesToSendIndex] = 65;
			bytesToSendIndex++;
			serialPort.Write (buffer, 0, bytesToSendIndex);
		}

		/// <summary>
		/// Receive the specified buf and size.
		/// </summary>
		/// <param name='buf'>
		/// Buffer.
		/// </param>
		/// <param name='size'>
		/// Size.
		/// </param>
		public int receive (ref byte[] buf)
		{
			// TO DO Your own code
			byte[] tempBuf = new byte[1]; //temp buffer to read bytes one at a time
			int bytesReceived = 0;
			while (true) {
				serialPort.Read (tempBuf, 0, 1);

				while (tempBuf [0] != 'A') {
				}
				bool registerStop = false;

				while (!registerStop) {

					serialPort.Read (tempBuf, 0, 1);

					if (tempBuf [0] == 'A')
						registerStop = true;
					else if (tempBuf [0] == 'B') {
						serialPort.Read (tempBuf, 0, 1);

						if (tempBuf [0] == 'C') {
							buf [bytesReceived] = 65;
							bytesReceived++;
						} else if (tempBuf [0] == 'D') {
							buf [bytesReceived] = 66;
							bytesReceived++;
						} else
							Console.WriteLine ("invalid byte after B received");
					} else {
						buf [bytesReceived] = tempBuf [0];
						bytesReceived++;
					}
				}
				return bytesReceived;
			}
		}
	}
}
