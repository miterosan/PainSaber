using System;
using System.Threading;
using System.Threading.Tasks;

namespace PainSaber.Utils
{
    public static class AsyncUtils
    {
        public static void RunWithCallback<T>(Task<T> task, Action<T> callback, Action<Exception> onException = null)
        {
            new Thread(async () => {
                T value;

                try {
                    value = await task;
                } catch (Exception e)
                {
                    onException?.Invoke(e);
                    return;
                }

                callback.Invoke(value);
            }).Start();
        }

        public static void FireAndForget(Task task) {
            new Thread(async () => {
                await task;
            }).Start();
        }



    }
}