using System;

namespace CompositeDirectorOld.CD.Executors
{
    public interface IProcessExecutor : IDisposable
    {
        event Action<IProcessExecutor> Disposed;
    }
}