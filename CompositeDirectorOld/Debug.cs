using System;

namespace CompositeDirectorOld
{
    public static class Debug
    {
        public static void Log(object message)
        {
#if UNITY_EDITOR
            Debug.Log(message);
#endif
            Console.WriteLine(message);
        }

        public static void LogError(object exception)
        {
#if UNITY_EDITOR
            Debug.LogError(message);
#endif
            Console.WriteLine(exception);
        }
    }
}