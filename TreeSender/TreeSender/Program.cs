using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TreeSender
{
    class Program
    {
        static async Task SendTree(NetworkStream stream, RedBlackTree tree)
        {
            byte[] data = new byte[tree.Count * 2];
            int i = 0;
            foreach (RedBlackNode node in tree.PreOrder())
            {
                data[i] = (byte)node.Value;
                data[i + 1] = node.IsRed ? (byte)1 : (byte)0;
                i += 2;
            }

            await stream.WriteAsync(BitConverter.GetBytes(data.Length));
            await stream.WriteAsync(data);
        }
        static void Main(string[] args)
        {
            RedBlackTree tree = new RedBlackTree();
            TcpClient client = new TcpClient();
            client.Connect("localhost", 9999);

            var stream = client.GetStream();
            while (true)
            {
                string command = Console.ReadLine();
                if (command.Length < 3) continue;
                int number;
                switch (command.ToLower().Substring(0, 3))
                {
                    case "ins":
                        if (command.Length > 3) number = Convert.ToInt32(command.Substring(4, command.Length - 4));
                        else
                        {
                            Console.WriteLine("Type value to insert:");
                            number = Convert.ToInt32(Console.ReadLine());
                        }
                        tree.Insert(number);
                        SendTree(stream, tree);
                        Console.WriteLine($"Inserted value {number}");
                        break;
                    case "del":
                        if (command.Length > 3) number = Convert.ToInt32(command.Substring(4, command.Length - 4));
                        else
                        {
                            Console.WriteLine("Type value to delete:");
                            number = Convert.ToInt32(Console.ReadLine());
                        }
                        bool successful = tree.Delete(number);
                        SendTree(stream, tree);
                        Console.WriteLine($"Deleting value {number}: {successful.ToString()}");
                        break;
                }
            }
        }
    }
}
