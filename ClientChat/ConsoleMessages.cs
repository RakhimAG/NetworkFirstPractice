using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientChat;

public static class ConsoleMessages
{
    public static void PrintChatOptions()
    {
        Console.WriteLine("Your options : \n[1] SEND A MESSAGE\n[2] LOOK AT THE GLOBAL CHAT\n[0] EXIT THE PROGRAM");
    }
    public static void Hint(string message)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"[i] {message}");
        Console.ResetColor();
    }
    public static void PrintWarning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n[!] {message}");
        Console.ResetColor();
    }

    public static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n[ERR] {message}");
        Console.ResetColor();
    }
    public static void PressToContinue()
    {
        Hint("Press [E] to continue");
        while (Console.ReadKey(true).Key != ConsoleKey.E) ;
    }
}
