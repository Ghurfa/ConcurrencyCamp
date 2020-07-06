using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace BadRaceConditions
{
    public class MyTask<T> : MyTask
    {
        public T Result { get; protected set; }
        public MyTask(Func<Action, Action<Exception>, T> funcToRun)
        {
            Result = funcToRun(() =>
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
    }
}
