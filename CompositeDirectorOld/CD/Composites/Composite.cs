using CompositeDirectorOld.CD.CompositeProviders;
using CompositeDirectorOld.CD.Executors;

namespace CompositeDirectorOld.CD.Composites
{
    public class Composite<T> where  T : IProcessExecutor
    {
        private readonly CompositeDirector _director;

        public Composite(CompositeDirector director)
        {
            _director = director;
        }

        public ICompositeContainer<T> Select()
            => _director.SelectComposite<T>();
    }
}