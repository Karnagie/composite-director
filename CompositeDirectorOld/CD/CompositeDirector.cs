using System;
using System.Collections.Generic;
using System.Linq;
using CompositeDirectorOld.CD.CompositeProviders;
using CompositeDirectorOld.CD.Composites;
using CompositeDirectorOld.CD.Executors;

namespace CompositeDirectorOld.CD
{
    public class CompositeDirector : IDisposable
    {
        private readonly List<IProcessComposite> _composites;

        public CompositeDirector(IEnumerable<IProcessComposite> composites)
        {
            _composites = composites.Distinct().ToList();
        }

        public void TryAddExecutor<T>(T item)
        {
            if (item is IProcessExecutor concrete)
            {
                foreach (IProcessComposite service in _composites)
                {
                    service.TryAdd(concrete);
                }
            }
            else
            {
                Debug.LogError($"The {item}  is not implementing {nameof(IProcessExecutor)}");
            }
        }

        public ICompositeContainer<T> SelectComposite<T>() where T : IProcessExecutor
        {
            IProcessComposite processComposite = FindComposite<T>();
            CompositeContainer<T> container = new CompositeContainer<T>(processComposite);
            return container;
        }
        
        private IProcessComposite FindComposite<T>() where T : IProcessExecutor
        {
            try
            {
                return _composites.Find(container => container is T);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        public void Dispose()
        {
            foreach (var composite in _composites)
            {
                composite.Dispose();
            }
        }
    }
}