using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BadRaceConditions
{
    class Program
    {
        public static object lock1 = new object();
        public static object lock2 = new object();
        static void deadlock()
        {
            Thread thread = new Thread(() =>
            {
                while (true)
                {
                    lock (lock1)
                    {
                        lock (lock2)
                        {
                            Console.WriteLine("Inside locks inside thread");
                        }
                    }
                }
            });

            thread.Start();

            while (true)
            {
                lock (lock2)
                {
                    lock (lock1)
                    {
                        Console.WriteLine("Inside locks outside thread");
                    }
                }
            }
        }
        static async Task Main(string[] args)
        {
            MyTask task = new MyTask((resolve, reject) =>
            {
                Task.Delay(500).ContinueWith((s) =>
                {
                    return File.ReadAllLinesAsync(@"\\GMRDC1\Folder Redirection\Lorenzo.Lopez\Documents\Concurrency Camp\asr.txt").ContinueWith((t) =>
                    {
                        if (t.IsCompletedSuccessfully)
                        {
                            resolve();
                        }
                        else
                        {
                            reject(t.Exception);
                        }
                    });
                });
            });
            Thread thr = new Thread(() => {
                task.ContinueWith(async tt =>
                {
                    await Task.Delay(500);
                }).ContinueWith((a) =>
                {
                    ;
                });
            });
            thr.Start();
            try
            {
                await task;
            }
            catch(FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadKey();
        }
    }
}
