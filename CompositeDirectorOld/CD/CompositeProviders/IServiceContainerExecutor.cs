using CompositeDirectorOld.CD.Executors;

namespace CompositeDirectorOld.CD.CompositeProviders
{
    public interface IServiceContainerExecutor<T> where T : IProcessExecutor
    {
        T Do();
    }
}