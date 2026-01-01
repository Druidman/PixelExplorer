using System.Threading;

public static class ThreadGuard
{
    public static int MainThreadId { get; private set; }

    public static void Initialize()
    {
        MainThreadId = Thread.CurrentThread.ManagedThreadId;
    }
}
