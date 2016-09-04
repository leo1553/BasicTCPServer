using System;

namespace BasicTCPServer {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("Leal's Cherno Controller\nCommands: exit\n");

            ReceiveAttribute.Register();
            Server.Start();

            while(true) {
                string s = Console.ReadLine();
                switch(s.ToLower()) {
                    case "exit":
                        break;
                }
            }
        }
    }
}
