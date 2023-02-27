using CompositeDirectorOld.CD.Composites;
using CompositeDirectorOld.CD.Executors;
using InterfaceBuilder.CompositeGeneration;

namespace CompositeDirectorOld.Mocks
{
    public interface IOldFooable : IProcessExecutor
    {
        void Foo();
        void FooWithArgs(string name);
    }
}