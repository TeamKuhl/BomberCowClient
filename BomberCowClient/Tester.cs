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

            while (true)
            {
                String message = Console.ReadLine();
                client.send(message);

            }
        }
    }
}
