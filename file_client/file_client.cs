using System;
using System.IO;
using Transportlaget;

namespace Application
{
  class file_client
  {
    /// <summary>
    /// The BUFSIZE.
    /// </summary>
    const int BUFSIZE = 1000;


    /// <summary>
    /// Initializes a new instance of the <see cref="file_client"/> class.
    /// 
    /// file_client metoden opretter en peer-to-peer forbindelse
    /// Sender en forspÃ¸rgsel for en bestemt fil om denne findes pÃ¥ serveren
    /// Modtager filen hvis denne findes eller en besked om at den ikke findes (jvf. protokol beskrivelse)
    /// Lukker alle streams og den modtagede fil
    /// Udskriver en fejl-meddelelse hvis ikke antal argumenter er rigtige
    /// </summary>
    /// <param name='args'>
    /// Filnavn med evtuelle sti.
    /// </param>
    private file_client(String[] args, Transport transport)
    {
      Console.WriteLine("Retrieving file");

      string fileToReceive = (args.Length > 0) ? args[0] : "billede.jpg";

      transport.sendText(fileToReceive);

      //Read confirmation that file exists

      if (transport.readText() == "File found")
      {
        receiveFile(fileToReceive, transport);
      }
      else
      {
        Console.WriteLine("404 - File not found");
      }

    }

    /// <summary>
    /// Receives the file.
    /// </summary>
    /// <param name='fileName'>
    /// File name.
    /// </param>
    /// <param name='transport'>
    /// Transportlaget
    /// </param>
    private void receiveFile(String fileName, Transport transport)
    {
      long fileSize = long.Parse(transport.readText());

      Console.WriteLine("Size of file: " + fileSize);

      byte[] RecData = new byte[BUFSIZE];
      int RecBytes;

      int totalrecbytes = 0;
      FileStream Fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
      while (fileSize > totalrecbytes)
      {
        RecBytes = transport.receive(ref RecData);
        Fs.Write(RecData, 0, RecBytes);
        totalrecbytes += RecBytes;
        Console.Write("\rReceived " + totalrecbytes + " bytes from server");
      }

      Console.WriteLine("\nFile transfer complete - Closing connection");

      Fs.Close();
    }

    /// <summary>
    /// The entry point of the program, where the program control starts and ends.
    /// </summary>
    /// <param name='args'>
    /// First argument: Filname
    /// </param>
    public static void Main(string[] args)
    {
      Transport transport = new Transport(BUFSIZE);
      new file_client(args, transport);
    }
  }
}