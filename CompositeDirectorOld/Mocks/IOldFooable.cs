using CompositeDirectorOld.CD.Executors;

namespace CompositeDirectorOld.Mocks
{
    public interface IOldFooable : IProcessExecutor
    {
        void Foo();
        void FooWithArgs(string name);
    }
}