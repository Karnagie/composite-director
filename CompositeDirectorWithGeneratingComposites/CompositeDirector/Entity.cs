using System;

namespace CompositeDirectorWithGeneratingComposites.CompositeDirector
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