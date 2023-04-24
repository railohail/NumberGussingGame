using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    private static TcpListener listener;
    private static List<TcpClient> clients = new List<TcpClient>();
    private static int secretNumber;
    private static Random random = new Random();

    static void Main(string[] args)
    {
        listener = new TcpListener(IPAddress.Any, 8080);
        listener.Start();
        Console.WriteLine("Server started...");

        GenerateSecretNumber();

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("Client connected...");
            clients.Add(client);
            Thread clientThread = new Thread(() => HandleClient(client));
            clientThread.Start();
        }
    }

    private static void GenerateSecretNumber()
    {
        secretNumber = random.Next(1, 101);
        Console.WriteLine($"Secret number generated: {secretNumber}");
    }

    private static void HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        StreamReader reader = new StreamReader(stream);
        StreamWriter writer = new StreamWriter(stream);

        writer.WriteLine("Welcome to the Guessing Game! Guess a number between 1 and 100:");
        writer.Flush();

        while (true)
        {
            string input = reader.ReadLine();
            int guess;

            if (int.TryParse(input, out guess))
            {
                Console.WriteLine($"Client guessed: {guess}");
                if (guess == secretNumber)
                {
                    BroadcastMessage("Congratulations! The correct number has been guessed!");
                    GenerateSecretNumber();
                    BroadcastMessage("A new secret number has been generated. Guess a number between 1 and 100:");
                }
                else
                {
                    int difference = Math.Abs(guess - secretNumber);
                    string feedback = difference < 10 ? "Hot" : "Cold";
                    BroadcastMessage($"Client guessed {guess}: {feedback}");
                }
            }
            else
            {
                writer.WriteLine("Invalid input. Please enter a number between 1 and 100:");
                writer.Flush();
            }
        }
    }

    private static void BroadcastMessage(string message)
    {
        byte[] data = Encoding.ASCII.GetBytes(message + "\r\n");
        foreach (TcpClient client in clients)
        {
            NetworkStream stream = client.GetStream();
            stream.Write(data, 0, data.Length);
        }
    }
}
