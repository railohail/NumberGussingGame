using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        TcpClient client = new TcpClient("127.0.0.1", 8080);
        Console.WriteLine("Connected to server...");

        Thread readThread = new Thread(() => ReadMessages(client));
        readThread.Start();

        while (true)
        {
            string input = Console.ReadLine();
            StreamWriter writer = new StreamWriter(client.GetStream());
            writer.WriteLine(input);
            writer.Flush();
        }
    }

    private static void ReadMessages(TcpClient client)
    {
        StreamReader reader = new StreamReader(client.GetStream());
        while (true)
        {
            string message = reader.ReadLine();
            Console.WriteLine(message);
        }
    }
}
