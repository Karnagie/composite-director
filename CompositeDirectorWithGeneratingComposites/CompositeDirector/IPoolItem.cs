using System;

namespace CompositeDirectorWithGeneratingComposites.CompositeDirector
{
    public interface IPoolItem : IDisposable
    {
        event Action Disposed;
    }
}