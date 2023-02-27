using System.Collections.Generic;
using CompositeDirectorOld.CD.Executors;

namespace CompositeDirectorOld.CD.Composites
{
    public interface IProcessComposite : IProcessExecutor
    {
        IProcessExecutor[] Items { get; }
        void TryAdd(IProcessExecutor representation);
        void TryRemove(IProcessExecutor representation);
        bool Contains(IProcessExecutor item);
        IProcessComposite Clone();
    }
}