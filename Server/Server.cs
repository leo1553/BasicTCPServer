using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BasicTCPServer {
    public static class Server {
        public const int port = 15530;
        public const string checkText = "RBA";
        public const char EOP = '\0';

        public static Socket openSocket;
        public static List<Client> clients = new List<Client>();

        public static Socket udpSocket;
        const int bufferSize = 64;
        static byte[] buffer = new byte[bufferSize];

        public static void Start() {
            Console.WriteLine("Starting server...");

            openSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            openSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            openSocket.Listen(5);
            openSocket.BeginAccept(new AsyncCallback(AsyncAccept), null);

            EndPoint ipEP = new IPEndPoint(IPAddress.Any, port);
            udpSocket = new Socket(ipEP.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            udpSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            udpSocket.BeginReceiveFrom(buffer, 0, bufferSize, SocketFlags.None, ref ipEP, new AsyncCallback(AsyncReceive), ipEP);

            Console.WriteLine("Server started. Waiting for clients...");
        }

        static void AsyncAccept(IAsyncResult ar) {
            try {
                Client client = new Client(openSocket.EndAccept(ar));
                Console.WriteLine("New Client: " + (IPEndPoint)client.socket.RemoteEndPoint);

                clients.Add(client);
                openSocket.BeginAccept(new AsyncCallback(AsyncAccept), null);
            }
            catch(Exception e) {
                Log.WriteError(e);
            }
        }

        static void AsyncReceive(IAsyncResult ar) {
            Console.WriteLine("a");
            try {
                EndPoint ipEP = (EndPoint)ar.AsyncState;
                int length = udpSocket.EndReceiveFrom(ar, ref ipEP);
                if(length > 0) {
                    string msg = Encoding.ASCII.GetString(buffer, 0, length);
                    if(msg == checkText) {
                        udpSocket.SendTo(Encoding.ASCII.GetBytes(checkText + "Host"), ipEP);
                        Console.WriteLine("Received " + msg + " from " + ((IPEndPoint)ipEP).Address + ":" + ((IPEndPoint)ipEP).Port);
                    }
                }

                ipEP = new IPEndPoint(IPAddress.Any, port);
                udpSocket.BeginReceiveFrom(buffer, 0, bufferSize, SocketFlags.None, ref ipEP, new AsyncCallback(AsyncReceive), ipEP);
            }
            catch(Exception e) {
                Log.WriteError(e);
            }
        }

        public static void Broadcast(ReceiveID type, string text) {
            text = checkText + (char)type + text + EOP;
            foreach(Client c in clients)
                c.socket.Send(Encoding.ASCII.GetBytes(text));
        }
    }
}
