using CompositeDirectorOld.CD.Composites;
using CompositeDirectorOld.CD.Executors;

namespace CompositeDirectorOld.CD.CompositeProviders
{
    public class CompositeScope<T> : ICompositeScope<T> where T : IProcessExecutor
    {
        private readonly IProcessComposite _processComposite;

        public CompositeScope(IProcessComposite processComposite)
        {
            _processComposite = processComposite;
        }

        public ICompositeScope<T> Select<TSpecific>() where  TSpecific : IProcessExecutor
        {
            foreach (IProcessExecutor item in _processComposite.Items)
            {
                if(item is not TSpecific)
                    _processComposite.TryRemove(item);
            }

            return new CompositeScope<T>(_processComposite);
        }
        
        public ICompositeScope<T> Except<TSpecific>() where  TSpecific : IProcessExecutor
        {
            foreach (IProcessExecutor item in _processComposite.Items)
            {
                if(item is TSpecific)
                    _processComposite.TryRemove(item);
            }

            return new CompositeScope<T>(_processComposite);
        }

        public T Do()
        {
            return (T)_processComposite;
        }
    }
}