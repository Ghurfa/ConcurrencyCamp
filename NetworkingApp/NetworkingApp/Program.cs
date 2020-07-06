using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkingApp
{
    class Program
    {
        static void UDPClient()
        {
            //client-server

            UdpClient client = new UdpClient();
            byte[] data = { 7, 7, 0, 1 };
            int returnVal = client.Send(data, 4, "192.168.1.172", 9999);
        }
        static void UDPServer()
        {
            UdpClient server = new UdpClient(9999);
            
            IPEndPoint ipEp = default;
            while (true)
            {
                byte[] data = server.Receive(ref ipEp);
            }
        }
        static void TCPClient()
        {
            TcpClient client = new TcpClient();
            client.Connect("192.168.1.172", 9999);

            var stream = client.GetStream();
            byte[] data = new byte[] { 7, 107, 237, 0 };
            while (true)
            {
                stream.Write(data, 0, 4);
                stream.Read(data, 0, 4);
                for (int j = 0; j < data.Length; j++)
                {
                    Console.Write(data[j]);
                    Console.Write(" ");
                }
                Console.WriteLine();
                Thread.Sleep(700);
            }
        }
        static void TCPServer()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 9999);

            listener.Start();


            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[4];

                while (true)
                {
                    stream.Read(buffer, 0, 4);
                    buffer[0] += 1;
                    stream.Write(buffer);
                }
            }
        }
        static void Sockets()
        {

            UdpClient client1 = new UdpClient(9999);
            UdpClient client2 = new UdpClient(10000);

            Socket client1Socket = client1.Client;
            Socket client2Socket = client2.Client;

            List<Socket> readSockets = new List<Socket>()
            {
                client1Socket,
                client2Socket
            };
            while(true)
            {
                readSockets.Clear();
                readSockets.Add(client1Socket);
                readSockets.Add(client2Socket);
                Socket.Select(readSockets, null, null, 1000);
                if(readSockets.Count > 0)
                {

                }
            }
        }
        static void Main(string[] args)
        {
            Sockets();
        }
    }
}
