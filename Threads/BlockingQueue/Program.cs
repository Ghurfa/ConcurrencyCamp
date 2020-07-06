using System;
using System.Collections.Generic;
using System.Threading;

namespace BlockingQueue
{
    //needs to be thread safe
    class BlockingQueue<T>
    {
        private Queue<T> queue;
        private readonly object lockObj = new object();

        public void Enqueue(T data)
        {
            lock(lockObj)
            {
                queue.Enqueue(data);
                Monitor.PulseAll(lockObj);
            }
        }

        //block until data is added
        public T Dequeue()
        {
            lock(lockObj)
            {
                while(IsEmpty())
                {
                    Monitor.Wait(lockObj);
                }
                return queue.Dequeue();
            }
        }

        public bool IsEmpty()
        {
            lock(lockObj)
            {
                return queue.Count == 0;
            }
        }
    }


    class Program
    {
        static void ThreadMain()
        {

        }
        static void Main(string[] args)
        {
            Thread[] threads = new Thread[5];
            for(int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(ThreadMain);
                threads[i].Start();
            }
        }
    }
}
