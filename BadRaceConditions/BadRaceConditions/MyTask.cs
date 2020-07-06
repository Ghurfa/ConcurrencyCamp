using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BadRaceConditions
{
    public class MyTask
    {
        public struct Awaiter : INotifyCompletion
        {
            private MyTask task;
            public Awaiter(MyTask task)
            {
                this.task = task;
            }
            public bool IsCompleted => task.IsCompletedSuccessfully;
            public void GetResult()
            {
                if (task.IsFaulted)
                {
                    if (task.exception is AggregateException ar)
                    {
                        if (ar.InnerExceptions.Count == 1)
                        {
                            throw ar.InnerException;
                        }
                        else throw ar;
                    }
                    else if (task.exception == null)
                    {
                        throw new Exception("Promise rejected without exception");
                    }
                    else throw task.exception;
                }
            }
            public void OnCompleted(Action continuation)
            {
                lock (task.lockObj)
                {
                    if(task.IsCompleted)
                    {
                        continuation();
                        return;
                    }
                    task.continuation += (t) =>
                    {
                        continuation();
                    };
                }
            }
        }

        protected readonly object lockObj = new object();

        protected Exception exception;

        public bool IsCompleted => IsCompletedSuccessfully || IsFaulted;
        public bool IsCompletedSuccessfully { get; protected set; }
        public bool IsFaulted { get; protected set; }

        protected Action<MyTask> continuation;

        protected MyTask()
        {
            awaiter = new Awaiter(this);
        }
        public MyTask(Action<Action, Action<Exception>> funcToRun)
        {
            awaiter = new Awaiter(this);
            funcToRun(() =>
            {
                lock (lockObj)
                {
                    IsCompletedSuccessfully = true;
                    IsFaulted = false;
                    continuation?.Invoke(this);
                }
            }, (ex) =>
            {
                lock (lockObj)
                {
                    exception = ex;
                    IsCompletedSuccessfully = false;
                    IsFaulted = true;
                    continuation?.Invoke(this);
                }
            });
        }

        public MyTask ContinueWith(Action<MyTask> continuation)
        {
            MyTask continueTask = new MyTask();
            Action<MyTask> onContinue = t =>
            {
                continuation(t);
                continueTask.continuation?.Invoke(continueTask);
            };
            this.continuation += onContinue;
            return continueTask;
        }
        public MyTask ContinueWith(Func<MyTask, Task> continuation)
        {
            MyTask continueTask = new MyTask();
            Action<MyTask> onContinue = t =>
            {
                Task awaitTask = continuation(t);
                awaitTask.ContinueWith((u) =>
                {
                    continueTask.continuation?.Invoke(continueTask);
                });
            };
            this.continuation += onContinue;
            return continueTask;
        }

        private Awaiter awaiter;
        public Awaiter GetAwaiter()
        {
            return awaiter;
        }
    }
}
