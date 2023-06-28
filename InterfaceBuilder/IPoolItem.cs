using System;

namespace InterfaceBuilder
{
    public interface IPoolItem : IDisposable
    {
        event Action Disposed;
    }
}