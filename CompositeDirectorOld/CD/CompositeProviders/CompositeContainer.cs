using System.Collections.Generic;
using System.Linq;
using CompositeDirectorOld.CD.Composites;
using CompositeDirectorOld.CD.Executors;

namespace CompositeDirectorOld.CD.CompositeProviders
{
    public class CompositeContainer<T> : ICompositeContainer<T> where T : IProcessExecutor
    {
        private readonly IProcessComposite _processComposite;
        private readonly List<T> _items;

        public CompositeContainer(IProcessComposite processComposite)
        {
            if(processComposite == null)
                return;
            _processComposite = processComposite.Clone();
            _items = processComposite.Items.ToList().ConvertAll(item => (T)item);
        }

        public ICompositeScope<T> For<TSpecific>() where TSpecific : IProcessExecutor
        {
            foreach (T item in _items)
            {
                if (item is TSpecific)
                    _processComposite.TryAdd(item);
            }
            
            return new CompositeScope<T>(_processComposite);
        }

        public ICompositeScope<T> ForAll()
        {
            AddAll();
            
            return new CompositeScope<T>(_processComposite);
        }

        public T Do()
        {
            AddAll();
            
            return (T) _processComposite;
        }
        
        private void AddAll()
        {
            foreach (T item in _items)
                _processComposite.TryAdd(item);
        }
    }
}