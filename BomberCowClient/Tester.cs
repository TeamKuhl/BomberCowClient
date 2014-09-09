using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberCowClient
{
    class Tester
    {
        static void Main(string[] args)
        {
            // test
            Client client = new Client();
            client.connect("192.168.2.3", 45454);

            Int64 counter = 0;

            Console.Write("Enter your name: ");
            String name = Console.ReadLine();

            while (true)
            {
                counter++;
                
                String message = Console.ReadLine();
                client.send(name+": "+message);

            }
        }
    }
}
