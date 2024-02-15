using System;
using System.Threading;
using System.Threading.Tasks;

namespace PainSaber
{
    public static class AsyncUtils
    {
        public static void RunWithCallback<T>(Task<T> task, Action<T> callback) 
        {
            new Thread(async () => {
                callback.Invoke(await task);
            }).Start();
        }

        public static void FireAndForget(Task task) {
            new Thread(async () => {
                await task;
            }).Start();
        }
    }
}