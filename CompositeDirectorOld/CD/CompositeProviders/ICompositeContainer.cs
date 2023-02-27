using CompositeDirectorOld.CD.Executors;

namespace CompositeDirectorOld.CD.CompositeProviders
{
    public interface ICompositeContainer<T> : IServiceContainerExecutor<T> where T : IProcessExecutor
    {
        ICompositeScope<T> For<TSpecific>() where TSpecific : IProcessExecutor;
        ICompositeScope<T> ForAll();
    }
}