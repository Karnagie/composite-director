using System;
using System.Collections.Generic;
using System.Linq;
using CompositeDirectorOld;
using CompositeDirectorWithGeneratingComposites.CompositeDirector.CompositeGeneration;

namespace CompositeDirectorWithGeneratingComposites.CompositeDirector
{
    public class CompositeDirector : IDisposable
    {
        private readonly CompositeCreator _compositeCreator = new();
        private readonly List<IPoolItem> _composites = new ();
        private readonly List<IPoolItem> _allItems = new();

        public void Dispose()
        {
            foreach (var composite in _composites)
            {
                composite.Dispose();
            }
        }

        public void Add<T>(T item) where T : class, IPoolItem
        {
            if(_allItems.Contains(item))
                return;
            
            _allItems.Add(item);
            
            foreach (var composite in _composites)
            {
                (composite as IPool<T>)?.Add(item);
            }
        }

        public IPool<T> SetupComposite<T>() where T : class, IPoolItem
        {
            var composite = _compositeCreator.Create<T>();
            _composites.Add(composite);
            foreach (var item in _allItems)
            {
                composite.Add(item as T);
            }
            
            return composite;
        }

        public T GetComposite<T>() where T : class, IPoolItem
        {
            var composite = _composites.FirstOrDefault((pool => pool is T));
            if (composite != null)
                return composite as T;
            
            Debug.Log($"There is no composite for {typeof(T)}. Created new one"); 
            return SetupComposite<T>() as T;
        }
    }
}