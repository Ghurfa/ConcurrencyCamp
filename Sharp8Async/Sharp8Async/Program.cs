using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Sharp8Async
{
    public interface InterfaceWithImplementation
    {
        int A { get; }
        int B => A + 3;

    }

    public class DisposableClass : IDisposable, IAsyncDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }
    }


    class Program
    {
        static async IAsyncEnumerable<int> GetItems(Stream reader)
        {
            byte[] bytes = new byte[4];
            while (reader.Position < reader.Length)
            {
                await reader.ReadAsync(bytes.AsMemory());
                yield return BitConverter.ToInt32(bytes);
            }
        }
        static void TupleSwitch((string first, string second) tuple)
        {
            switch (tuple)
            {
                case ("abc", "def"):
                    break;
                case ("ghi", "jkl"):
                    break;
            }

        }
        static async Task Main(string[] args)
        {
            byte[] data = new byte[16];
            BitConverter.TryWriteBytes(data.AsSpan(), 107);
            BitConverter.TryWriteBytes(data.AsSpan().Slice(4), 2370);
            BitConverter.TryWriteBytes(data.AsSpan().Slice(8), 919);
            BitConverter.TryWriteBytes(data.AsSpan().Slice(12), 867);
            MemoryStream stream = new MemoryStream(data);
            await foreach (int val in GetItems(stream).WhereAwait(async x =>
             {
                //await Task.Delay(100);
                return x % 3 != 1;
             }))
            {
                Console.WriteLine(val);
            }

            Range range = 1..5;
            Random random = new Random();
            int x = random.Next(2, 5);
            Range range2 = (x)..^(x); //^x = x spaces from end
            byte[] dataSlice = data[range];
            byte[] dataSlice2 = data[range2];

            await using(DisposableClass disposable = new DisposableClass())
            {

            }
        }
    }
}
