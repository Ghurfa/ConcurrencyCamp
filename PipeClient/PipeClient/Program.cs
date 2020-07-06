using System;
using System.IO.Pipes;

namespace PipeClient
{
    class Program
    {
        static void Main(string[] args)
        {
            NamedPipeClientStream stream = new NamedPipeClientStream();
            Console.WriteLine("Hello World!");
        }
    }
}
