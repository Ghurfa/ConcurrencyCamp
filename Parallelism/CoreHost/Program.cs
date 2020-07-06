using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Pipes;
using System.IO;

namespace CoreHostm
{
    class Program
    {
        static void MainHost1()
        {
            string exeLocation = AppDomain.CurrentDomain.BaseDirectory + "CoreHost.exe";
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = exeLocation;
            startInfo.Arguments = "asd";
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardInput = true;
            startInfo.UseShellExecute = false;

            Console.WriteLine("Starting child process");
            Process process = Process.Start(startInfo);
            while (true)
            {
                process.StandardInput.WriteLine(Console.ReadLine());
                string output = process.StandardOutput.ReadLine();
                Console.WriteLine(output);
            }
        }
        static void MainHost2()
        {
            Console.WriteLine("Server: ");
            string exeLocation = AppDomain.CurrentDomain.BaseDirectory + "CoreHost.exe";
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = exeLocation;

            NamedPipeServerStream serverIn = new NamedPipeServerStream("Server-in", PipeDirection.In);
            NamedPipeServerStream serverOut = new NamedPipeServerStream("Server-out", PipeDirection.InOut);

            startInfo.Arguments = "Server-in Server-out";

            //Process process = Process.Start(startInfo);

            serverOut.WaitForConnection();
            StreamWriter writer = new StreamWriter(serverOut);
            writer.AutoFlush = true;
            while (true)
            {
                writer.WriteLine(Console.ReadLine());
            }
        }

        static async Task HandleClient(NamedPipeServerStream stream)
        {
            using (stream)
            using (StreamReader reader = new StreamReader(stream))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                string line = reader.ReadLine();
                await writer.WriteLineAsync(line + "!");
            }
        }
        static NamedPipeServerStream GetNewStream()
        {
            return new NamedPipeServerStream("server-in", PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
        }
        static async void MainHost3()
        {
            NamedPipeServerStream stream = GetNewStream();
            LinkedList<Task> taskList = new LinkedList<Task>();
            Task connectTask = stream.WaitForConnectionAsync();
            taskList.AddFirst(connectTask);

            while (true)
            {
                Task completedTask = await Task.WhenAny(taskList);
                if (completedTask == connectTask)
                {
                    var localStream = stream;
                    stream = GetNewStream();
                    connectTask = stream.WaitForConnectionAsync();
                    taskList.First.Value = connectTask;
                    taskList.AddLast(HandleClient(localStream));
                }
                else
                {
                    taskList.Remove(completedTask);
                }
            }
        }
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                MainHost2();
            }
            else
            {
                try
                {
                    Client.MainClient(args);
                }
                finally
                {
                    Console.ReadLine();
                }
            }
        }
    }
}
