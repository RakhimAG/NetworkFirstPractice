using System.Collections.Concurrent;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ClientChat;

// CLIENT

bool isRunning = true;

Console.Write("Write your name => ");
string clientName = Console.ReadLine()!;

using var client = new Socket(
    AddressFamily.InterNetwork,
    SocketType.Stream,
    ProtocolType.Tcp);

var port = 5000;
var serverAddress = new IPEndPoint(
    IPAddress.Loopback, port);


ConcurrentQueue<string> chatMessages = new ConcurrentQueue<string>();

async Task ReceiveChatMessagesAsync()
{
    byte[] buffer = new byte[1024];

    while (true)
    {
        try
        {
            var received = await client.ReceiveAsync(buffer);
            if (received == 0)
            {
                ConsoleMessages.PrintError("Server disconnected!");
                ConsoleMessages.PrintWarning("Program will close automaticly..");
                ConsoleMessages.PressToContinue();
                isRunning = false;
                break;
            }
            chatMessages.Enqueue(Encoding.UTF8.GetString(buffer, 0, received));
        }
        catch (Exception err)
        {
            ConsoleMessages.PrintError(err.Message);
            ConsoleMessages.PrintWarning("Program will close automaticly..");
            ConsoleMessages.PressToContinue();
            isRunning=false;
        }
    }
}

try
{
    await client.ConnectAsync(serverAddress);
    Console.WriteLine("Connected to the server!");

    Console.WriteLine("Sending your name to the server...");
    await client.SendAsync
    (
        Encoding.UTF8.GetBytes(clientName)
    );
    Console.WriteLine("Operation is completed successfuly");
    ConsoleMessages.PressToContinue();
}
catch (SocketException err)
{
    ConsoleMessages.PrintError(err.Message);
    ConsoleMessages.PrintWarning("While connecting to server, something went wrong. Try connecting later.");
    ConsoleMessages.PressToContinue();
    return;
}

var receiveMessagesTask = ReceiveChatMessagesAsync();

ConsoleKey userChoice;
while (isRunning)
{
    Console.Clear();

    ConsoleMessages.PrintChatOptions();
    Console.Write("Press a key according to the options ->");
    userChoice = Console.ReadKey(true).Key;
    switch (userChoice)
    {
        case ConsoleKey.D1:
        {
            Console.Write("\nType a message and press Enter =>");
            string clientMessage = Console.ReadLine()!;
            if (string.IsNullOrWhiteSpace(clientMessage))
            {
                ConsoleMessages.PrintWarning("You can't send empty message!");
                ConsoleMessages.PressToContinue();
                continue;
            }

            byte[] messageData = Encoding.UTF8.GetBytes(clientMessage);

            try
            {
                await client.SendAsync(messageData);
            }
            catch (SocketException)
            {
                ConsoleMessages.PrintError("Server got offline when tried to send the message!");
                ConsoleMessages.PrintWarning("Program will close automaticly..");
                ConsoleMessages.PressToContinue();
                isRunning = false;
            }
            break;
        }

        case ConsoleKey.D2:
        {
            Console.WriteLine('\n');
            Console.WriteLine(new string('=', 24));
            foreach (var message in chatMessages)
                    Console.WriteLine($"\n{message}");
            Console.WriteLine('\n');
            Console.WriteLine(new string('=', 24));
            ConsoleMessages.PressToContinue();

            break;
        }
        
        case ConsoleKey.D0:
            isRunning = false;
            break;
    }
}    
client.Close();

