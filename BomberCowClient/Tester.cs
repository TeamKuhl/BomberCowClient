using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BomberLib;

namespace BomberCowClient
{
    class Tester
    {
        static void Main(string[] args)
        {
            Console.Title = "BomberChatTest";

            // test
            Client client = new Client();

            Int64 counter = 0;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Enter your name: ");
            String name = Console.ReadLine();

            if (client.connect("172.25.66.17", 45454))
            {
                client.send(name + " joined");
                Console.WriteLine("You joined the server");

                Console.ResetColor();

                while (true)
                {
                    counter++;

                    String message = Console.ReadLine();
                    if (message == "/leave")
                    {
                        client.send(name + " leaved");
                        Environment.Exit(0);
                    }
                    else if (message == "/nick")
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("New nickname: ");
                        String oldname = name;
                        name = Console.ReadLine();
                        Console.WriteLine("Your name is now " + name);
                        Console.ResetColor();
                        client.send(oldname + " changed his name to " + name);

                    }
                    else if (message == "") { }
                    else client.send(name + ": " + message);

                }
            }
            else
            {
                Console.WriteLine("Press any key to Exit!");
                Console.ReadLine();
                Environment.Exit(0);
            }
        }
    }
}
