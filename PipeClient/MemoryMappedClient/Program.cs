using System;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Threading;

namespace MemoryMappedClient
{
    class Program
    {
        static void Main(string[] args)
        {
            MemoryMappedFile file = MemoryMappedFile.CreateOrOpen("memoryMappedFile", 1024, MemoryMappedFileAccess.ReadWrite);
            MemoryMappedViewAccessor accessor = file.CreateViewAccessor();

            var mutex = Mutex.OpenExisting("TestMMFMutex");
            //var semaphore = Semaphore.OpenExisting("TestMMFSemaphore");
            Console.WriteLine("Client: ");
            while(true)
            {
                int length;

                mutex.WaitOne();
                accessor.Read(0, out length);
                if(length > 0)
                {
                    byte[] data = new byte[length];
                    accessor.ReadArray(4, data, 0, length);
                    accessor.Write(0, 0);
                    mutex.ReleaseMutex();
                    Console.WriteLine(Encoding.UTF8.GetString(data));
                }
                else
                {
                    mutex.ReleaseMutex();
                }
            }

        }
    }
}
