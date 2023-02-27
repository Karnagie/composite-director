using CompositeDirectorOld.CD.Executors;

namespace CompositeDirectorOld.CD.CompositeProviders
{
    public interface ICompositeScope<T>  :  IServiceContainerExecutor<T> where  T : IProcessExecutor
    {
        ICompositeScope<T> Select<TSpecific>() where TSpecific : IProcessExecutor;
        ICompositeScope<T> Except<TSpecific>() where TSpecific : IProcessExecutor;
    }
}