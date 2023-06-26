using System;

namespace InterfaceBuilder
{
    public abstract class Entity : IPoolItem
    {
        protected virtual void DisposeInternal() { }
        
        public void Dispose()
        {
            DisposeInternal();
            Disposed?.Invoke();
        }

        public event Action Disposed;
    }
}