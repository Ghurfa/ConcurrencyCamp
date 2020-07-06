using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreHost
{
    public class Client
    {
        public static void MainClient(string[] args)
        {
            Console.WriteLine("Client: ");
            NamedPipeClientStream clientIn = new NamedPipeClientStream(args[1]); //Server out
            clientIn.Connect();
            NamedPipeClientStream clientOut = new NamedPipeClientStream(args[0]); //Server in
            StreamWriter writer = new StreamWriter(clientOut);
            StreamReader reader = new StreamReader(clientIn);
            while(true)
            {
                Console.WriteLine(reader.ReadLine());
            }
        }
    }
}
