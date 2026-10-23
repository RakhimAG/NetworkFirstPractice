using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;

// SERVER

var listener = new Socket(
    AddressFamily.InterNetwork,
    SocketType.Stream,
    ProtocolType.Tcp);

listener.Bind(new IPEndPoint(IPAddress.Any, 5000));
listener.Listen();

ConcurrentDictionary<Socket, byte> connectedClients = new();

// finalMessage => [userName] : message\n
// OR events => [user] connected || disconnected
async Task BroadcastAsync(string finalMessage)
{
    byte[] data = Encoding.UTF8.GetBytes(finalMessage);

    foreach (Socket client in connectedClients.Keys)
    {
        try
        {
            await client.SendAsync
            (
                data,
                SocketFlags.None
            );
        }
        catch (SocketException)
        {
            connectedClients.TryRemove(client, out _);
            client.Dispose();
        }
        catch (ObjectDisposedException)
        {
            connectedClients.TryRemove(client, out _);
        }
    }
}

async Task HandleClient(Socket client)
{
    byte[] buffer = new byte[1024];

    var receivedName = await client.ReceiveAsync
    (
        buffer, SocketFlags.None
    );

    if (receivedName == 0)
    {
        client.Dispose();
        return;
    }

    connectedClients.TryAdd(client, 0);
    string clientName = Encoding.UTF8.GetString(buffer, 0, receivedName);

    Console.WriteLine($"New User [{clientName}] connected.");
    await BroadcastAsync($"New User [{clientName}] connected.");

    while (true)
    {
        try
        {
            var received = await client.ReceiveAsync
            (
                buffer, SocketFlags.None
            );

            if (received == 0)
            {
                client.Dispose();
                break;
            }
            string message = Encoding.UTF8.GetString(buffer, 0, received);
            await BroadcastAsync($"[{clientName}] {message}");

            Console.WriteLine($"[{clientName}] {message}");
        }
        catch (SocketException) 
        {
            client.Dispose();
            break;
        }
    }
    Console.WriteLine($"User [{clientName}] Disconnected");
    await BroadcastAsync($"User [{clientName}] Disconnected");
}

Console.WriteLine("Server started.");
while (true)
{
    Socket client = await listener.AcceptAsync();

    _ = HandleClient(client);
}
