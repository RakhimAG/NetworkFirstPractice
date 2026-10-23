
Console.WriteLine("START");
var thread1 = new Thread(() =>
{
    Console.WriteLine("Helloo");
    Thread.Sleep(100);
    Console.WriteLine("waddup");
    Thread.Sleep(1000);
    Console.WriteLine("1s after brodder");
    Thread.Sleep(2000);
    Console.WriteLine("Chat ended chatting");
});
thread1.Start();
var result =await Console.In.ReadLineAsync();


Console.WriteLine($"RESULT : {result}");
Console.WriteLine("END");