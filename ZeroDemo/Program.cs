﻿using NetMQ;
using NetMQ.Sockets;

namespace ZeroDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var server = new ResponseSocket("@tcp://localhost:5557")) // bind
            using (var client = new RequestSocket(">tcp://localhost:5557"))  // connect
            {
                // Send a message from the client socket
                client.SendFrame("Hello");

                // Receive the message from the server socket
                string m1 = server.ReceiveFrameString();
                Console.WriteLine("From Client: {0}", m1);

                // Send a response back from the server
                server.SendFrame("Hi Back");

                // Receive the response from the client socket
                string m2 = client.ReceiveFrameString();
                Console.WriteLine("From Server: {0}", m2);
            }
        }
    }
}
