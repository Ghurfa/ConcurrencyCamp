using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Parallelism
{
    class Program
    {
        static void adding()
        {
            Stopwatch sw = Stopwatch.StartNew();

            int sum = 0;
            Parallel.For(0, 10000, (i, s) =>
            {
                Interlocked.Add(ref sum, i);
            });
            sw.Stop();
            Console.WriteLine(sw.ElapsedTicks / (double)Stopwatch.Frequency);
            Console.WriteLine(sum);

            sum = 0;
            sw.Reset();
            sw.Start();
            for (int i = 0; i < 100000; i++)
            {
                sum += i;
            }
            sw.Stop();
            Console.WriteLine(sw.ElapsedTicks / (double)Stopwatch.Frequency);
            Console.WriteLine(sum);
        }
        public unsafe struct LargeStruct
        {
            public fixed long data[1000];
        }
        static unsafe void largeStructInit()
        {
            for (int h = 5; h <= 500_000; h *= 10)
            {
                Console.WriteLine($"{h} items");
                Stopwatch sw = Stopwatch.StartNew();
                LargeStruct[] items = new LargeStruct[h];
                Parallel.For(0, items.Length, (i) =>
                {
                    ref LargeStruct s = ref items[i];
                    for (int j = 0; j < 1000; j++)
                    {
                        s.data[j] = i;
                    }
                });

                sw.Stop();
                Console.WriteLine(sw.ElapsedTicks / (double)Stopwatch.Frequency);
                sw.Reset();
                sw.Start();

                items = new LargeStruct[h];
                for (int i = 0; i < items.Length; i++)
                {
                    ref LargeStruct s = ref items[i];
                    for (int j = 0; j < 1000; j++)
                    {
                        s.data[j] = i;
                    }
                }
                sw.Stop();
                Console.WriteLine(sw.ElapsedTicks / (double)Stopwatch.Frequency);
                Console.WriteLine();
            }
        }

        class StructWrapper
        {
            public LargeStruct inner;
        }

        static unsafe void largeStructInitForeach()
        {
            for (int h = 5; h <= 500_000; h *= 10)
            {
                Console.WriteLine($"{h} items");
                Stopwatch sw = Stopwatch.StartNew();
                StructWrapper[] items = new StructWrapper[h];
                Parallel.ForEach(items, (i)=>
                {
                    i.inner = new LargeStruct();
                });

                sw.Stop();
                Console.WriteLine(sw.ElapsedTicks / (double)Stopwatch.Frequency);
            }
        }
        static void mergeSortTest()
        {
            Random random = new Random();
            int[] items = new int[100];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = random.Next(0, 512);
            }

            int[] items2 = new int[items.Length];
            int[] items3 = new int[items.Length];
            items.CopyTo(items2, 0);
            items.CopyTo(items3, 0);

            Stopwatch sw = Stopwatch.StartNew();
            Array.Sort(items);
            sw.Stop();
            Console.WriteLine($"Array.Sort: {sw.ElapsedTicks / (double)Stopwatch.Frequency}");

            int processorCount = Environment.ProcessorCount;
        }
        static void Main(string[] args)
        {
            mergeSortTest();

            Console.ReadKey();
        }
    }
}
