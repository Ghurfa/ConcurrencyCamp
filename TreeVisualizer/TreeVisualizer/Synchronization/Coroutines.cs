using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TreeVisualizer.Synchronization
{
    public static class Coroutines
    {
        public static MonoGameSynchronizationContext SyncContext = new MonoGameSynchronizationContext();

        public static void StartCoroutine(Func<Task> coroutine)
        {
            coroutine();
        }
        public static Task StartCoroutineAsync(Func<Task> coroutine)
        {
            var oldSyncContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(SyncContext);
            async Task LocalExecute()
            {
                await coroutine();
            }
            Task toRet = LocalExecute();
            SynchronizationContext.SetSynchronizationContext(oldSyncContext);
            return toRet;
        }

        public static Task ContinueOnMainThread()
        {
            SynchronizationContext.SetSynchronizationContext(SyncContext);
            async Task LocalFunc()
            {
                await Task.Yield();
            }
            return LocalFunc();
        }

        public static void ExecuteContinuations()
        {
            SyncContext.ExecuteContinuations();
        }
    }
}
