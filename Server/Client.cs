using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace BasicTCPServer {
    public class Client {
        public const int bufferSize = 1024;

        public Socket socket;
        public byte[] buffer = new byte[bufferSize];

        string fraction = null;

        public Client(Socket socket) {
            this.socket = socket;
            socket.BeginReceive(buffer, 0, bufferSize, SocketFlags.None, new AsyncCallback(AsyncReceive), null);
        }

        public void Send(ReceiveID type, string text) {
            text = Server.checkText + (char)type + text + Server.EOP;
            socket.Send(Encoding.ASCII.GetBytes(text));
        }

        void AsyncReceive(IAsyncResult ar) {
            try {
                int length = socket.EndReceive(ar);
                if(length > 0) {
                    HandleInformation(Encoding.ASCII.GetString(buffer, 0, length));
                    socket.BeginReceive(buffer, 0, bufferSize, SocketFlags.None, new AsyncCallback(AsyncReceive), null);
                }
                else {
                    //Close Connection
                    socket.Close();
                    Server.clients.Remove(this);
                }
            }
            catch {
                //Time Out
                socket.Close();
                Server.clients.Remove(this);
            }
        }

        void HandleInformation(string text) {
            Console.WriteLine(text);
            if(fraction != null) {
                text = fraction + text;
                fraction = null;
            }

            List<string> package = new List<string>();
            int lastEOP = 0;
            for(int i = 0; i < text.Length; i++) {
                if(text[i] == Server.EOP) {
                    if(i - lastEOP == Server.checkText.Length)
                        continue;

                    package.Add(text.Substring(lastEOP, i - lastEOP));
                    lastEOP = i + 1;
                }
            }

            if(lastEOP != text.Length)
                fraction = text.Substring(lastEOP, text.Length - lastEOP);

            string s;
            for(int i = 0; i < package.Count; i++) {
                s = package[i];
                if(s.IndexOf(Server.checkText) != 0)
                    continue;

                ReceiveID type = (ReceiveID)(s[Server.checkText.Length]);
                if(ReceiveAttribute.all.ContainsKey(type))
                    ReceiveAttribute.all[type].Invoke(null,
                        ReceiveAttribute.all[type].IsStatic ?
                            new object[2] { this, s.Substring(Server.checkText.Length + 1) } :
                            new object[1] { s.Substring(Server.checkText.Length + 1) });
            }
        }
    }
}
