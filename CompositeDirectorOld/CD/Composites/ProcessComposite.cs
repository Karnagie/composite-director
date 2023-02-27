using System;
using System.Collections.Generic;
using System.Linq;
using CompositeDirectorOld.CD.Executors;

namespace CompositeDirectorOld.CD.Composites
{
    public abstract class ProcessComposite<T> : IProcessComposite where T : IProcessExecutor
    {
        private readonly List<T> _items = new();
        
        public abstract event Action<IProcessExecutor> Disposed;

        public IProcessExecutor[] Items => _items.Select(executor => executor as IProcessExecutor).ToArray();

        public void TryAdd(IProcessExecutor item)
        {
            if (item == this)
                throw new Exception($"Try adding service {this.GetType().Name} to itself");
            
            if (item is T matching)
            {
                _items.Add(matching);
                item.Disposed += TryRemove;
            }
        }

        public void TryRemove(IProcessExecutor item)
        {
            if (item is T matching)
            {
                item.Disposed -= TryRemove;
                _items.Remove(matching);
            }
        }

        public bool Contains(IProcessExecutor item)
        {
            return _items.Contains((T)item);
        }

        public void Dispose()
        {
            foreach (T representation in _items)
            {
                representation.Disposed -= TryRemove;
                representation.Dispose();
            }
            _items.Clear();
            
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing) { }
        
        public abstract IProcessComposite Clone();
    }
}