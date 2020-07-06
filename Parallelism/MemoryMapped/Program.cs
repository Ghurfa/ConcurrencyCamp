using System;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Threading;

namespace MemoryMapped
{
    struct DataStruct
    {
        public int X;
        public long Y;
        public double Z;
    }

    class Program
    {
        static void Main(string[] args)
        {
            MemoryMappedFile file = MemoryMappedFile.CreateOrOpen("memoryMappedFile", 1024, MemoryMappedFileAccess.ReadWrite);
            MemoryMappedViewAccessor accessor = file.CreateViewAccessor();

            DataStruct data = new DataStruct()
            {
                X = 2,
                Y = 1124353245,
                Z = 3.3
            };
            Mutex mutex = new Mutex(false, "TestMMFMutex");
            //Semaphore semaphore = new Semaphore(0, 100, "TestMMFSemaphore");

            accessor.Write(0, ref data);

            while(true)
            {
                var line = Console.ReadLine();
                byte[] bytes = Encoding.UTF8.GetBytes(line);
                int length = bytes.Length;
                mutex.WaitOne();
                accessor.Write(0, length);
                accessor.WriteArray(4, bytes, 0, length);
                mutex.ReleaseMutex();
            }

        }
    }
}
