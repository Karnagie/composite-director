using System;
using CompositeDirectorOld.CD.Composites;
using CompositeDirectorOld.CD.Executors;

namespace CompositeDirectorOld.Mocks
{
    public class MockComposite : ProcessComposite<IOldFooable>, IOldFooable
    {
        public override event Action<IProcessExecutor> Disposed;
        
        public override IProcessComposite Clone()
            => new MockComposite();

        public void Foo()
        {
            foreach (IOldFooable item in Items)
            {
                item.Foo();
            }
        }

        public void FooWithArgs(string name)
        {
            foreach (IOldFooable item in Items)
            {
                item.FooWithArgs(name);
            }
        }
    }
    
    public class Mock : IOldFooable
    {
        public event Action<IProcessExecutor> Disposed;

        public void Foo()
        {
            Console.WriteLine("Foo");
        }

        public void FooWithArgs(string name)
        {
            Console.WriteLine($"savable: {name}");
        }

        public void Dispose()
        {
            Disposed?.Invoke(this);
        }
    }

    public class NotSavableMock : IOldFooable
    {
        public event Action<IProcessExecutor> Disposed;
        
        public void Foo()
        {
            Console.WriteLine("not save foo");
        }
    
        public void FooWithArgs(string name)
        {
            Console.WriteLine($"not save {name}");
        }

        public void Dispose()
        {
            Disposed?.Invoke(this);
        }
    }
}