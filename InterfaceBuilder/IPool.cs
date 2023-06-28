using System.Collections.Generic;

namespace InterfaceBuilder
{
    public interface IPool<T> : IPoolItem where T : IPoolItem
    {
        void Add(T item);
        void Remove(T item);
    
        List<T> Items { get; }
    }
}